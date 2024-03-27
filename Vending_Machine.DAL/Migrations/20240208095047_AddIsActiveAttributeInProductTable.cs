using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vending_Machine.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveAttributeInProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Product",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Product");
        }
    }
}
