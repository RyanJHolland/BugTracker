﻿using BugTracker.Areas.Accounts.Models.ViewComponents;
using BugTracker.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BugTracker.Areas.Accounts.ViewComponents
{
	public class AccountsNavBarViewComponent : ViewComponent
	{
		#region Construction

		private readonly ApplicationDbContext context;

		public AccountsNavBarViewComponent(ApplicationDbContext context)
		{
			this.context = context;
		}

		#endregion Construction

		public IViewComponentResult Invoke()
		{
			var model = new AccountsNavBarModel();
			model.UserCount = context.Users.Count();
			model.RoleCount = context.Roles.Count();
			return View(model);
		}
	}
}