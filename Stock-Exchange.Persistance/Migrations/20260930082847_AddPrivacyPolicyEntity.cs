using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stock_Exchange.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class AddPrivacyPolicyEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PrivacyPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DescriptionEn = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
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
                    table.PrimaryKey("PK_PrivacyPolicies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrivacyPolicySections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrivacyPolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TitleEn = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    TitleAr = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ContentEn = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
                    ContentAr = table.Column<string>(type: "nvarchar(max)", maxLength: 8000, nullable: false),
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
                    table.PrimaryKey("PK_PrivacyPolicySections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivacyPolicySections_PrivacyPolicies_PrivacyPolicyId",
                        column: x => x.PrivacyPolicyId,
                        principalTable: "PrivacyPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivacyPolicies_IsDeleted",
                table: "PrivacyPolicies",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PrivacyPolicySections_IsDeleted",
                table: "PrivacyPolicySections",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_PrivacyPolicySections_PrivacyPolicyId",
                table: "PrivacyPolicySections",
                column: "PrivacyPolicyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivacyPolicySections");

            migrationBuilder.DropTable(
                name: "PrivacyPolicies");
        }
    }
}
