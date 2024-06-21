using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnAPI.Migrations
{
    /// <inheritdoc />
    public partial class TBLRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_refreshtoken",
                columns: table => new
                {
                    userid = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    tokenid = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    refreshtoken = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_refreshtoken", x => x.userid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_refreshtoken");
        }
    }
}
