using TicketTracker.Models;
using System.Collections.Generic;

namespace TicketTracker.ViewModels
{
	public class ViewTicketVM
	{
		public Project Project { get; set; }
		public Ticket Ticket { get; set; }
		public List<Comment> Comments { get; set; }
	}
}