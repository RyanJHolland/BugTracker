using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
		// Displays the most recent tickets on the chosen project. This is where a user browses a project's tickets.
		public async Task<IActionResult> View(
			int? id,
			int p = 1, int ps = 10, string q = null,
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

			// Retrieve data:
			var query = _context.Users.AsQueryable();
			var fullCount = query.Count();
			if (!String.IsNullOrWhiteSpace(q))
				query = query.Where(d => d.NormalizedUserName.Contains(q) || d.NormalizedEmail.Contains(q));

			var filteredCount = query.Count();
			/*
			// Build model:
			var model = new ProjectViewModel();
			model.DataPage = new DataPage<IdentityUser>()
			{
				CurrentPage = p,
				PageSize = ps,
				FullCount = fullCount,
				FilteredCount = filteredCount,
				Filter = q,
				Items = query.OrderBy(u => u.UserName).Page(p, ps)
			};
			*/


			// Validate that the sort order parameter is a valid column
			var props = typeof(Ticket).GetProperties()
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

			//if (!string.IsNullOrEmpty(search)){var tickets = await _context.Ticket		.Where(x => x.Title.Contains(search) || x.Body.Contains(search) || x.UserName.Contains(search));}


			var tickets = await _context.Ticket
			.FromSqlRaw<Ticket>($"SELECT * FROM Ticket WHERE ParentProjectId={id} ORDER BY {orderByColumn} {orderDirection} OFFSET {page * ticketsPerPage} ROWS FETCH NEXT {ticketsPerPage} ROWS ONLY;")
			.ToListAsync();

			// AND Title LIKE '%{search}%' OR Status LIKE ...
			// TO DO: change to parameterized query to prevent sql injection thru search field

			var totalTicketsInQuery = await _context.Ticket
				.Where(x => x.ParentProjectId == id)
				.CountAsync();

			oldViewProjectVM vm = new oldViewProjectVM
			{
				Project = project,
				Tickets = tickets,
				totalTicketsInQuery = totalTicketsInQuery,
				orderByColumn = orderByColumn,
				orderDirection = orderDirection,
				ticketsPerPage = ticketsPerPage,
				page = page,
				search = search
			};



			return View(vm);
		}

		/*
		// GET: Projects/View/5
		// Displays the most recent tickets on the chosen project. This is where a user browses a project's tickets.
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
			var props = typeof(Ticket).GetProperties()
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

			//if (!string.IsNullOrEmpty(search)){var tickets = await _context.Ticket		.Where(x => x.Title.Contains(search) || x.Body.Contains(search) || x.UserName.Contains(search));}


			var tickets = await _context.Ticket
			.FromSqlRaw<Ticket>($"SELECT * FROM Ticket WHERE ParentProjectId={id} ORDER BY {orderByColumn} {orderDirection} OFFSET {page * ticketsPerPage} ROWS FETCH NEXT {ticketsPerPage} ROWS ONLY;")
			.ToListAsync();

			// AND Title LIKE '%{search}%' OR Status LIKE ...
			// TO DO: change to parameterized query to prevent sql injection thru search field

			var totalTicketsInQuery = await _context.Ticket
				.Where(x => x.ParentProjectId == id)
				.CountAsync();

			ProjectTicketsViewModel vm = new ProjectTicketsViewModel
			{
				Project = project,
				Tickets = tickets,
				totalTicketsInQuery = totalTicketsInQuery,
				orderByColumn = orderByColumn,
				orderDirection = orderDirection,
				ticketsPerPage = ticketsPerPage,
				page = page,
				search = search
			};

			return View(vm);
		}
		*/

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
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ProjectExists(int id)
		{
			return _context.Project.Any(e => e.Id == id);
		}
	}
}
