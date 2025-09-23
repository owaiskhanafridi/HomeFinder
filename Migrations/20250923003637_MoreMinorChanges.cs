using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeFinder.Api.Migrations
{
    /// <inheritdoc />
    public partial class MoreMinorChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UploadAt",
                table: "Photos",
                newName: "UploadedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UploadedAt",
                table: "Photos",
                newName: "UploadAt");
        }
    }
}
