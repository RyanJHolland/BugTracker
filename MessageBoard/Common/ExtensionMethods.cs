using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;

namespace TicketTracker.Common
{
	public static class ExtensionMethods
	{
		// gets User Id...
		// source: https://www.dxsdata.com/2017/03/asp-net-core-get-user-id/
		public static string GetUserId(this ClaimsPrincipal user)
		{
			if (!user.Identity.IsAuthenticated)
				return null;

			ClaimsPrincipal currentUser = user;
			return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
		}

		// Used for pagination in DB queries.
		public static T[] Page<T>(this IQueryable<T> query, int page, int pageSize)
		{
			return query.Skip((page - 1) * pageSize).Take(pageSize).ToArray();
		}

		// Used for account management. Checks if user is locked out from logging in because of failed attempts.
		public static bool IsLockedout(this IdentityUser user)
		{
			return user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow;
		}
	}
}