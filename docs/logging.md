# Logging

Application code continues to use `ILogger<T>`. The API registers Serilog through `builder.ConfigureLogging()`; `app.UseRequestLogging()` establishes a correlation scope and writes one completion event per HTTP request. Host disposal flushes and closes its logger. Configuration errors thrown before the host/logger is built still appear on stderr, rather than being guaranteed to reach the file sink.

## Development configuration

All new logging settings are in the `Serilog` section of `src/BuckleApp.Api/appsettings.Development.json`. That file is already ignored by Git; add the following section to your own local development configuration if it is missing. Existing Authentication and Resend settings are separate and must be preserved.

```json
{
  "Serilog": {
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.File",
      "Serilog.Formatting.Compact"
    ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      },
      {
        "Name": "File",
        "Args": {
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact",
          "path": "logs/buckleapp-.json",
          "rollingInterval": "Day",
          "fileSizeLimitBytes": 10485760,
          "rollOnFileSizeLimit": true,
          "retainedFileCountLimit": 14,
          "retainedFileTimeLimit": "7.00:00:00"
        }
      }
    ]
  }
}
```

The Serilog section controls this pipeline's levels and sinks. The older `Logging:LogLevel` settings are not where the Serilog minimum levels are configured.

Both console and files use newline-delimited compact JSON (CLEF): `@t` is UTC timestamp, `@mt` is message template, `@l` is the level (Information may be omitted), and `@x` is exception detail. Other properties remain individually searchable.

The relative path resolves under the process working directory. Running `dotnet run --project src/BuckleApp.Api` from the repository root writes under `<repo>/logs/`. IDE launch working directories may differ. Use an absolute path through configuration if needed.

Files roll daily in the sink's local calendar time, or after approximately 10 MiB. Retention applies both a seven-day age limit and a fourteen-file count limit; whichever is exceeded removes older files. Heavy logging can therefore retain less than seven days. Cleanup occurs when the sink opens/rolls files, not as a continuous background expiration job. Files can slightly exceed the size threshold by one event.

`logs/` is excluded from Git, Docker build context and API publish content.

## Log content

- Request completion: method, path (without query string), status, elapsed milliseconds, `TraceId`, `RequestId`.
- Application logs within a request share the same correlation IDs. `Application` and `Environment` are attached globally.
- Expected business rejections are Warning events without exception stack traces. Unexpected failures are Error events with server-side exception details. The completion summary is separate from the detailed exception event.
- Email delivery logs do not include recipients, subject, body, codes or provider response objects. The recoverable student-email exception path records only exception type to avoid recording provider payloads.
- Session credential staging is Debug and records only internal user/session IDs. It does not claim the surrounding transaction has already committed.
- Do not add request bodies, query strings, headers, Authorization values, passwords, login codes, tokens or configuration secrets to logs. Keep EF Core sensitive-data logging disabled. Review exception messages and any future third-party logging before enabling verbose levels.

The existing error middleware now returns a generic message for unexpected 500 responses. Its broader exception/response contract is unchanged and is a separate follow-up.

## Other environments

Without `Serilog:WriteTo`, the API falls back to compact JSON console output. To persist files in a container, explicitly mount a writable durable volume and configure the file path there. Centralized ingestion, production retention and access control depend on hosting; none is introduced here. Application logs are not stored in the business PostgreSQL database.

## Verification

A local HTTP smoke test with a fake application checked JSON file output and flush, correlation between application and completion events, 401/500 levels, one detailed exception log, generic 500 responses, and omission of sentinel secrets placed in headers, query strings and request bodies. No database or real email provider was used.

## Login service example

`LoginUserService` emits Debug events for attempt start, password/Student-code dispatch and generic 401 rejection. It emits Information for successful login only after the relevant transaction completes. Unexpected exceptions propagate to the central error middleware without being logged again by the service. No email, password, OTP or issued token is included.

For the development demonstration the `Serilog:MinimumLevel:Override` section includes:

```json
"BuckleApp.Application.Features.Authentication.LoginUser.LoginUserService": "Debug"
```

Remove this override or set it to Information to hide the detailed flow. A successful password login appears approximately as:

```text
[14:25:10 DBG] Login attempt started
[14:25:10 DBG] Verifying password login for user <user-id>
[14:25:10 INF] User <user-id> logged in using Password
```

Student success uses `StudentCode` as AuthenticationMethod. Failed credentials produce a generic Debug rejection, followed by the existing middleware/HTTP summaries. These are diagnostic logs, not a durable security audit trail.
