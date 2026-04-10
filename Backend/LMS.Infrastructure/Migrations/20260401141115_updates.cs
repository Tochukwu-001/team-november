using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Answer",
                table: "Submissions",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentContentType",
                table: "Submissions",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentName",
                table: "Submissions",
                type: "character varying(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "AttachmentSizeBytes",
                table: "Submissions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachmentUrl",
                table: "Submissions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Lessons",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EstimatedMinutes",
                table: "Lessons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "LessonContents",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachmentContentType",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "AttachmentName",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "AttachmentSizeBytes",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "AttachmentUrl",
                table: "Submissions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "EstimatedMinutes",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "LessonContents");

            migrationBuilder.AlterColumn<string>(
                name: "Answer",
                table: "Submissions",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(5000)",
                oldMaxLength: 5000,
                oldNullable: true);
        }
    }
}
