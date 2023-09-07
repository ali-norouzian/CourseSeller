using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseSeller.DataLayer.Migrations
{
    public partial class DiscountTwiceUseInOrderBugHandled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UsedDiscount",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedDiscount",
                table: "Orders");
        }
    }
}
