using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Poochatting.Migrations
{
    /// <inheritdoc />
    public partial class editedmessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WasEdited",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasEdited",
                table: "Messages");
        }
    }
}
