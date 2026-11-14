# Student authentication

The existing Identity user, role manager, JWT generator, email service, repositories and Unit of Work are reused. Students have no password. Existing students' old passwords are not removed by the migration, but password login and password reset reject Student accounts. All roles use `POST /api/Auth/login` with the same `email` and `password` fields. For Students, `password` contains the emailed code; for other roles it contains the actual password. The server selects the verification method from stored roles. Every successful login returns the same access/refresh authentication response.

## Configuration and deployment

Set `Authentication__LoginCodeHashKey` to a Base64-encoded cryptographically random secret containing at least 32 bytes. Generate it with `openssl rand -base64 32` and store it in the deployment secret manager. All API instances must share this key. Startup fails if it is missing or too short. Keep it separate from the existing `Token` JWT signing key. Configure the existing `Resend` email settings. Neither key nor login codes should be logged.

Review and apply migration `20261111120000_StudentAuthentication` using the existing EF tooling and deployment process. It adds `UserLoginCodes`, `RefreshTokens`, and `AspNetUsers.IsDisabled`, and deletes only obsolete `OneTimeLoginCode` claims. It preserves user accounts and existing business tables. No migrations are applied on application startup. Do not run Down against live sessions: it drops the newly introduced authentication tables.

## API

All endpoints use JSON and the `/api/Auth` prefix.

| POST endpoint | Request | Response |
| --- | --- | --- |
| `student/request-code` | `{ "email": "student@example.com" }` | Generic 200 message for missing, ineligible, throttled and eligible accounts. Never returns a code. |
| `login` | `{ "email": "student@example.com", "password": "123456" }` (Student code) or actual password for other roles | Authentication response, or generic 401. |
| `refresh` | `{ "refreshToken": "..." }` | Rotated authentication response, or generic 401. |
| `logout` | `{ "refreshToken": "..." }` | 200; revokes that token family. Invalid tokens also return 200. |

Authentication responses contain `accessToken`, `refreshToken`, `accessTokenExpiresAt`, and `refreshTokenExpiresAt` (UTC). Access tokens contain existing user, organization and role claims; send them as `Authorization: Bearer ...`. Every successful login starts a separate refresh family per device. Refresh expiry is absolute, 30 days from login; rotation does not extend it. Replay of any revoked token revokes the whole family, including its successor. Clients must serialize refresh calls and atomically replace their stored token after success. Do not retry an old token after an ambiguous refresh failure; authenticate again if necessary.

Logout revokes refresh credentials, not the Identity account. Already issued access tokens remain valid for up to 15 minutes; disabled, deleted and locked accounts are rejected by bearer validation. After logout or expiry, request another email code. No passwords are needed for students.

## Limits and persistence

Codes are six cryptographically generated digits, valid for 10 minutes with at most five wrong attempts. Only an HMAC-SHA256 hash bound to user ID and code-record ID is stored. New issuance invalidates prior codes. Issuance has a persistent one-minute cooldown and five-per-hour account limit, including failed delivery attempts. Verification is limited to five failures per issued code. Authentication endpoints additionally share a 20-requests-per-minute IP limiter (429); this limiter is per API instance. Configure trusted proxy handling explicitly before using forwarded client addresses. The persistent account limits and PostgreSQL locks apply across instances.

All mutations lock the owning Identity user row in a transaction. That prevents concurrent code consumption or refresh rotation from both succeeding. Refresh credentials contain 64 random bytes, with only SHA-256 hashes stored. Security-stamp changes invalidate refresh eligibility. Student OTP eligibility requires the Student role alone, an enabled account, an active organization and no current lockout. Refresh supports all four roles under the same account/organization eligibility checks. Mixed privileged/Student accounts are deliberately denied both student OTP and password login until their roles are corrected.

