using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BHSystem.API.Migrations
{
    public partial class initialv2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-002",
                column: "Icon",
                value: "fa-solid fa-people-roof");

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-003",
                column: "Icon",
                value: "fa-solid fa-user-check");

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-004",
                column: "Icon",
                value: "fa-solid fa-user-pen");

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-005",
                column: "Icon",
                value: "fa-solid fa-users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-002",
                column: "Icon",
                value: "fa fa-chart-bar");

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-003",
                column: "Icon",
                value: "fa fa-chart-bar");

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-004",
                column: "Icon",
                value: "fa fa-chart-bar");

            migrationBuilder.UpdateData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-005",
                column: "Icon",
                value: "fa fa-chart-bar");
        }
    }
}
