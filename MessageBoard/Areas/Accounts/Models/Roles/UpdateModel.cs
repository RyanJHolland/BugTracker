using Microsoft.AspNetCore.Identity;

namespace BugTracker.Areas.Accounts.Models.Roles
{
	public class UpdateModel
	{
		public IdentityRole Item { get; set; }

		public string ReturnUrl { get; set; }
	}
}