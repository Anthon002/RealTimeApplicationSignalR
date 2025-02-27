using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealTimeApplication.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _270220250954 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DmToken",
                table: "FriendRequests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DmToken",
                table: "FriendRequests");
        }
    }
}
