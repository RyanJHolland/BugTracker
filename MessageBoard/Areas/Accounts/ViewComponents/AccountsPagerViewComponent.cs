using BugTracker.Areas.Accounts.Models;
using BugTracker.Areas.Accounts.Models.ViewComponents;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Areas.Accounts.ViewComponents
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