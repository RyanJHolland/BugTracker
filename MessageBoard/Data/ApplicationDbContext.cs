using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TicketTracker.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
				: base(options)
		{
		}

		public DbSet<TicketTracker.Models.Project> Project { get; set; }
		public DbSet<TicketTracker.Models.Ticket> Ticket { get; set; }
		public DbSet<TicketTracker.Models.Comment> Comment { get; set; }
	}
}
