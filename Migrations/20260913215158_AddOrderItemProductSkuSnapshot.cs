using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderManagementApi.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemProductSkuSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Add ProductSku temporarily as nullable.
            migrationBuilder.AddColumn<string>(
                name: "ProductSku",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            // 2. Backfill existing order items from the current Product SKU.
            migrationBuilder.Sql("""
        UPDATE oi
        SET oi.ProductSku = p.Sku
        FROM OrderItems oi
        INNER JOIN Products p
            ON oi.ProductId = p.Id;
        """);

            // 3. Make ProductSku required after every existing row has a value.
            migrationBuilder.AlterColumn<string>(
                name: "ProductSku",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductSku",
                table: "OrderItems");
        }
    }
}
