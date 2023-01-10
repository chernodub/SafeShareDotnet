#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;

namespace SafeShare.Migrations.ShareableResources;

/// <inheritdoc />
public partial class AddResourceLifetime : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            "ExpirationDate",
            "Messages",
            "ExpiresAt");

        migrationBuilder.AddColumn<bool>(
            "IsOneTimeUse",
            "Messages",
            "boolean",
            nullable: false,
            defaultValue: false);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            "ExpiresAt",
            "Messages",
            "ExpirationDate");

        migrationBuilder.DropColumn(
            "IsOneTimeUse",
            "Messages");
    }
}