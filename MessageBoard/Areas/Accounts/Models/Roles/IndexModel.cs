using Microsoft.AspNetCore.Identity;

namespace TicketTracker.Areas.Accounts.Models.Roles
{
	public class IndexModel
	{
		public DataPage<IdentityRole> DataPage { get; set; }
	}
}