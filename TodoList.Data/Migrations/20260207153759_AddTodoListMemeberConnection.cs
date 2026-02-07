using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TodoListApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTodoListMemeberConnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoListMembers_TodoListId",
                table: "TodoListMembers");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "TodoListMembers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_TodoListMembers_TodoListId_UserId",
                table: "TodoListMembers",
                columns: new[] { "TodoListId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TodoListMembers_TodoListId_UserId",
                table: "TodoListMembers");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "TodoListMembers",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TodoListMembers_TodoListId",
                table: "TodoListMembers",
                column: "TodoListId");
        }
    }
}
