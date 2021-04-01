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
	public class ZugsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ZugsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Zugs
		public async Task<IActionResult> Index()
		{
			return View(await _context.Zug.ToListAsync());
		}

		// GET: Zugs/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var zug = await _context.Zug
					.FirstOrDefaultAsync(m => m.Id == id);
			if (zug == null)
			{
				return NotFound();
			}

			return View(zug);
		}

		// GET: Zugs/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: Zugs/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Title,Body,UserId,UserName,CreationTime,ParentProjectId,Priority,Category,Status")] Zug zug)
		{
			if (ModelState.IsValid)
			{
				_context.Add(zug);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(zug);
		}

		// GET: Zugs/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var zug = await _context.Zug.FindAsync(id);
			if (zug == null)
			{
				return NotFound();
			}
			return View(zug);
		}

		// POST: Zugs/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Body,UserId,UserName,CreationTime,ParentProjectId,Priority,Category,Status")] Zug zug)
		{
			if (id != zug.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(zug);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ZugExists(zug.Id))
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
			return View(zug);
		}

		// GET: Zugs/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var zug = await _context.Zug
					.FirstOrDefaultAsync(m => m.Id == id);
			if (zug == null)
			{
				return NotFound();
			}

			return View(zug);
		}

		// POST: Zugs/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var zug = await _context.Zug.FindAsync(id);
			_context.Zug.Remove(zug);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool ZugExists(int id)
		{
			return _context.Zug.Any(e => e.Id == id);
		}
	}
}