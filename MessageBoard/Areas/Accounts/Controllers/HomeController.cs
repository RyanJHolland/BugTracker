using TicketTracker.Data;
using Microsoft.AspNetCore.Mvc;

namespace TicketTracker.Areas.Accounts.Controllers
{
	public class HomeController : BaseController
	{
		#region Construction

		private readonly ApplicationDbContext context;

		public HomeController(ApplicationDbContext context)
		{
			this.context = context;
		}

		#endregion Construction

		public IActionResult Index()
		{
			return View();
		}
	}
}