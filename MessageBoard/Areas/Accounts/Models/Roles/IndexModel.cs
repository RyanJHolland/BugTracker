using Microsoft.AspNetCore.Identity;

namespace BugTracker.Areas.Accounts.Models.Roles
{
	public class IndexModel
	{
		public DataPage<IdentityRole> DataPage { get; set; }
	}
}