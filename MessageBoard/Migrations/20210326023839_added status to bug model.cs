using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracker.Migrations
{
	public partial class addedstatustobugmodel : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AlterColumn<string>(
					name: "Title",
					table: "Bug",
					type: "nvarchar(100)",
					maxLength: 100,
					nullable: false,
					oldClrType: typeof(string),
					oldType: "nvarchar(50)",
					oldMaxLength: 50);

			migrationBuilder.AddColumn<int>(
					name: "Status",
					table: "Bug",
					type: "int",
					nullable: false,
					defaultValue: 0);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
					name: "Status",
					table: "Bug");

			migrationBuilder.AlterColumn<string>(
					name: "Title",
					table: "Bug",
					type: "nvarchar(50)",
					maxLength: 50,
					nullable: false,
					oldClrType: typeof(string),
					oldType: "nvarchar(100)",
					oldMaxLength: 100);
		}
	}
}