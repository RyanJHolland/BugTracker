using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TicketTracker.Common;
using TicketTracker.Data;
using TicketTracker.Models;
using TicketTracker.ViewModels;

namespace TicketTracker.Controllers
{
	public class CommentsController : Controller
	{
		#region Construction

		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserManager<IdentityUser> _userManager;

		public CommentsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_context = context;
			_userManager = userManager;
		}

		#endregion Construction

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

			return Redirect($"~/Tickets/View/{comment.ParentTicketId}?CommentId={id}");
		}

		// GET: Comments/Create/{ticketId}
		[Authorize]
		public async Task<IActionResult> Create(int id)
		{
			int ticketId = id; // This is just to clarify the following code.

			var ticket = await _context.Ticket
					.FirstOrDefaultAsync(m => m.Id == ticketId);
			if (ticket == null)
			{
				return NotFound();
			}

			var project = await _context.Project
					.FirstOrDefaultAsync(m => m.Id == ticket.ParentProjectId);
			if (project == null)
			{
				return NotFound();
			}

			var comment = new Comment();

			EditCommentVM vm = new EditCommentVM
			{
				Project = project,
				Ticket = ticket,
				Comment = comment
			};

			return View(vm);
		}

		// POST: Comments/Create/{ticketId}
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Title,Body")] Comment comment, int id, int? parentCommentId)
		{
			int ticketId = id; // This is just to clarify the following code.
			if (ModelState.IsValid)
			{
				var ticket = await _context.Ticket
					.FirstOrDefaultAsync(m => m.Id == ticketId);
				if (ticket == null)
				{
					return NotFound();
				}

				var project = await _context.Project
						.FirstOrDefaultAsync(m => m.Id == ticket.ParentProjectId);
				if (project == null)
				{
					return NotFound();
				}

				comment.ParentTicketId = ticketId;
				comment.UserId = User.GetUserId();
				comment.UserName = User.Identity.Name;
				comment.CreationTime = DateTime.UtcNow;
				if (parentCommentId != null)
				{
					comment.ParentCommentId = (int)parentCommentId;
				}

				_context.Add(comment);
				await _context.SaveChangesAsync();
				return Redirect($"~/Tickets/View/{ticketId}?CommentId={comment.Id}");
			}
			return Redirect($"~/Tickets/View/{ticketId}");
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

			// Authorize
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != comment.UserId)
				{
					return Unauthorized();
				}
			}

			var ticket = await _context.Ticket
					.FirstOrDefaultAsync(m => m.Id == comment.ParentTicketId);
			if (ticket == null)
			{
				return NotFound();
			}

			var project = await _context.Project
				 .FirstOrDefaultAsync(m => m.Id == ticket.ParentProjectId);
			if (project == null)
			{
				return NotFound();
			}

			EditCommentVM vm = new EditCommentVM
			{
				Project = project,
				Ticket = ticket,
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

					// Authorize
					if (!User.IsInRole("Administrator"))
					{
						var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
						if (userId != comment.UserId)
						{
							return Unauthorized();
						}
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

			// Authorize
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != comment.UserId)
				{
					return Unauthorized();
				}
			}

			var ticket = await _context.Ticket
					.FirstOrDefaultAsync(m => m.Id == comment.ParentTicketId);
			if (ticket == null)
			{
				return NotFound();
			}

			var project = await _context.Project
				 .FirstOrDefaultAsync(m => m.Id == ticket.ParentProjectId);
			if (project == null)
			{
				return NotFound();
			}

			EditCommentVM vm = new EditCommentVM
			{
				Project = project,
				Ticket = ticket,
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

			var comment = await _context.Comment
					.FirstOrDefaultAsync(m => m.Id == id);
			if (comment == null)
			{
				return NotFound();
			}

			// Authorize
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != comment.UserId)
				{
					return Unauthorized();
				}
			}

			_context.Comment.Remove(comment);
			await _context.SaveChangesAsync();

			return Redirect($"~/Tickets/View/{comment.ParentTicketId}");
		}

		private bool CommentExists(int id)
		{
			return _context.Comment.Any(e => e.Id == id);
		}
	}
}
