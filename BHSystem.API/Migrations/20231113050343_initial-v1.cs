using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BHSystem.API.Migrations
{
    public partial class initialv1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "MenuId", "Date_Create", "Date_Update", "Icon", "IsDeleted", "Level", "Link", "Name", "Parent", "User_Create", "User_Update" },
                values: new object[,]
                {
                    { "000-001", null, null, "fa fa-chart-bar", false, 0, "/admin/report", "Biểu đồ thống kê", "", null, null },
                    { "000-002", null, null, "fa fa-chart-bar", false, 0, "/admin/boarding-house", "Quản lý phòng trọ", "", null, null },
                    { "000-003", null, null, "fa fa-chart-bar", false, 0, "/admin/approve-room", "Phê duyệt phòng", "", null, null },
                    { "000-004", null, null, "fa fa-chart-bar", false, 0, "/admin/approve-booking", "Xác nhận đặt phòng", "", null, null },
                    { "000-005", null, null, "fa fa-chart-bar", false, 0, "/admin/user", "Quản lý người dùng", "", null, null },
                    { "000-006", null, null, "fa-solid fa-folder-tree", false, 0, "/admin/role", "Quản lý nhóm quyền", "", null, null }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-001");

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-002");

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-003");

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-004");

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-005");

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "MenuId",
                keyValue: "000-006");
        }
    }
}
