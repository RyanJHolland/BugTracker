using Microsoft.AspNetCore.Identity;
using TicketTracker.Models;

namespace TicketTracker.Areas.Accounts.Models.Roles
{
	public class IndexModel
	{
		public DataPage<IdentityRole> DataPage { get; set; }
	}
}
