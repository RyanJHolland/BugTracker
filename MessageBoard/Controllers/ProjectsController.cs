using BugTracker.Data;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class ProjectsController : Controller
	{
		#region Constructor

		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserManager<IdentityUser> _userManager;

		public ProjectsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_context = context;
			_userManager = userManager;
		}

		#endregion Constructor

		// Displays all projects. This is where a user selects a project to browse.
		// GET: Projects
		public async Task<IActionResult> Index()
		{
			return View(await _context.Project.ToListAsync());
		}

		// GET: Projects/View/5
		// Displays the most recent bugs on the chosen project. This is where a user browses a project's bugs.
		public async Task<IActionResult> View(
			int? id,
			string orderByColumn = "CreationTime",
			string orderDirection = "DESC",
			int ticketsPerPage = 10,
			int page = 0,
			string search = ""
			)
		{
			if (id == null)
			{
				return NotFound();
			}

			var project = await _context.Project
					.FirstOrDefaultAsync(m => m.Id == id);
			if (project == null)
			{
				return NotFound();
			}

			// Validate that the sort order parameter is a valid column
			var props = typeof(Bug).GetProperties()
				.Select(prop => prop.Name)
				.ToArray();
			if (!props.Contains(orderByColumn))
			{
				orderByColumn = "CreationTime";
			}

			// sanitize orderByColumn
			if (orderDirection != "ASC")
			{
				orderDirection = "DESC";
			}
			/*
			if (!string.IsNullOrEmpty(search))
			{
				var bugs = await _context.Bug
					.Where(x => x.Title.Contains(search) || x.Body.Contains(search) || x.UserName.Contains(search));
			}
			*/

			var bugs = await _context.Bug
			.FromSqlRaw<Bug>($"SELECT * FROM Bug WHERE ParentProjectId={id} ORDER BY {orderByColumn} {orderDirection} OFFSET {page * ticketsPerPage} ROWS FETCH NEXT {ticketsPerPage} ROWS ONLY;")
			.ToListAsync();

			// AND Title LIKE '%{search}%' OR Status LIKE ...
			// TO DO: change to parameterized query to prevent sql injection thru search field

			var totalTicketsInQuery = await _context.Bug
				.Where(x => x.ParentProjectId == id)
				.CountAsync();

			ProjectBugsViewModel vm = new ProjectBugsViewModel
			{
				Project = project,
				Bugs = bugs,
				totalTicketsInQuery = totalTicketsInQuery,
				orderByColumn = orderByColumn,
				orderDirection = orderDirection,
				ticketsPerPage = ticketsPerPage,
				page = page,
				search = search
			};

			return View(vm);
		}

		// GET: Projects/Create
		[Authorize(Roles = "Administrator, Project Manager")]
		public IActionResult Create()
		{
			return View();
		}

		// POST: Projects/Create
		[Authorize(Roles = "Administrator, Project Manager")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Name,Description")] Project project)
		{
			if (ModelState.IsValid)
			{
				// add the user ID of the project creator
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				project.PMId = userId;

				_context.Add(project);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(project);
		}

		// GET: Projects/Edit/5
		[Authorize(Roles = "Administrator, Project Manager")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var project = await _context.Project.FindAsync(id);
			if (project == null)
			{
				return NotFound();
			}

			// Authorize PM to edit this project:
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != project.PMId)
				{
					return Unauthorized();
				}
			}

			// get current Project Manager's username:
			string currentPMUserName = "";
			try
			{
				currentPMUserName = _context.Users
				.FindAsync(project.PMId)
				.Result.UserName;
			}
			catch
			{
				currentPMUserName = "unassigned";
			}

			// get list of all possible alternative Project Managers' usernames:
			List<string> possiblePMUserNames = new List<string>();
			var PMs = await _userManager.GetUsersInRoleAsync("Project Manager");
			var admins = await _userManager.GetUsersInRoleAsync("Administrator");
			PMs.ToList().ForEach(u => possiblePMUserNames.Add(u.UserName));
			admins.ToList().ForEach(u => possiblePMUserNames.Add(u.UserName));
			possiblePMUserNames.Remove(currentPMUserName);

			ProjectEditViewModel vm = new ProjectEditViewModel
			{
				Project = project,
				PossiblePMUserNames = possiblePMUserNames,
				CurrentPMUserName = currentPMUserName ?? "unassigned"
			};
			return View(vm);
		}

		// POST: Projects/Edit/5
		[Authorize(Roles = "Administrator, Project Manager")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Description, CurrentPMUserName")] Project project, string CurrentPMUserName)
		{
			if (id != project.Id)
			{
				return NotFound();
			}

			try
			{
				// Get the User ID of the newly chosen PM from their username
				string PMId = (await _userManager.FindByNameAsync(CurrentPMUserName)).Id;
				if (PMId == null)
				{
					return BadRequest();
				}

				// and validate their role is at least PM
				List<string> possiblePMUserNames = new List<string>();
				var PMs = await _userManager.GetUsersInRoleAsync("Project Manager");
				var admins = await _userManager.GetUsersInRoleAsync("Administrator");
				PMs.ToList().ForEach(u => possiblePMUserNames.Add(u.UserName));
				admins.ToList().ForEach(u => possiblePMUserNames.Add(u.UserName));
				if (!possiblePMUserNames.Contains(CurrentPMUserName))
				{
					return BadRequest();
				}
				else
				{
					// save them as PM
					project.PMId = PMId;
				}
			}
			catch
			{
				return BadRequest();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(project);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ProjectExists(project.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}

			// Authorize PM to edit this project
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != project.PMId)
				{
					return Unauthorized();
				}
			}
			return View(project);
		}

		// GET: Projects/Delete/5
		[Authorize(Roles = "Administrator, Project Manager")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var project = await _context.Project
					.FirstOrDefaultAsync(m => m.Id == id);
			if (project == null)
			{
				return NotFound();
			}

			// Authorize PM to edit this project
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != project.PMId)
				{
					return Unauthorized();
				}
			}

			return View(project);
		}

		// POST: Projects/Delete/5
		[Authorize(Roles = "Administrator, Project Manager")]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var project = await _context.Project.FindAsync(id);

			// Authorize PM to edit this project
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != project.PMId)
				{
					return Unauthorized();
				}
			}

			_context.Project.Remove(project);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProjectExists(int id)
		{
			return _context.Project.Any(e => e.Id == id);
		}
	}
}