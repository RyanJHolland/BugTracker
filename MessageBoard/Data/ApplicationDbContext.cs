﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Data
{
	public class ApplicationDbContext : IdentityDbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
				: base(options)
		{
		}

		public DbSet<BugTracker.Models.Project> Project { get; set; }
		public DbSet<BugTracker.Models.Bug> Bug { get; set; }
		public DbSet<BugTracker.Models.Comment> Comment { get; set; }
	}
}