using TicketTracker.Areas.Accounts.Models;
using TicketTracker.Models;

namespace TicketTracker.ViewModels
{
	public class ViewProjectVM
	{
		public DataPage<Ticket> DataPage { get; set; }
	}
}
