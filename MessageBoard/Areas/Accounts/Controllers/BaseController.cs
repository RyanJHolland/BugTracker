using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Areas.Accounts.Controllers
{
	[Area("Accounts")]
	[Authorize(Roles = "Administrator, Project Manager")]
	public abstract class BaseController : Controller
	{
		protected IActionResult Acknowledge()
		{
			return this.StatusCode(202);
		}

		protected IActionResult DialogClose()
		{
			return this.StatusCode(204);
		}

		protected IActionResult DialogOk(string returnUrl = null)
		{
			if (returnUrl != null) this.Response.Headers["Location"] = returnUrl;
			return this.StatusCode(205);
		}
	}
}