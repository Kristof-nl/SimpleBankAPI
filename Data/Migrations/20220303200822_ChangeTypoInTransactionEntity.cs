using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class ChangeTypoInTransactionEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AmmountBefore",
                table: "Transactions",
                newName: "TransactionAmount");

            migrationBuilder.RenameColumn(
                name: "Ammount",
                table: "Transactions",
                newName: "AmountBefore");

            migrationBuilder.RenameColumn(
                name: "AammountAfter",
                table: "Transactions",
                newName: "AmountAfter");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionAmount",
                table: "Transactions",
                newName: "AmmountBefore");

            migrationBuilder.RenameColumn(
                name: "AmountBefore",
                table: "Transactions",
                newName: "Ammount");

            migrationBuilder.RenameColumn(
                name: "AmountAfter",
                table: "Transactions",
                newName: "AammountAfter");
        }
    }
}
