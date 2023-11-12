using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BHSystem.API.Migrations
{
    public partial class initialv5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "Role_Id", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Role_Id", "UserId", "Date_Create", "Date_Update", "IsDeleted", "User_Create", "User_Update" },
                values: new object[] { 1, 2, null, null, false, null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "Role_Id", "UserId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Role_Id", "UserId", "Date_Create", "Date_Update", "IsDeleted", "User_Create", "User_Update" },
                values: new object[] { 1, 1, null, null, false, null, null });
        }
    }
}
