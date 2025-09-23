using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeFinder.Api.Migrations
{
    /// <inheritdoc />
    public partial class UniqueListingTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Listings_Title",
                table: "Listings",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Listings_Title",
                table: "Listings");
        }
    }
}
