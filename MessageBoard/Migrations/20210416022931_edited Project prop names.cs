using Microsoft.EntityFrameworkCore.Migrations;

namespace TicketTracker.Migrations
{
    public partial class editedProjectpropnames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PMId",
                table: "Project",
                newName: "ProjectOwnerUserName");

            migrationBuilder.AddColumn<string>(
                name: "ProjectOwnerId",
                table: "Project",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectOwnerId",
                table: "Project");

            migrationBuilder.RenameColumn(
                name: "ProjectOwnerUserName",
                table: "Project",
                newName: "PMId");
        }
    }
}
