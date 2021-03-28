using BugTracker.Common;
using BugTracker.Data;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

// To-Do: Change the URL scheme to be something like Projects/View/#/Comments/View/#

namespace BugTracker.Controllers
{
	public class CommentsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public CommentsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Comments/View/5
		public async Task<IActionResult> View(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var comment = await _context.Comment
					.FirstOrDefaultAsync(m => m.Id == id);
			if (comment == null)
			{
				return NotFound();
			}

			return Redirect($"~/Bugs/View/{comment.ParentBugId}?CommentId={id}");
		}

		// GET: Comments/Create/{bugId}
		[Authorize]
		public async Task<IActionResult> Create(int id)
		{
			int bugId = id; // This is just to clarify the following code.

			var bug = await _context.Bug
					.FirstOrDefaultAsync(m => m.Id == bugId);
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

			var comment = new Comment();

			ProjectBugCommentViewModel vm = new ProjectBugCommentViewModel
			{
				Project = project,
				Bug = bug,
				Comment = comment
			};

			return View(vm);
		}

		// POST: Comments/Create/{bugId}
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Title,Body")] Comment comment, int id)
		{
			int bugId = id; // This is just to clarify the following code.
			if (ModelState.IsValid)
			{
				var bug = await _context.Bug
					.FirstOrDefaultAsync(m => m.Id == bugId);
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

				comment.ParentBugId = bugId;
				comment.UserId = User.GetUserId();
				comment.UserName = User.Identity.Name;
				comment.CreationTime = DateTime.UtcNow;

				_context.Add(comment);
				await _context.SaveChangesAsync();
				return Redirect($"~/Bugs/View/{bugId}?CommentId={comment.Id}");
			}
			return Redirect($"~/Bugs/View/{bugId}");
		}

		// GET: Comments/Edit/5
		[Authorize]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var comment = await _context.Comment
					.FirstOrDefaultAsync(m => m.Id == id);
			if (comment == null)
			{
				return NotFound();
			}

			var bug = await _context.Bug
					.FirstOrDefaultAsync(m => m.Id == comment.ParentBugId);
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

			ProjectBugCommentViewModel vm = new ProjectBugCommentViewModel
			{
				Project = project,
				Bug = bug,
				Comment = comment
			};

			return View(vm);
		}

		// POST: Comments/Edit/5
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Body")] Comment comment)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var commentToUpdate = await _context.Comment
					 .FirstOrDefaultAsync(m => m.Id == id);
					if (commentToUpdate == null)
					{
						return NotFound();
					}

					commentToUpdate.Body = comment.Body;

					_context.Update(commentToUpdate);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CommentExists(id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
			}
			return Redirect($"~/Comments/View/{id}");
		}

		// GET: Comments/Delete/5
		[Authorize]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var comment = await _context.Comment
					.FirstOrDefaultAsync(m => m.Id == id);
			if (comment == null)
			{
				return NotFound();
			}

			var bug = await _context.Bug
					.FirstOrDefaultAsync(m => m.Id == comment.ParentBugId);
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

			ProjectBugCommentViewModel vm = new ProjectBugCommentViewModel
			{
				Project = project,
				Bug = bug,
				Comment = comment
			};

			return View(vm);
		}

		// POST: Delete Comment
		[Authorize]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var comment = await _context.Comment.FindAsync(id);
			var bug = await _context.Bug.FindAsync(comment.ParentBugId);

			_context.Comment.Remove(comment);
			await _context.SaveChangesAsync();

			return Redirect($"~/Projects/View/{bug.ParentProjectId}");
		}

		private bool CommentExists(int id)
		{
			return _context.Comment.Any(e => e.Id == id);
		}
	}
}

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BugTracker.Data;
using BugTracker.Models;

namespace BugTracker.Controllers
{
	public class CommentsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public CommentsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Comments
		public async Task<IActionResult> Index()
		{
			return View(await _context.Comment.ToListAsync());
		}

		// GET: Comments/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var comment = await _context.Comment
					.FirstOrDefaultAsync(m => m.Id == id);
			if (comment == null)
			{
				return NotFound();
			}

			return View(comment);
		}

		// GET: Comments/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Comments/Create
		// To protect from overcommenting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Body,UserId,CreationTime")] Comment comment)
		{
			if (ModelState.IsValid)
			{
				_context.Add(comment);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(comment);
		}

		// GET: Comments/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var comment = await _context.Comment.FindAsync(id);
			if (comment == null)
			{
				return NotFound();
			}
			return View(comment);
		}

		// POST: Comments/Edit/5
		// To protect from overcommenting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Body,UserId,CreationTime")] Comment comment)
		{
			if (id != comment.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(comment);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CommentExists(comment.Id))
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
			return View(comment);
		}

		// GET: Comments/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var comment = await _context.Comment
					.FirstOrDefaultAsync(m => m.Id == id);
			if (comment == null)
			{
				return NotFound();
			}

			return View(comment);
		}

		// POST: Comments/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var comment = await _context.Comment.FindAsync(id);
			_context.Comment.Remove(comment);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool CommentExists(int id)
		{
			return _context.Comment.Any(e => e.Id == id);
		}
	}
}
*/