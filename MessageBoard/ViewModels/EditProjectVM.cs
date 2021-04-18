using System.Collections.Generic;
using TicketTracker.Models;

namespace TicketTracker.ViewModels
{
	public class EditProjectVM
	{
		public Project Project { get; set; }
		public List<string> PossibleProjectOwnerUserNames { get; set; }

		//		m => m.Ticket.Priority, new SelectList(Enum.GetValues(typeof(Ticket.PriorityEnum)))
	}
}
