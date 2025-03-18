using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class uprdate008 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductUser_Products_FavouritesProductProductId",
                table: "ProductUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductUser_Users_FavouritesProductId",
                table: "ProductUser");

            migrationBuilder.RenameColumn(
                name: "FavouritesProductProductId",
                table: "ProductUser",
                newName: "UsersId");

            migrationBuilder.RenameColumn(
                name: "FavouritesProductId",
                table: "ProductUser",
                newName: "ProductsProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductUser_FavouritesProductProductId",
                table: "ProductUser",
                newName: "IX_ProductUser_UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductUser_Products_ProductsProductId",
                table: "ProductUser",
                column: "ProductsProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductUser_Users_UsersId",
                table: "ProductUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductUser_Products_ProductsProductId",
                table: "ProductUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductUser_Users_UsersId",
                table: "ProductUser");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "ProductUser",
                newName: "FavouritesProductProductId");

            migrationBuilder.RenameColumn(
                name: "ProductsProductId",
                table: "ProductUser",
                newName: "FavouritesProductId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductUser_UsersId",
                table: "ProductUser",
                newName: "IX_ProductUser_FavouritesProductProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductUser_Products_FavouritesProductProductId",
                table: "ProductUser",
                column: "FavouritesProductProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductUser_Users_FavouritesProductId",
                table: "ProductUser",
                column: "FavouritesProductId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
