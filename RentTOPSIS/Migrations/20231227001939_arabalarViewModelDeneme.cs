using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentTOPSIS.Migrations
{
    /// <inheritdoc />
    public partial class arabalarViewModelDeneme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArabalarViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArabaAdi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GunlukUcret = table.Column<double>(type: "float", nullable: false),
                    YakitEkonomisi = table.Column<double>(type: "float", nullable: false),
                    BagajHacmi = table.Column<double>(type: "float", nullable: false),
                    AracDonanimi = table.Column<double>(type: "float", nullable: false),
                    KmSinirlamasi = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArabalarViewModel", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArabalarViewModel");
        }
    }
}
