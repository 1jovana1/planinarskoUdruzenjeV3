using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using planinarskoUdruzenjeV3.Models;

namespace planinarskoUdruzenjeV3.Controllers
{
    public class RatesController : Controller
    {
        private readonly PlaninarskoUdruzenjeContext _context;

        public RatesController(PlaninarskoUdruzenjeContext context)
        {
            _context = context;
        }

        // GET: Rates
        public async Task<IActionResult> Index()
        {
            var planinarskoUdruzenjeContext = _context.Rate.Include(r => r.Event);
            return View(await planinarskoUdruzenjeContext.ToListAsync());
        }

        // GET: Rates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rate = await _context.Rate
                .Include(r => r.Event)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rate == null)
            {
                return NotFound();
            }

            return View(rate);
        }

        // GET: Rates/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Event, "Id", "Description");
            return View();
        }

        // POST: Rates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EventId,UserId,Rate1,Comment,CreatedAt")] Rate rate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Event, "Id", "Description", rate.EventId);
            return View(rate);
        }

        // GET: Rates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rate = await _context.Rate.FindAsync(id);
            if (rate == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Event, "Id", "Description", rate.EventId);
            return View(rate);
        }

        // POST: Rates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EventId,UserId,Rate1,Comment,CreatedAt")] Rate rate)
        {
            if (id != rate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RateExists(rate.Id))
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
            ViewData["EventId"] = new SelectList(_context.Event, "Id", "Description", rate.EventId);
            return View(rate);
        }

        // GET: Rates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rate = await _context.Rate
                .Include(r => r.Event)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rate == null)
            {
                return NotFound();
            }

            return View(rate);
        }

        // POST: Rates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rate = await _context.Rate.FindAsync(id);
            _context.Rate.Remove(rate);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RateExists(int id)
        {
            return _context.Rate.Any(e => e.Id == id);
        }

        [HttpGet]
        public async Task<IActionResult> Comments(int eventId)
        {

            var rate = await _context.Rate
                .Where(r => r.EventId == eventId)
                .Include(r => r.Event)
                .Include(r => r.User)
                .ToListAsync();


            return PartialView(rate);
        }


    }
}
