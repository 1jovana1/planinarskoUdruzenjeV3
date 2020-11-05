using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.EventLog;
using planinarskoUdruzenjeV3.Areas.Identity.Data;
using planinarskoUdruzenjeV3.Models;

namespace planinarskoUdruzenjeV3.Controllers
{
    public class EventsController : Controller
    {
        private readonly PlaninarskoUdruzenjeContext _context;
        private readonly UserManager<User> _userManager;

        public EventsController(PlaninarskoUdruzenjeContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Image(int id)
        {
            var fileToRetrieve = _context.File.Where(x => x.EventId == id).FirstOrDefault();
            if (fileToRetrieve == null)
            {
                var path = "~/images/photo-1473984951266-787b955c9e0b.jpg";
                return File(path, "image/jpeg");
            }
           
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }
        // GET: Events
        public async Task<IActionResult> Index(int p=1)
        {
            int pageSize = 8;
            var events = _context.Event.Skip((p - 1) * pageSize).Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)_context.Event.Count() / pageSize);

            return View(await events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var @event = await _context.Event.Include(x => x.File).FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            ViewBag.isApproved = IsRegistered(id);
            ViewBag.Participants = GetParticipants(id);
            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Date,Deadline,MaxParticipanst,Location,Price")] Event @event, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                _context.Event.Add(@event);
                //add files
                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await formFile.CopyToAsync(memoryStream);

                            var file = new Models.File()
                            {
                                FileName = formFile.FileName,
                                ContentType = formFile.ContentType,
                                Content = memoryStream.ToArray()
                            };

                            @event.File.Add(file);
                        }
                    }

                }
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.Include(x=>x.File).SingleOrDefaultAsync(x => x.Id == id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Date,Deadline,MaxParticipanst,Location,Price,CreatedBy,CreatedAt")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
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
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Delete foreign key
            var files =  _context.File.Where(x => x.EventId == id);
            _context.File.RemoveRange(files);
            //Delete event
            var @event = await _context.Event.FindAsync(id);
            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Participate(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var exist = _context.Participation.Where(x => x.EventId == id && x.UserId == userId).FirstOrDefault();
            if(exist != null)
            {
                return RedirectToAction(nameof(Index));
            }

            var @event = await _context.Event
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            if(@event.Deadline < DateTime.Now)
            {
                //ToDo: Dodati ispis greske
                return RedirectToAction(nameof(Index));
            }



            var participation = new Participation();
            participation.EventId = id;
            participation.UserId = userId;
            participation.IsApproved = 0;

            _context.Participation.Add(participation);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private int IsRegistered(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var exist = _context.Participation.Where(x => x.EventId == id && x.UserId == userId).FirstOrDefault();
            if (exist == null)
            {
                return Participation.NOT_EXIST;
            }

            if(exist.IsApproved == 1)
            {
                return Participation.APPROVED;
            }

            return Participation.NOT_APPROVED;

        }

        private IList<Participant> GetParticipants(int id)
        {
            IList<Participant> participants = new List<Participant>();
            var participantsList = _context.Participation
                .Include(x => x.User)
                .Where(x => x.EventId == id)
                .ToList();


            foreach (var item in participantsList)
            {
                participants.Add(
                    new Participant
                    {
                        UserId = item.User.Id,
                        Name = item.User.FirstName + " " + item.User.LastName,
                        isApproved = Convert.ToBoolean(item.IsApproved)
                    });
            }

            return participants;
        }

    }
}
