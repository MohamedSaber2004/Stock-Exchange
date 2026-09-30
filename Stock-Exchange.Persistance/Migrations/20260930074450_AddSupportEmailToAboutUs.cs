using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock_Exchange.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportEmailToAboutUs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupportEmail",
                table: "AboutUs",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupportEmail",
                table: "AboutUs");
        }
    }
}
