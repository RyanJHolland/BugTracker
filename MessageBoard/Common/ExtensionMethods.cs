using System.Security.Claims;

// source: https://www.dxsdata.com/2017/03/asp-net-core-get-user-id/
namespace BugTracker.Common
{
	public static class ExtensionMethods
	{
		/// <summary>
		/// User ID
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public static string GetUserId(this ClaimsPrincipal user)
		{
			if (!user.Identity.IsAuthenticated)
				return null;

			ClaimsPrincipal currentUser = user;
			return currentUser.FindFirst(ClaimTypes.NameIdentifier).Value;
		}
	}
}