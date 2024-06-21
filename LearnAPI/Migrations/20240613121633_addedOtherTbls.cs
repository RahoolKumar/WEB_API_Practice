using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedOtherTbls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_menu",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_menu", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "tbl_otpManager",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    otptext = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false),
                    otptype = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    expiration = table.Column<DateTime>(type: "datetime", nullable: false),
                    createddate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_otpManager", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_pwdManger",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_pwdManger", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_role",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_role", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "tbl_rolepermission",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userrole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    menucode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    haveview = table.Column<bool>(type: "bit", nullable: false),
                    haveadd = table.Column<bool>(type: "bit", nullable: false),
                    haveedit = table.Column<bool>(type: "bit", nullable: false),
                    havedelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_rolepermission", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_tempuser",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tbl_tempuser1", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_menu");

            migrationBuilder.DropTable(
                name: "tbl_otpManager");

            migrationBuilder.DropTable(
                name: "tbl_pwdManger");

            migrationBuilder.DropTable(
                name: "tbl_role");

            migrationBuilder.DropTable(
                name: "tbl_rolepermission");

            migrationBuilder.DropTable(
                name: "tbl_tempuser");
        }
    }
}
