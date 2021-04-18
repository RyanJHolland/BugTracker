using TicketTracker.Models;
using System.Collections.Generic;

namespace TicketTracker.ViewModels
{
	public class ProjectEditViewModel
	{
		public Project Project { get; set; }
		public List<string> PossibleProjectOwnerUserNames { get; set; }

		//		m => m.Ticket.Priority, new SelectList(Enum.GetValues(typeof(Ticket.PriorityEnum)))
	}
}
