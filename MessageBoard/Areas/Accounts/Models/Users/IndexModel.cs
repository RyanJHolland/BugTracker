using Microsoft.AspNetCore.Identity;

namespace BugTracker.Areas.Accounts.Models.Users
{
	public class IndexModel
	{
		public DataPage<IdentityUser> DataPage { get; set; }
	}
}