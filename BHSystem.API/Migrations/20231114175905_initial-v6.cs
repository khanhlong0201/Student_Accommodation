using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BHSystem.API.Migrations
{
    public partial class initialv6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "RoleMenus",
            //    keyColumns: new[] { "Menu_Id", "Role_Id" },
            //    keyValues: new object[] { "000-001", 1 });

            //migrationBuilder.DeleteData(
            //    table: "RoleMenus",
            //    keyColumns: new[] { "Menu_Id", "Role_Id" },
            //    keyValues: new object[] { "000-002", 1 });

            //migrationBuilder.DeleteData(
            //    table: "RoleMenus",
            //    keyColumns: new[] { "Menu_Id", "Role_Id" },
            //    keyValues: new object[] { "000-003", 1 });

            //migrationBuilder.DeleteData(
            //    table: "RoleMenus",
            //    keyColumns: new[] { "Menu_Id", "Role_Id" },
            //    keyValues: new object[] { "000-004", 1 });

            //migrationBuilder.DeleteData(
            //    table: "RoleMenus",
            //    keyColumns: new[] { "Menu_Id", "Role_Id" },
            //    keyValues: new object[] { "000-005", 1 });

            //migrationBuilder.DeleteData(
            //    table: "RoleMenus",
            //    keyColumns: new[] { "Menu_Id", "Role_Id" },
            //    keyValues: new object[] { "000-006", 1 });

            //migrationBuilder.DeleteData(
            //    table: "Roles",
            //    keyColumn: "Id",
            //    keyValue: 2);

            //migrationBuilder.DeleteData(
            //    table: "UserRoles",
            //    keyColumns: new[] { "Role_Id", "UserId" },
            //    keyValues: new object[] { 1, 2 });

            //migrationBuilder.DeleteData(
            //    table: "Users",
            //    keyColumn: "UserId",
            //    keyValue: 1);

            //migrationBuilder.DeleteData(
            //    table: "Menus",
            //    keyColumn: "MenuId",
            //    keyValue: "000-001");

            //migrationBuilder.DeleteData(
            //    table: "Menus",
            //    keyColumn: "MenuId",
            //    keyValue: "000-002");

            //migrationBuilder.DeleteData(
            //    table: "Menus",
            //    keyColumn: "MenuId",
            //    keyValue: "000-003");

            //migrationBuilder.DeleteData(
            //    table: "Menus",
            //    keyColumn: "MenuId",
            //    keyValue: "000-004");

            //migrationBuilder.DeleteData(
            //    table: "Menus",
            //    keyColumn: "MenuId",
            //    keyValue: "000-005");

            //migrationBuilder.DeleteData(
            //    table: "Menus",
            //    keyColumn: "MenuId",
            //    keyValue: "000-006");

            //migrationBuilder.DeleteData(
            //    table: "Roles",
            //    keyColumn: "Id",
            //    keyValue: 1);

            //migrationBuilder.DeleteData(
            //    table: "Users",
            //    keyColumn: "UserId",
            //    keyValue: 2);

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(2500)", maxLength: 2500, nullable: true),
                    JText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserMessages",
                columns: table => new
                {
                    Message_Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsReaded = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessages", x => new { x.Message_Id, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserMessages_Messages_Message_Id",
                        column: x => x.Message_Id,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_UserId",
                table: "UserMessages",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMessages");

            migrationBuilder.DropTable(
                name: "Messages");

            //migrationBuilder.InsertData(
            //    table: "Menus",
            //    columns: new[] { "MenuId", "Date_Create", "Date_Update", "Icon", "IsDeleted", "Level", "Link", "Name", "Parent", "User_Create", "User_Update" },
            //    values: new object[,]
            //    {
            //        { "000-001", null, null, "fa fa-chart-bar", false, 0, "/admin/report", "Biểu đồ thống kê", "", null, null },
            //        { "000-002", null, null, "fa-solid fa-people-roof", false, 0, "/admin/boarding-house", "Quản lý phòng trọ", "", null, null },
            //        { "000-003", null, null, "fa-solid fa-user-check", false, 0, "/admin/approve-room", "Phê duyệt phòng", "", null, null },
            //        { "000-004", null, null, "fa-solid fa-user-pen", false, 0, "/admin/approve-booking", "Xác nhận đặt phòng", "", null, null },
            //        { "000-005", null, null, "fa-solid fa-users", false, 0, "/admin/user", "Quản lý người dùng", "", null, null },
            //        { "000-006", null, null, "fa-solid fa-folder-tree", false, 0, "/admin/role", "Quản lý nhóm quyền", "", null, null }
            //    });

            //migrationBuilder.InsertData(
            //    table: "Roles",
            //    columns: new[] { "Id", "Date_Create", "Date_Update", "IsDeleted", "Name", "User_Create", "User_Update" },
            //    values: new object[,]
            //    {
            //        { 1, null, null, false, "Admintrator", null, null },
            //        { 2, null, null, false, "Bussiness Partner", null, null }
            //    });

            //migrationBuilder.InsertData(
            //    table: "Users",
            //    columns: new[] { "UserId", "Address", "Date_Create", "Date_Update", "Email", "FullName", "IsDeleted", "Password", "PasswordReset", "Phone", "Type", "UserName", "User_Create", "User_Update", "Ward_Id" },
            //    values: new object[,]
            //    {
            //        { 1, null, null, null, null, "Khách Vãn Lai", false, "", null, null, "Client", "", null, null, 1 },
            //        { 2, null, null, null, null, "Testing", false, "KZY9mwl2Mv4NM4jrKXv4ug==", null, null, "Admin", "testing", null, null, 1 }
            //    });

            //migrationBuilder.InsertData(
            //    table: "RoleMenus",
            //    columns: new[] { "Menu_Id", "Role_Id", "Date_Create", "Date_Update", "IsDeleted", "User_Create", "User_Update" },
            //    values: new object[,]
            //    {
            //        { "000-001", 1, null, null, false, null, null },
            //        { "000-002", 1, null, null, false, null, null },
            //        { "000-003", 1, null, null, false, null, null },
            //        { "000-004", 1, null, null, false, null, null },
            //        { "000-005", 1, null, null, false, null, null },
            //        { "000-006", 1, null, null, false, null, null }
            //    });

            //migrationBuilder.InsertData(
            //    table: "UserRoles",
            //    columns: new[] { "Role_Id", "UserId", "Date_Create", "Date_Update", "IsDeleted", "User_Create", "User_Update" },
            //    values: new object[] { 1, 2, null, null, false, null, null });
        }
    }
}
