using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticsHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditWithdrawalrequestmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WithDrawalRequests_Wallets_WalletId1",
                table: "WithDrawalRequests");

            migrationBuilder.DropIndex(
                name: "IX_WithDrawalRequests_WalletId1",
                table: "WithDrawalRequests");

            migrationBuilder.DropColumn(
                name: "WalletId1",
                table: "WithDrawalRequests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WalletId1",
                table: "WithDrawalRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WithDrawalRequests_WalletId1",
                table: "WithDrawalRequests",
                column: "WalletId1");

            migrationBuilder.AddForeignKey(
                name: "FK_WithDrawalRequests_Wallets_WalletId1",
                table: "WithDrawalRequests",
                column: "WalletId1",
                principalTable: "Wallets",
                principalColumn: "Id");
        }
    }
}
