using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TicketTracker.Areas.Accounts.Models.Users
{
	public class UpdateModel
	{
		public bool IsDirty { get; set; }

		public IdentityUser Item { get; set; }

		public List<IdentityUserClaim<string>> UserClaims { get; set; } = new List<IdentityUserClaim<string>>();

		public IdentityUserClaim<string> NewClaim { get; set; }

		public List<string> UserRoleNames { get; set; } = new List<string>();

		public bool SupportsUserClaims { get; internal set; }

		public bool SupportsUserRoles { get; internal set; }

		public List<IdentityRole> Roles { get; internal set; }

		public string ReturnUrl { get; set; }

		public string[] ClaimTypes { get; internal set; }
	}
}
