using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentTOPSIS.Migrations
{
    /// <inheritdoc />
    public partial class migrateCars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arabalar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Marka = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GunlukUcret = table.Column<int>(type: "int", nullable: false),
                    YakitEkonomisi = table.Column<int>(type: "int", nullable: false),
                    BagajHacmi = table.Column<int>(type: "int", nullable: false),
                    AracDonanimi = table.Column<int>(type: "int", nullable: false),
                    KmSinirlamasi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arabalar", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arabalar");
        }
    }
}
