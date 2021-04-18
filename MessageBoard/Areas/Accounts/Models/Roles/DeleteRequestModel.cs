using Microsoft.AspNetCore.Identity;

namespace TicketTracker.Areas.Accounts.Models.Roles
{
	public class DeleteRequestModel
	{
		public IdentityRole Item { get; set; }
	}
}