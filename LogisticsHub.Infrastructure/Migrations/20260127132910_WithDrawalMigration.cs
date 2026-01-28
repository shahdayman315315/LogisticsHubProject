using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogisticsHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class WithDrawalMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WithDrawalRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WalletId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminComment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletId1 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WithDrawalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WithDrawalRequests_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WithDrawalRequests_Wallets_WalletId1",
                        column: x => x.WalletId1,
                        principalTable: "Wallets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WithDrawalRequests_WalletId",
                table: "WithDrawalRequests",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_WithDrawalRequests_WalletId1",
                table: "WithDrawalRequests",
                column: "WalletId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WithDrawalRequests");
        }
    }
}
