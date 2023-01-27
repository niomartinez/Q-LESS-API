using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLESS.Migrations
{
    /// <inheritdoc />
    public partial class TopUpAttr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountToLoad",
                table: "Cards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CashValue",
                table: "Cards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Change",
                table: "Cards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NewBalance",
                table: "Cards",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountToLoad",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CashValue",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Change",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "NewBalance",
                table: "Cards");
        }
    }
}
