using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketTracker.Data;
using TicketTracker.Models;
using TicketTracker.ViewModels;

namespace TicketTracker.Controllers
{
	public class ProjectsController : Controller
	{
		#region Constructor

		private readonly IConfiguration _config;
		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserManager<IdentityUser> _userManager;

		public ProjectsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager, IConfiguration config)
		{
			_httpContextAccessor = httpContextAccessor;
			_context = context;
			_userManager = userManager;
			_config = config;
		}

		#endregion Constructor

		// Displays all projects. This is where a user selects a project to browse.
		// GET: Projects
		public async Task<IActionResult> Index()
		{
			return View(await _context.Project.ToListAsync());
		}

		// GET: Projects/View/5
		// Displays the most recent tickets on the chosen project. This is where a user browses a project's tickets.
		public async Task<IActionResult> View(
			int? id,
			int currentPage = 1,
			int pageSize = 10,
			string filter = null,
			string orderDirection = "DESC",
			string orderByColumn = "CreationTime"
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

			// Start to build a query
			var query = _context.Ticket
				.Where(t => t.ParentProjectId == id)
				.AsQueryable();

			// Get count of all tickets in project
			var fullCount = await query.CountAsync();

			// Add search filter to query
			if (!String.IsNullOrWhiteSpace(filter))
			{
				query = query
				.Where(t => t.Body.Contains(filter)
					|| t.Title.Contains(filter)
					|| t.UserName.Contains(filter));
			}

			// Get # of tickets that match the filter
			var filteredCount = await query.CountAsync();

			// Add sort order and direction to query
			switch (orderByColumn)
			{
				case "Title":
					query = orderDirection == "DESC"
						? query.OrderByDescending(t => t.Title)
						: query.OrderBy(t => t.Title);
					break;

				case "Category":
					query = orderDirection == "DESC"
						? query.OrderByDescending(t => t.Category)
						: query.OrderBy(t => t.Category);
					break;

				case "Priority":
					query = orderDirection == "DESC"
						? query.OrderByDescending(t => t.Priority)
						: query.OrderBy(t => t.Priority);
					break;

				case "UserName":
					query = orderDirection == "DESC"
						? query.OrderByDescending(t => t.UserName)
						: query.OrderBy(t => t.UserName);
					break;

				case "Status":
					query = orderDirection == "DESC"
						? query.OrderByDescending(t => t.Status)
						: query.OrderBy(t => t.Status);
					break;

				default:
					query = orderDirection == "DESC"
						? query.OrderByDescending(t => t.CreationTime)
						: query.OrderBy(t => t.CreationTime);
					break;
			}

			// Paginate the query
			query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);

			// Authorize user to edit this project
			bool userCanEditProject = true;
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != project.ProjectOwnerId)
				{
					userCanEditProject = false;
				}
			}

			// Instantiate view model
			var vm = new ViewProjectVM()
			{
				Project = project,
				UserCanEditProject = userCanEditProject
			};
			vm.DataPage = new DataPage<Ticket>()
			{
				CurrentPage = currentPage,
				PageSize = pageSize,
				Filter = filter,
				FullCount = fullCount,
				FilteredCount = filteredCount,
				OrderByColumn = orderByColumn,
				OrderDirection = orderDirection
			};

			// Execute the query
			vm.DataPage.Items = query.ToArray();

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
				// add the user ID and name of the creator
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				project.ProjectOwnerId = userId;
				project.ProjectOwnerUserName = User.Identity.Name;

				// create project
				_context.Add(project);
				await _context.SaveChangesAsync();

				// return view
				return RedirectToAction(nameof(Index));
			}
			return View(project);
		}

		// GET: Projects/Edit/5
		[Authorize(Roles = "Administrator, Project Manager")]
		public async Task<IActionResult> Edit(int? id)
		{
			// check that project exists
			if (id == null)
			{
				return NotFound();
			}
			var project = await _context.Project.FindAsync(id);
			if (project == null)
			{
				return NotFound();
			}

			// Authorize user to edit this project
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != project.ProjectOwnerId)
				{
					return Unauthorized();
				}
			}

			// fix any null data
			if (project.ProjectOwnerUserName == null)
			{
				project.ProjectOwnerUserName = "unassigned";
			}
			if (project.ProjectOwnerId == null)
			{
				project.ProjectOwnerId = "unassigned";
			}

			/*
			// get current project owner's username
			if (project.ProjectOwnerUserName == null)
			{
				string currentProjectOwnerUserName = "";
				try
				{
					currentProjectOwnerUserName = _context.Users
					.FindAsync(project.ProjectOwnerId)
					.Result.UserName;
				}
				catch
				{
					currentProjectOwnerUserName = "unassigned";
				}
			}
			*/

			// populate a list of all possible alternative Project Managers' usernames, for the dropdown box
			List<string> possibleProjectOwnerUserNames = new List<string>();
			var PMs = await _userManager.GetUsersInRoleAsync("Project Manager");
			var admins = await _userManager.GetUsersInRoleAsync("Administrator");
			PMs.ToList().ForEach(u => possibleProjectOwnerUserNames.Add(u.UserName));
			admins.ToList().ForEach(u => possibleProjectOwnerUserNames.Add(u.UserName));
			possibleProjectOwnerUserNames.Remove(project.ProjectOwnerUserName);

			EditProjectVM vm = new EditProjectVM
			{
				Project = project,
				PossibleProjectOwnerUserNames = possibleProjectOwnerUserNames
			};
			return View(vm);
		}

		// POST: Projects/Edit/5
		[Authorize(Roles = "Administrator, Project Manager")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id, Name, Description, ProjectOwnerUserName")] Project project, string ProjectOwnerUserName)
		{
			if (id != project.Id)
			{
				return NotFound();
			}

			try
			{
				// Get the User ID of the newly chosen PM from their username
				string ProjectOwnerId = (await _userManager.FindByNameAsync(ProjectOwnerUserName)).Id;
				if (ProjectOwnerId == null)
				{
					return BadRequest();
				}

				// and validate their role is at least PM
				List<string> possibleProjectOwnerUserNames = new List<string>();
				var PMs = await _userManager.GetUsersInRoleAsync("Project Manager");
				var admins = await _userManager.GetUsersInRoleAsync("Administrator");
				PMs.ToList().ForEach(u => possibleProjectOwnerUserNames.Add(u.UserName));
				admins.ToList().ForEach(u => possibleProjectOwnerUserNames.Add(u.UserName));
				if (!possibleProjectOwnerUserNames.Contains(ProjectOwnerUserName))
				{
					return BadRequest();
				}
				else
				{
					// save them as PM
					project.ProjectOwnerUserName = ProjectOwnerUserName;
					project.ProjectOwnerId = ProjectOwnerId;
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
				if (userId != project.ProjectOwnerId)
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
				if (userId != project.ProjectOwnerId)
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
				if (userId != project.ProjectOwnerId)
				{
					return Unauthorized();
				}
			}

			_context.Project.Remove(project);

			var tickets = await _context.Ticket
				.Where(t => t.ParentProjectId == id)
				.ToListAsync();

			foreach (Ticket t in tickets)
			{
				var comments = await _context.Comment
					.Where(c => c.ParentTicketId == t.Id)
					.ToListAsync();

				foreach (Comment c in comments)
				{
					_context.Comment.Remove(c);
				}

				_context.Ticket.Remove(t);
			}

			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProjectExists(int id)
		{
			return _context.Project.Any(e => e.Id == id);
		}
	}
}
