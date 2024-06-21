using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnAPI.Migrations
{
    /// <inheritdoc />
    public partial class Dbnew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_customer",
                columns: table => new
                {
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Phone = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Creditlimit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    Taxcode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_customer", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "tbl_user",
                columns: table => new
                {
                    username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    password = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    isactive = table.Column<bool>(type: "bit", nullable: true),
                    role = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    islocked = table.Column<bool>(type: "bit", nullable: true),
                    failattempt = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_user", x => x.username);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_customer");

            migrationBuilder.DropTable(
                name: "tbl_user");
        }
    }
}
