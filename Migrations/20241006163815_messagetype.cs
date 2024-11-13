using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poochatting.Migrations
{
    /// <inheritdoc />
    public partial class messagetype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MessageTypeEnum",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageTypeEnum",
                table: "Messages");
        }
    }
}
