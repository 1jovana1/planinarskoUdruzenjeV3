using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using planinarskoUdruzenjeV3.Areas.Identity.Data;
using planinarskoUdruzenjeV3.Models;

namespace planinarskoUdruzenjeV3.Controllers
{
    public class UserController : Controller
    {

        private readonly PlaninarskoUdruzenjeContext _context;

        public UserController(PlaninarskoUdruzenjeContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = _context.Users;
            return View(await users.ToListAsync());
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }


        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Edit(string id, [Bind("Id,FirstName,LastName,PhoneNumber")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    _context.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if ( !UserExists(user.Id))
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
            return View(user);
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

    }
}
