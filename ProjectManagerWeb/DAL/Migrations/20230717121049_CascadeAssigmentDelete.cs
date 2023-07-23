using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class CascadeAssigmentDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assigments_Projects_CurrentProjectId",
                table: "Assigments");

            migrationBuilder.AddForeignKey(
                name: "FK_Assigments_Projects_CurrentProjectId",
                table: "Assigments",
                column: "CurrentProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assigments_Projects_CurrentProjectId",
                table: "Assigments");

            migrationBuilder.AddForeignKey(
                name: "FK_Assigments_Projects_CurrentProjectId",
                table: "Assigments",
                column: "CurrentProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
