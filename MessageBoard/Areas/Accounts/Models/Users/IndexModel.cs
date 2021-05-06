using Microsoft.AspNetCore.Identity;
using TicketTracker.Models;

namespace TicketTracker.Areas.Accounts.Models.Users
{
	public class IndexModel
	{
		public DataPage<IdentityUser> DataPage { get; set; }
	}
}
