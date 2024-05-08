using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataCollector.Migrations
{
    /// <inheritdoc />
    public partial class linkStoreWithApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Stores",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_CreatedBy",
                table: "Stores",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_AspNetUsers_CreatedBy",
                table: "Stores",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stores_AspNetUsers_CreatedBy",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_CreatedBy",
                table: "Stores");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
