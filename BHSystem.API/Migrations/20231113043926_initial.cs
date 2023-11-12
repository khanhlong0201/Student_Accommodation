using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BHSystem.API.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Citys",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Citys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    MenuId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Icon = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Link = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Parent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.MenuId);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Distincts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    City_Id = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distincts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Distincts_Citys_City_Id",
                        column: x => x.City_Id,
                        principalTable: "Citys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImagesDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Image_Id = table.Column<int>(type: "int", nullable: false),
                    File_Path = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesDetails", x => new { x.Id, x.Image_Id });
                    table.ForeignKey(
                        name: "FK_ImagesDetails_Images_Image_Id",
                        column: x => x.Image_Id,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleMenus",
                columns: table => new
                {
                    Role_Id = table.Column<int>(type: "int", nullable: false),
                    Menu_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenus", x => new { x.Role_Id, x.Menu_Id });
                    table.ForeignKey(
                        name: "FK_RoleMenus_Menus_Menu_Id",
                        column: x => x.Menu_Id,
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleMenus_Roles_Role_Id",
                        column: x => x.Role_Id,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Distincts_Id = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wards_Distincts_Distincts_Id",
                        column: x => x.Distincts_Id,
                        principalTable: "Distincts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PasswordReset = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Ward_Id = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Wards_Ward_Id",
                        column: x => x.Ward_Id,
                        principalTable: "Wards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardingHouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Ward_Id = table.Column<int>(type: "int", nullable: false),
                    Adddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Image_Id = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardingHouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardingHouses_Images_Image_Id",
                        column: x => x.Image_Id,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BoardingHouses_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BoardingHouses_Wards_Ward_Id",
                        column: x => x.Ward_Id,
                        principalTable: "Wards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Role_Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.Role_Id });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_Role_Id",
                        column: x => x.Role_Id,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Describe = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Image_Id = table.Column<int>(type: "int", nullable: false),
                    BoardingHouse_Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_BoardingHouses_BoardingHouse_Id",
                        column: x => x.BoardingHouse_Id,
                        principalTable: "BoardingHouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Images_Image_Id",
                        column: x => x.Image_Id,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Boarding_House_Id = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Length = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Width = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Image_Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_BoardingHouses_Boarding_House_Id",
                        column: x => x.Boarding_House_Id,
                        principalTable: "BoardingHouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Rooms_Images_Image_Id",
                        column: x => x.Image_Id,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    Room_Id = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Rooms_Room_Id",
                        column: x => x.Room_Id,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoomPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Room_Id = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Date_Create = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Create = table.Column<int>(type: "int", nullable: true),
                    Date_Update = table.Column<DateTime>(type: "datetime2", nullable: true),
                    User_Update = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomPrices", x => new { x.Id, x.Room_Id });
                    table.ForeignKey(
                        name: "FK_RoomPrices_Rooms_Room_Id",
                        column: x => x.Room_Id,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BoardingHouses_Image_Id",
                table: "BoardingHouses",
                column: "Image_Id");

            migrationBuilder.CreateIndex(
                name: "IX_BoardingHouses_User_Id",
                table: "BoardingHouses",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_BoardingHouses_Ward_Id",
                table: "BoardingHouses",
                column: "Ward_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_Room_Id",
                table: "Bookings",
                column: "Room_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_BoardingHouse_Id",
                table: "Comments",
                column: "BoardingHouse_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_Image_Id",
                table: "Comments",
                column: "Image_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Distincts_City_Id",
                table: "Distincts",
                column: "City_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ImagesDetails_Image_Id",
                table: "ImagesDetails",
                column: "Image_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenus_Menu_Id",
                table: "RoleMenus",
                column: "Menu_Id");

            migrationBuilder.CreateIndex(
                name: "IX_RoomPrices_Room_Id",
                table: "RoomPrices",
                column: "Room_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Boarding_House_Id",
                table: "Rooms",
                column: "Boarding_House_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Image_Id",
                table: "Rooms",
                column: "Image_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_Role_Id",
                table: "UserRoles",
                column: "Role_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Ward_Id",
                table: "Users",
                column: "Ward_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_Distincts_Id",
                table: "Wards",
                column: "Distincts_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "ImagesDetails");

            migrationBuilder.DropTable(
                name: "RoleMenus");

            migrationBuilder.DropTable(
                name: "RoomPrices");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "BoardingHouses");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "Distincts");

            migrationBuilder.DropTable(
                name: "Citys");
        }
    }
}
