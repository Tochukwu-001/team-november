using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDisciplineToRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discipline",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Users",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE "Users"
                SET "Discipline" = 'General'
                WHERE COALESCE(NULLIF(TRIM("Discipline"), ''), '') = '';
                """);

            migrationBuilder.Sql("""
                UPDATE "Users"
                SET "PublicId" = CONCAT(
                    'TF-',
                    EXTRACT(YEAR FROM NOW())::text,
                    '-',
                    UPPER(SUBSTRING(REPLACE("Id"::text, '-', '') FROM 1 FOR 6))
                )
                WHERE COALESCE(NULLIF(TRIM("PublicId"), ''), '') = '';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PublicId",
                table: "Users",
                column: "PublicId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_PublicId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Discipline",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Users");
        }
    }
}
