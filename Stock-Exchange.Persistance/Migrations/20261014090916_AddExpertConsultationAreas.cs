using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock_Exchange.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddExpertConsultationAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpertConsultationAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpertId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TextAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Version = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpertConsultationAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpertConsultationAreas_Experts_ExpertId",
                        column: x => x.ExpertId,
                        principalTable: "Experts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpertConsultationAreas_ExpertId",
                table: "ExpertConsultationAreas",
                column: "ExpertId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpertConsultationAreas_IsDeleted",
                table: "ExpertConsultationAreas",
                column: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpertConsultationAreas");
        }
    }
}
