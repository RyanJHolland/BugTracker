using System.Collections.Generic;
using TicketTracker.Models;

namespace TicketTracker.ViewModels
{
	public class oldViewProjectVM
	{
		public Project Project { get; set; }
		public List<Ticket> Tickets { get; set; }

		public int totalTicketsInQuery { get; set; }

		/*
		 * orderBy:	category by which to sort the tickets
		 * order: ASC or DESC
		 */
		public string search { get; set; }
		public string orderByColumn { get; set; }
		public string orderDirection { get; set; }
		public int ticketsPerPage { get; set; }
		public int page { get; set; }
	}
}
