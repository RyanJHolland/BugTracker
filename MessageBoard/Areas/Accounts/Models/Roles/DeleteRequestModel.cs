using Microsoft.AspNetCore.Identity;

namespace BugTracker.Areas.Accounts.Models.Roles
{
	public class DeleteRequestModel
	{
		public IdentityRole Item { get; set; }
	}
}