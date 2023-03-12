using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceAppointmentSystem.Migrations
{
    public partial class DatabaseModification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Services_ServiceID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserID",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_Services_ServiceID",
                table: "ShoppingCart");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_Users_UserID",
                table: "ShoppingCart");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "ShoppingCart");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "ShoppingCart",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "ServiceID",
                table: "ShoppingCart",
                newName: "ServiceId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "ShoppingCart",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCart_UserID",
                table: "ShoppingCart",
                newName: "IX_ShoppingCart_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCart_ServiceID",
                table: "ShoppingCart",
                newName: "IX_ShoppingCart_ServiceId");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Services",
                newName: "BasePrice");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Orders",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "SessionID",
                table: "Orders",
                newName: "SessionId");

            migrationBuilder.RenameColumn(
                name: "PaymentIntentID",
                table: "Orders",
                newName: "PaymentIntentId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Orders",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_UserID",
                table: "Orders",
                newName: "IX_Orders_UserId");

            migrationBuilder.RenameColumn(
                name: "ServiceID",
                table: "OrderDetails",
                newName: "ServiceId");

            migrationBuilder.RenameColumn(
                name: "OrderID",
                table: "OrderDetails",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "OrderDetails",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ServiceID",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ServiceId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_OrderID",
                table: "OrderDetails",
                newName: "IX_OrderDetails_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Services_ServiceId",
                table: "OrderDetails",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_Services_ServiceId",
                table: "ShoppingCart",
                column: "ServiceId",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_Users_UserId",
                table: "ShoppingCart",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Services_ServiceId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_Services_ServiceId",
                table: "ShoppingCart");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingCart_Users_UserId",
                table: "ShoppingCart");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ShoppingCart",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "ShoppingCart",
                newName: "ServiceID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ShoppingCart",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCart_UserId",
                table: "ShoppingCart",
                newName: "IX_ShoppingCart_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_ShoppingCart_ServiceId",
                table: "ShoppingCart",
                newName: "IX_ShoppingCart_ServiceID");

            migrationBuilder.RenameColumn(
                name: "BasePrice",
                table: "Services",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Orders",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "Orders",
                newName: "SessionID");

            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "Orders",
                newName: "PaymentIntentID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Orders",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                newName: "IX_Orders_UserID");

            migrationBuilder.RenameColumn(
                name: "ServiceId",
                table: "OrderDetails",
                newName: "ServiceID");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "OrderDetails",
                newName: "OrderID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderDetails",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_ServiceId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_ServiceID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                newName: "IX_OrderDetails_OrderID");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "ShoppingCart",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderID",
                table: "OrderDetails",
                column: "OrderID",
                principalTable: "Orders",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Services_ServiceID",
                table: "OrderDetails",
                column: "ServiceID",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserID",
                table: "Orders",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_Services_ServiceID",
                table: "ShoppingCart",
                column: "ServiceID",
                principalTable: "Services",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingCart_Users_UserID",
                table: "ShoppingCart",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
