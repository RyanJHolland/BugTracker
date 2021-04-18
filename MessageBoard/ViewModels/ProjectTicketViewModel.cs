using TicketTracker.Models;

namespace TicketTracker.ViewModels
{
	public class ProjectTicketViewModel
	{
		public Project Project { get; set; }
		public Ticket Ticket { get; set; }
		public string OwnerId { get; set; }
		public string OwnerUserName { get; set; }

		//		m => m.Ticket.Priority, new SelectList(Enum.GetValues(typeof(Ticket.PriorityEnum)))
	}
}
