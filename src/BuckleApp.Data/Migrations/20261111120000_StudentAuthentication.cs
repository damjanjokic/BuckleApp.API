using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BuckleApp.Data.Migrations;

public partial class StudentAuthentication : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(name: "IsDisabled", table: "AspNetUsers", type: "boolean", nullable: false, defaultValue: false);
        migrationBuilder.CreateTable(name: "UserLoginCodes", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            CodeHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            ConsumedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
            FailedAttempts = table.Column<int>(type: "integer", nullable: false)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_UserLoginCodes", x => x.Id);
            table.ForeignKey("FK_UserLoginCodes_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
            table.CheckConstraint("CK_UserLoginCodes_Attempts", "\"FailedAttempts\" BETWEEN 0 AND 5");
        });
        migrationBuilder.CreateTable(name: "RefreshTokens", columns: table => new
        {
            Id = table.Column<Guid>(type: "uuid", nullable: false),
            UserId = table.Column<Guid>(type: "uuid", nullable: false),
            FamilyId = table.Column<Guid>(type: "uuid", nullable: false),
            TokenHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
            SecurityStamp = table.Column<string>(type: "text", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
        }, constraints: table =>
        {
            table.PrimaryKey("PK_RefreshTokens", x => x.Id);
            table.ForeignKey("FK_RefreshTokens_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", onDelete: ReferentialAction.Cascade);
        });
        migrationBuilder.CreateIndex("IX_UserLoginCodes_UserId_CreatedAt", "UserLoginCodes", new[] { "UserId", "CreatedAt" });
        migrationBuilder.CreateIndex("IX_UserLoginCodes_UserId", "UserLoginCodes", "UserId", unique: true, filter: "\"ConsumedAt\" IS NULL");
        migrationBuilder.CreateIndex("IX_RefreshTokens_TokenHash", "RefreshTokens", "TokenHash", unique: true);
        migrationBuilder.CreateIndex("IX_RefreshTokens_UserId_FamilyId", "RefreshTokens", new[] { "UserId", "FamilyId" });
        migrationBuilder.Sql("DELETE FROM \"AspNetUserClaims\" WHERE \"ClaimType\" = 'OneTimeLoginCode'");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("UserLoginCodes");
        migrationBuilder.DropTable("RefreshTokens");
        migrationBuilder.DropColumn("IsDisabled", "AspNetUsers");
    }
}
