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

		/***********************************************************/
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


		/***********************************************************/
		#region GET: Projects
		public async Task<IActionResult> Index(
			string filter = ""
			)
		{

			#region Start to build a query
			var query = _context.Project
				.AsQueryable();
			#endregion

			#region Add search string filter to query
			if (!String.IsNullOrWhiteSpace(filter))
			{
				query = query
				.Where(t => t.Name.Contains(filter)
					|| t.Description.Contains(filter)
					|| t.ProjectOwnerUserName.Contains(filter));
			}
			#endregion

			#region Authorize user to edit this project
			bool userCanCreateProject =
				User.IsInRole("Administrator") || User.IsInRole("Project Manager");
			#endregion

			#region Instantiate view model
			var vm = new ProjectsIndex()
			{
				Projects = await query.ToListAsync(),
				UserCanCreateProject = userCanCreateProject,
				Filter = filter
			};
			#endregion

			return View(vm);
		}
		#endregion


		/***********************************************************/
		#region GET: Projects/View/5
		// Displays the most recent tickets on the chosen project. This is where a user browses a project's tickets.
		#region params
		public async Task<IActionResult> View(
			int? id,
			int currentPage = 1,
			int pageSize = 10,
			string filter = null,
			string orderDirection = "DESC",
			string orderByColumn = "CreationTime",
			string AllCategories = "on",
			string Bug = "on",
			string Feature = "on",
			string Style = "on",
			string Change = "on",
			string AllPriorities = "on",
			string Critical = "on",
			string High = "on",
			string Medium = "on",
			string Low = "on",
			string AllStatuses = "off",
			string Unassigned = "on",
			string Open = "on",
			string Resolved = "off",
			string Cancelled = "off",
			bool ShowFilterDropdown = false
			)
		#endregion
		{
			#region validate Id
			if (id == null)
			{
				return NotFound();
			}
			#endregion

			#region validate Project exists
			var project = await _context.Project
				.FirstOrDefaultAsync(m => m.Id == id);
			if (project == null)
			{
				return NotFound();
			}
			#endregion

			#region Start to build a query
			var query = _context.Ticket
				.Where(t => t.ParentProjectId == id)
				.AsQueryable();
			#endregion

			#region Apply filters
			if (Bug == "off") { query = query.Where(t => !t.Category.Equals(Ticket.CategoryEnum.Bug)); }
			if (Feature == "off") { query = query.Where(t => !t.Category.Equals(Ticket.CategoryEnum.Feature)); }
			if (Style == "off") { query = query.Where(t => !t.Category.Equals(Ticket.CategoryEnum.Style)); }
			if (Change == "off") { query = query.Where(t => !t.Category.Equals(Ticket.CategoryEnum.Change)); }
			if (Critical == "off") { query = query.Where(t => !t.Priority.Equals(Ticket.PriorityEnum.Critical)); }
			if (High == "off") { query = query.Where(t => !t.Priority.Equals(Ticket.PriorityEnum.High)); }
			if (Medium == "off") { query = query.Where(t => !t.Priority.Equals(Ticket.PriorityEnum.Medium)); }
			if (Low == "off") { query = query.Where(t => !t.Priority.Equals(Ticket.PriorityEnum.Low)); }
			if (Open == "off") { query = query.Where(t => !t.Status.Equals(Ticket.StatusEnum.Open)); }
			if (Unassigned == "off") { query = query.Where(t => !t.Status.Equals(Ticket.StatusEnum.Unassigned)); }
			if (Resolved == "off") { query = query.Where(t => !t.Status.Equals(Ticket.StatusEnum.Resolved)); }
			if (Cancelled == "off") { query = query.Where(t => !t.Status.Equals(Ticket.StatusEnum.Cancelled)); }
			#endregion

			#region Get count of all tickets in project
			var fullCount = await query.CountAsync();
			#endregion

			#region Add search string filter to query
			if (!String.IsNullOrWhiteSpace(filter))
			{
				query = query
				.Where(t => t.Body.Contains(filter)
					|| t.Title.Contains(filter)
					|| t.UserName.Contains(filter));
			}
			#endregion

			#region Get # of tickets that match the filter
			var filteredCount = await query.CountAsync();
			#endregion

			#region Add sort order and direction to query
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
			#endregion

			#region Paginate the query
			query = query.Skip((currentPage - 1) * pageSize).Take(pageSize);
			#endregion

			#region Authorize user to edit this project
			bool userCanEditProject = true;
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != project.ProjectOwnerId)
				{
					userCanEditProject = false;
				}
			}
			#endregion

			#region Instantiate view model
			var vm = new ViewProjectVM()
			{
				Project = project,
				UserCanEditProject = userCanEditProject,
				ShowFilterDropdown = ShowFilterDropdown
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
			#endregion

			#region Execute the query
			vm.DataPage.Items = query.ToArray();
			#endregion

			return View(vm);
		}
		#endregion


		/***********************************************************/
		#region GET: Projects/Create
		[Authorize(Roles = "Administrator, Project Manager")]
		public IActionResult Create()
		{
			return View();
		}
		#endregion


		/***********************************************************/
		#region POST: Projects/Create
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
		#endregion


		/***********************************************************/
		#region GET: Projects/Edit/5
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
		#endregion


		/***********************************************************/
		#region POST: Projects/Edit/5
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
		#endregion


		/***********************************************************/
		#region GET: Projects/Delete/5
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
		#endregion


		/***********************************************************/
		#region POST: Projects/Delete/5
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
	#endregion

}
