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

// To-Do: Change the URL scheme to be something like Projects/View/#/Tickets/View/#

namespace TicketTracker.Controllers
{
	#region Construction

	public class TicketsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserManager<IdentityUser> _userManager;

		public TicketsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
		{
			_httpContextAccessor = httpContextAccessor;
			_context = context;
			_userManager = userManager;
		}

		#endregion Construction

		/*
		 * GET: Tickets/View/5
		 * This is where a user reads a ticket after clicking on it.
		*/

		[Authorize]
		public async Task<IActionResult> View(int id)
		{
			var ticket = await _context.Ticket
					.FirstOrDefaultAsync(m => m.Id == id);
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

			var comments = await _context.Comment
					.FromSqlRaw<Comment>($"SELECT * FROM Comment WHERE ParentTicketId={id} ORDER BY CreationTime ASC;")
					.ToListAsync();

			ViewTicketVM vm = new ViewTicketVM
			{
				Project = project,
				Ticket = ticket,
				Comments = comments
			};

			return View(vm);
		}

		// GET: Tickets/Create/5
		[Authorize]
		public async Task<IActionResult> Create(int id)
		{
			var project = await _context.Project
					.FirstOrDefaultAsync(m => m.Id == id);
			if (project == null)
			{
				return NotFound();
			}

			var ticket = new Ticket();

			EditTicketVM vm = new()
			{
				Project = project,
				Ticket = ticket
			};
			vm.Ticket.Priority = Ticket.PriorityEnum.Medium;
			vm.Ticket.Category = Ticket.CategoryEnum.Bug;
			vm.Ticket.Status = Ticket.StatusEnum.Open;

			return View(vm);
		}

		// POST: Tickets/Create/5
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Title,Body,Priority,Category,Status")] Ticket ticket, int id)
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

				ticket.ParentProjectId = projectId;
				ticket.UserId = User.GetUserId();
				ticket.UserName = User.Identity.Name;
				ticket.CreationTime = DateTime.UtcNow;

				_context.Add(ticket);
				await _context.SaveChangesAsync();
				return Redirect($"~/Tickets/View/{ticket.Id}");
			}
			return Redirect($"~/Tickets/Create/{projectId}");
		}

		// GET: Tickets/Edit/5
		[Authorize(Roles = "Administrator, Project Manager, Developer")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var ticket = await _context.Ticket
					.FirstOrDefaultAsync(m => m.Id == id);
			if (ticket == null)
			{
				return NotFound();
			}

			// Authorize
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != ticket.UserId)
				{
					return Unauthorized();
				}
			}

			var project = await _context.Project
				 .FirstOrDefaultAsync(m => m.Id == ticket.ParentProjectId);
			if (project == null)
			{
				return NotFound();
			}

			EditTicketVM vm = new()
			{
				Project = project,
				Ticket = ticket
			};

			return View(vm);
		}

		// POST: Tickets/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrator, Project Manager, Developer")]
		public async Task<IActionResult> Edit(
			int id,
			[Bind("Title,Body,Priority,Category,Status")] Ticket ticket,
			string? redirectUrl
			)
		{
			if (ModelState.IsValid)
			{
				try
				{
					var ticketToUpdate = await _context.Ticket
					 .FirstOrDefaultAsync(m => m.Id == id);
					if (ticketToUpdate == null)
					{
						return NotFound();
					}

					// Authorize
					if (!User.IsInRole("Administrator"))
					{
						var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
						if (userId != ticket.UserId)
						{
							return Unauthorized();
						}
					}

					ticketToUpdate.Title = ticket.Title;
					ticketToUpdate.Body = ticket.Body;
					ticketToUpdate.Priority = ticket.Priority;
					ticketToUpdate.Category = ticket.Category;
					ticketToUpdate.Status = ticket.Status;

					_context.Update(ticketToUpdate);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!TicketExists(id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
			}

			// if this ticket was edited from the Project View table, redirect there
			if (redirectUrl != null)
			{
				return Redirect(redirectUrl);
			}
			// else redirect to the ticket
			return Redirect($"~/Tickets/View/{id}");
		}

		// GET: Tickets/Delete/5
		[Authorize(Roles = "Administrator, Project Manager, Developer")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var ticket = await _context.Ticket
					.FirstOrDefaultAsync(m => m.Id == id);
			if (ticket == null)
			{
				return NotFound();
			}

			// Authorize
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != ticket.UserId)
				{
					return Unauthorized();
				}
			}

			var project = await _context.Project
				 .FirstOrDefaultAsync(m => m.Id == ticket.ParentProjectId);
			if (project == null)
			{
				return NotFound();
			}

			EditTicketVM vm = new EditTicketVM
			{
				Project = project,
				Ticket = ticket
			};

			return View(vm);
		}

		// POST: Delete Ticket
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = "Administrator, Project Manager, Developer")]
		public async Task<IActionResult> DeleteConfirmed(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			var ticket = await _context.Ticket.FindAsync(id);

			// Authorize
			if (!User.IsInRole("Administrator"))
			{
				var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
				if (userId != ticket.UserId)
				{
					return Unauthorized();
				}
			}

			var comments = await _context.Comment
				.Where(c => c.ParentTicketId == ticket.Id)
				.ToListAsync();

			foreach (Comment c in comments)
			{
				_context.Comment.Remove(c);
			}

			int projectId = ticket.ParentProjectId;

			_context.Ticket.Remove(ticket);
			await _context.SaveChangesAsync();

			return Redirect($"~/Projects/View/{projectId}");
		}

		private bool TicketExists(int id)
		{
			return _context.Ticket.Any(e => e.Id == id);
		}
	}
}