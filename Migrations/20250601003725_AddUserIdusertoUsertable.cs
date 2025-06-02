using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactKeeper.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdusertoUsertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "UserContact");

            migrationBuilder.AlterColumn<string>(
                name: "DDI",
                table: "UserContact",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "LocalNumber",
                table: "UserContact",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "userId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocalNumber",
                table: "UserContact");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "User");

            migrationBuilder.AlterColumn<int>(
                name: "DDI",
                table: "UserContact",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "UserContact",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
