using Microsoft.AspNetCore.Identity;

namespace TicketTracker.Areas.Accounts.Models.Users
{
	public class IndexModel
	{
		public DataPage<IdentityUser> DataPage { get; set; }
	}
}