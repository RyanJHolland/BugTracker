using TicketTracker.Models;

namespace TicketTracker.ViewModels
{
	public class ProjectTicketCommentViewModel
	{
		public Project Project { get; set; }
		public Ticket Ticket { get; set; }
		public Comment Comment { get; set; }
	}
}