Registration is authorized: SuperAdmin can create Admin, Instructor or Student accounts; Admin can create Instructor or Student accounts in their own organization; Instructor can create Student accounts in their own organization. SuperAdmin creation is restricted to an administrative bootstrap process outside this endpoint. Instructor-created students are linked to that instructor. Identity account creation and role assignment are transactional. Student email failure leaves a recoverable account: the code is invalidated, and a new request is allowed after the cooldown. Email delivery happens inside the user transaction; configure provider timeouts to keep locks bounded. No external email send can be atomic with a database commit; if commit fails after delivery, the student can request a replacement.

## Mobile clients and operations

Store refresh tokens using iOS Keychain or Android Keystore-backed encrypted storage. Keep access tokens in memory where practical, transmit credentials only over HTTPS, clear local credentials on logout, and never put tokens in URLs or analytics. Browser clients need their own secure storage strategy; this implementation returns tokens in JSON and does not set cookies.

Keep revoked token records until their families expire so replay remains detectable. Expired code and session history may be periodically pruned after the issuance-rate window and family expiration respectively. Rotating the HMAC secret invalidates outstanding codes. Existing unrelated business-resource authorization rules are not replaced by this authentication flow.

## Code organization

HTTP actions delegate through `IAuthService` / `AuthService`, just like the existing password login and reset actions. Each operation has a feature folder containing its request, service and service interface:

- `RequestStudentLoginCode`
- `VerifyStudentLoginCode`
- `RefreshSession`
- `LogoutUser`

Registration reuses `IRequestStudentLoginCodeService`. Login delegates Student credentials to `IVerifyStudentLoginCodeService`; code verification stays an internal feature with no separate public verification endpoint. Password login, verification and refresh reuse `IAuthenticationSessionService` in `Authentication/Services`. The shared response is in `Application/Dtos/AuthenticationResponse.cs`. Code hashing is implemented by `Infrastructure/Security/LoginCodeHasher` behind `ILoginCodeHasher`. Shared eligibility rules remain in Application. Transaction handling and persistence remain in the existing Data repositories; this structural refactor does not change the database model or migration.

## Verification

The refactor is verified by building the solution and running temporary PostgreSQL regression checks against the separate `buckleapp_auth_tests` database with a fake email adapter. Checks cover DI resolution through the actual feature registrations, unified password/OTP login, student registration, shared code cooldown, invalid code rejection, concurrent OTP consumption, refresh rotation/replay, and logout for password and OTP sessions. No real emails are sent and the application database is not used.

`AuthenticationSessionService.CreateAsync(User user, Guid familyId, DateTime? expiresAt = null)` is a shared Application service, not a separate feature. It stages the refresh-token record without saving; login, code verification and refresh own the surrounding transaction. No request DTO or public endpoint is required for this internal operation.

## Central lifetime configuration

`AuthenticationOptions` in `Infrastructure/Security` binds the `Authentication` section through `ConfigureAuthenticationOptions()` in the API. Options are validated on startup. During development, all Authentication settings are kept in the existing `Authentication` section of `appsettings.Development.json`; `appsettings.json` has no Authentication section. The lifetime values are:

```json
{
  "Authentication": {
    "AccessTokenLifetimeMinutes": 15,
    "RefreshSessionLifetimeDays": 30,
    "LoginCodeLifetimeMinutes": 10,
    "IdentityTokenLifetimeMinutes": 60
  }
}
```

Allowed ranges: access/Identity lifetimes 1–1440 minutes, login codes 1–60 minutes, refresh sessions 1–365 days. Existing secret `LoginCodeHashKey` stays in User Secrets/environment configuration.

The shared session service calculates access expiry once and passes it to `JwtGenerator`, so the response timestamp matches JWT `exp`. It also calculates the default refresh expiry for new logins. Rotation passes the stored expiry without extending it. Identity Data Protection token options derive their lifetime from the same configuration; the previous separate `ConfigureIdentityToken()` registration and unused `JwtHandler` have been removed. Configuration changes apply to newly issued credentials and do not rewrite existing tokens or refresh-session expirations. Keep lifetime configuration consistent across API instances.
