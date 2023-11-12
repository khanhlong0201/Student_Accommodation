using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BHSystem.API.Migrations
{
    public partial class initialv4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Date_Create", "Date_Update", "IsDeleted", "Name", "User_Create", "User_Update" },
                values: new object[,]
                {
                    { 1, null, null, false, "Admintrator", null, null },
                    { 2, null, null, false, "Bussiness Partner", null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Address", "Date_Create", "Date_Update", "Email", "FullName", "IsDeleted", "Password", "PasswordReset", "Phone", "Type", "UserName", "User_Create", "User_Update", "Ward_Id" },
                values: new object[,]
                {
                    { 1, null, null, null, null, "Khách Vãn Lai", false, "", null, null, "Client", "", null, null, 1 },
                    { 2, null, null, null, null, "Testing", false, "KZY9mwl2Mv4NM4jrKXv4ug==", null, null, "Admin", "testing", null, null, 1 }
                });

            migrationBuilder.InsertData(
                table: "RoleMenus",
                columns: new[] { "Menu_Id", "Role_Id", "Date_Create", "Date_Update", "IsDeleted", "User_Create", "User_Update" },
                values: new object[,]
                {
                    { "000-001", 1, null, null, false, null, null },
                    { "000-002", 1, null, null, false, null, null },
                    { "000-003", 1, null, null, false, null, null },
                    { "000-004", 1, null, null, false, null, null },
                    { "000-005", 1, null, null, false, null, null },
                    { "000-006", 1, null, null, false, null, null }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Role_Id", "UserId", "Date_Create", "Date_Update", "IsDeleted", "User_Create", "User_Update" },
                values: new object[] { 1, 1, null, null, false, null, null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RoleMenus",
                keyColumns: new[] { "Menu_Id", "Role_Id" },
                keyValues: new object[] { "000-001", 1 });

            migrationBuilder.DeleteData(
                table: "RoleMenus",
                keyColumns: new[] { "Menu_Id", "Role_Id" },
                keyValues: new object[] { "000-002", 1 });

            migrationBuilder.DeleteData(
                table: "RoleMenus",
                keyColumns: new[] { "Menu_Id", "Role_Id" },
                keyValues: new object[] { "000-003", 1 });

            migrationBuilder.DeleteData(
                table: "RoleMenus",
                keyColumns: new[] { "Menu_Id", "Role_Id" },
                keyValues: new object[] { "000-004", 1 });

            migrationBuilder.DeleteData(
                table: "RoleMenus",
                keyColumns: new[] { "Menu_Id", "Role_Id" },
                keyValues: new object[] { "000-005", 1 });

            migrationBuilder.DeleteData(
                table: "RoleMenus",
                keyColumns: new[] { "Menu_Id", "Role_Id" },
                keyValues: new object[] { "000-006", 1 });

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumns: new[] { "Role_Id", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);
        }
    }
}
