using BugTracker.Data;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
	public class ProjectsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ProjectsController(ApplicationDbContext context)
		{
			_context = context;
		}

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
			string orderBy = "CreationTime",
			string order = "DESC")
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
			/*
			var bugs = await _context.Bug
					.Where(b => b.ParentProjectId == id)
					.ToListAsync();

			bugs.Sort(delegate (Bug x, Bug y)
			{
				return x.GetType().GetProperty(orderBy).GetValue(orderBy) > y.GetType().GetProperty(orderBy).GetValue(orderBy);
			});
			*/

			// Validate that the sort order parameter is a valid column
			var props = typeof(Bug).GetProperties()
				.Select(prop => prop.Name)
				.ToArray();
			if (!props.Contains(orderBy))
			{
				orderBy = "CreationTime";
			}
			// Validate the ascending or descending order parameter
			if (order != "ASC")
			{
				order = "DESC";
			}

			var bugs = await _context.Bug
					.FromSqlRaw<Bug>($"SELECT * FROM Bug WHERE ParentProjectId={id} ORDER BY {orderBy} {order};")
					.ToListAsync();

			ProjectBugsViewModel vm = new ProjectBugsViewModel
			{
				Project = project,
				Bugs = bugs,
				order = order,
				orderBy = orderBy
			};

			return View(vm);
		}

		/*
		 *
		 * Below are actions for admins only. (Creating, editing, removing projects.)
		 *
		 */

		// I might remove this one. It will not be useful unless I add more properties to Project.
		// GET: Projects/Details/5
		[Authorize]
		public async Task<IActionResult> Details(int? id)
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

			return View(project);
		}

		// GET: Projects/Create
		[Authorize]
		public IActionResult Create()
		{
			return View();
		}

		// POST: Projects/Create
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Name,Description")] Project project)
		{
			if (ModelState.IsValid)
			{
				_context.Add(project);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(project);
		}

		[Authorize]
		// GET: Projects/Edit/5
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
			return View(project);
		}

		// POST: Projects/Edit/5
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Project project)
		{
			if (id != project.Id)
			{
				return NotFound();
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
			return View(project);
		}

		// GET: Projects/Delete/5
		[Authorize]
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

			return View(project);
		}

		// POST: Projects/Delete/5
		[Authorize]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var project = await _context.Project.FindAsync(id);
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