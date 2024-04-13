using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuctionService.Migrations
{
    /// <inheritdoc />
    public partial class changestatuscolumnname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "Auctions",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Auctions",
                newName: "status");
        }
    }
}
