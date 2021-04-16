using BugTracker.Common;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

// To-Do: Change the URL scheme to be something like Projects/View/#/Bugs/View/#

namespace BugTracker.Controllers
{
	#region Construction

	public class BugsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserManager<IdentityUser> _userManager;

		public BugsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_context = context;
			_userManager = userManager;
		}

		#endregion Construction

		/*
		 * GET: Bugs/View/5
		 * This is where a user reads a bug after clicking on it.
		*/

		[Authorize]
		public async Task<IActionResult> View(int id)
		{
			var bug = await _context.Bug
					.FirstOrDefaultAsync(m => m.Id == id);
			if (bug == null)
			{
				return NotFound();
			}

			var project = await _context.Project
				 .FirstOrDefaultAsync(m => m.Id == bug.ParentProjectId);
			if (project == null)
			{
				return NotFound();
			}

			var comments = await _context.Comment
					.FromSqlRaw<Comment>($"SELECT * FROM Comment WHERE ParentBugId={id} ORDER BY CreationTime ASC;")
					.ToListAsync();

			ProjectBugCommentsViewModel vm = new ProjectBugCommentsViewModel
			{
				Project = project,
				Bug = bug,
				Comments = comments
			};

			return View(vm);
		}

		// GET: Bugs/Create/5
		[Authorize]
		public async Task<IActionResult> Create(int id)
		{
			var project = await _context.Project
					.FirstOrDefaultAsync(m => m.Id == id);
			if (project == null)
			{
				return NotFound();
			}

			var bug = new Bug();

			ProjectBugViewModel vm = new()
			{
				Project = project,
				Bug = bug
			};
			vm.Bug.Priority = Bug.PriorityEnum.Medium;
			vm.Bug.Category = Bug.CategoryEnum.Bug;
			vm.Bug.Status = Bug.StatusEnum.Open;

			return View(vm);
		}

		// POST: Bugs/Create/5
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Title,Body,Priority,Category,Status")] Bug bug, int id)
		{
			int projectId = id; // This is just to clarify the following code.
			if (ModelState.IsValid)
			{
				var project = await _context.Project
					 .FirstOrDefaultAsync(m => m.Id == projectId);
				if (project == null)
				{
					return NotFound();
				}

				bug.ParentProjectId = projectId;
				bug.UserId = User.GetUserId();
				bug.UserName = User.Identity.Name;
				bug.CreationTime = DateTime.UtcNow;

				_context.Add(bug);
				await _context.SaveChangesAsync();
				return Redirect($"~/Bugs/View/{bug.Id}");
			}
			return Redirect($"~/Bugs/Create/{projectId}");
		}

		// GET: Bugs/Edit/5
		[Authorize(Roles = "Administrator, Project Manager, Developer")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var bug = await _context.Bug
					.FirstOrDefaultAsync(m => m.Id == id);
			if (bug == null)
			{
				return NotFound();
			}

			// Authorize
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != bug.UserId)
				{
					return Unauthorized();
				}
			}

			var project = await _context.Project
				 .FirstOrDefaultAsync(m => m.Id == bug.ParentProjectId);
			if (project == null)
			{
				return NotFound();
			}

			ProjectBugViewModel vm = new()
			{
				Project = project,
				Bug = bug
			};

			return View(vm);
		}

		// POST: Bugs/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrator, Project Manager, Developer")]
		public async Task<IActionResult> Edit(int id, [Bind("Title,Body,Priority,Category,Status")] Bug bug)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var bugToUpdate = await _context.Bug
					 .FirstOrDefaultAsync(m => m.Id == id);
					if (bugToUpdate == null)
					{
						return NotFound();
					}

					// Authorize
					if (!User.IsInRole("Administrator"))
					{
						var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
						if (userId != bug.UserId)
						{
							return Unauthorized();
						}
					}

					bugToUpdate.Title = bug.Title;
					bugToUpdate.Body = bug.Body;
					bugToUpdate.Priority = bug.Priority;
					bugToUpdate.Category = bug.Category;
					bugToUpdate.Status = bug.Status;

					_context.Update(bugToUpdate);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!BugExists(id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
			}
			return Redirect($"~/Bugs/View/{id}");
		}

		// GET: Bugs/Delete/5
		[Authorize(Roles = "Administrator, Project Manager, Developer")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var bug = await _context.Bug
					.FirstOrDefaultAsync(m => m.Id == id);
			if (bug == null)
			{
				return NotFound();
			}

			// Authorize
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != bug.UserId)
				{
					return Unauthorized();
				}
			}

			var project = await _context.Project
				 .FirstOrDefaultAsync(m => m.Id == bug.ParentProjectId);
			if (project == null)
			{
				return NotFound();
			}

			ProjectBugViewModel vm = new ProjectBugViewModel
			{
				Project = project,
				Bug = bug
			};

			return View(vm);
		}

		// POST: Delete Bug
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrator, Project Manager, Developer")]
		public async Task<IActionResult> DeleteConfirmed(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var bug = await _context.Bug.FindAsync(id);


			// Authorize
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != bug.UserId)
				{
					return Unauthorized();
				}
			}

			int projectId = bug.ParentProjectId;

			_context.Bug.Remove(bug);
			await _context.SaveChangesAsync();

			return Redirect($"~/Projects/View/{projectId}");
		}

		private bool BugExists(int id)
		{
			return _context.Bug.Any(e => e.Id == id);
		}
	}
}
