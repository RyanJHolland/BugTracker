using TicketTracker.Models;

namespace TicketTracker.ViewModels
{
	public class ViewProjectVM
	{
		public Project Project { get; set; }
		public Ticket Ticket { get; set; }
		public DataPage<Ticket> DataPage { get; set; }

		//public string[] CategoryValues = Enum.GetNames(typeof(Ticket.CategoryEnum));

		/*
		 * public List<Ticket> Tickets { get; set; }

		public int totalTicketsInQuery { get; set; }

		/*
		 * orderBy:	category by which to sort the tickets
		 * order: ASC or DESC
		 */
		/*
		public string search { get; set; }

		public int ticketsPerPage { get; set; }
		public int page { get; set; }
		*/
	}
}
