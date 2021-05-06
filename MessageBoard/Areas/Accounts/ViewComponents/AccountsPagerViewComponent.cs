using Microsoft.AspNetCore.Mvc;
using TicketTracker.Areas.Accounts.Models.ViewComponents;
using TicketTracker.Models;

namespace TicketTracker.Areas.Accounts.ViewComponents
{
	public class AccountsPagerViewComponent : ViewComponent
	{
		#region Construction

		public AccountsPagerViewComponent()
		{ }

		#endregion Construction

		public IViewComponentResult Invoke(DataPage page)
		{
			var model = new AccountsPagerModel();
			model.DataPage = page;

			return View(model);
		}
	}
}
