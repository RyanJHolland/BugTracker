using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace BugTracker.Areas.Accounts
{
	public static class Extensions
	{
		public static T[] Page<T>(this IQueryable<T> query, int page, int pageSize)
		{
			return query.Skip((page - 1) * pageSize).Take(pageSize).ToArray();
		}

		public static bool IsLockedout(this IdentityUser user)
		{
			return (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow);
		}
	}
}