using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedTblproduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_product",
                columns: table => new
                {
                    code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_product", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "tbl_productimage",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productcode = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    productimage = table.Column<byte[]>(type: "image", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_productimage", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_product");

            migrationBuilder.DropTable(
                name: "tbl_productimage");
        }
    }
}
