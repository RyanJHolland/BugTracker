using Microsoft.EntityFrameworkCore.Migrations;

namespace TicketTracker.Migrations
{
    public partial class changedTagnametoBugTypeinBug : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tag",
                table: "Bug",
                newName: "BugType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BugType",
                table: "Bug",
                newName: "Tag");
        }
    }
}
