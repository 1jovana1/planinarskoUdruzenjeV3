using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.EventLog;
using Newtonsoft.Json.Linq;
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
            var fileToRetrieve = _context.File.Where(x => x.EventId == id && x.ContentType.StartsWith("image"))
                                .FirstOrDefault();
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
            var events = _context.Event
                .OrderByDescending(e => e.Id)
                .Skip((p - 1) * pageSize).Take(pageSize);


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

            var createdByUserName = await _context.Users
                .Where(x => x.Id == @event.CreatedBy)
                .FirstOrDefaultAsync();

            ViewBag.CreatedBy = createdByUserName != null ?
                createdByUserName.FirstName + " " + createdByUserName.LastName :
                "Nepoznato";

            ViewBag.isApproved = IsRegistered(id);
            ViewBag.Participants = GetParticipants(id);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.CanComment = _context.Participation
                                .Where(x => x.EventId == id && x.UserId == userId)
                                .Count();
            //komentari
            var listOfComments = GetComments(id);
            ViewBag.Comments = listOfComments;
            //ukupna ocjena na dogadjaju
            var average = 0;
            foreach(var comment in listOfComments)
            {
                average += comment.Stars;
            }
            ViewBag.Average = average == 0 ? 0 : average / (float)listOfComments.Count;
            return View(@event);
        }

        // GET: Events/Create
        [Authorize(Roles ="administrator")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        [Authorize(Roles = "administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Date,Deadline,MaxParticipanst,Location,Price")] Event @event, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                @event.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
        [Authorize(Roles = "administrator")]
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
        [Authorize(Roles = "administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Date,Deadline,MaxParticipanst,Location,Price")] Event @event, List<IFormFile> files, int[] oldFiles)
        {
            var _event = await _context.Event.FindAsync(id);
            if (_event == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _event.Title = @event.Title;
                _event.Description = @event.Description;
                _event.Date = @event.Date;
                _event.Deadline = @event.Deadline;
                _event.MaxParticipanst = @event.MaxParticipanst;
                _event.Location = @event.Location;
                _event.Price = @event.Price;

                foreach(var oldFileId in oldFiles)
                {
                    if(oldFileId != 0)
                    {
                        var file = await _context.File.FindAsync(oldFileId);
                        if(file != null)
                        {
                        _context.File.Remove(file);
                        }
                    }
                    
                }

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

                            _event.File.Add(file);
                        }
                    }

                }

                _context.Update(_event);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(_event);
        }


        [Authorize(Roles = "administrator")]
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
        [Authorize(Roles = "administrator")]
        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Delete foreign key
            var files =  _context.File.Where(x => x.EventId == id);
            _context.File.RemoveRange(files);

            var participations = _context.Participation
                .Where(x => x.EventId == id);
            _context.Participation.RemoveRange(participations);

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

            return RedirectToAction("Details", new { id = id });
         
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
        [Authorize(Roles = "administrator")]
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


        private IList<Rate> GetComments(int eventId)
        {

            var rate =  _context.Rate
                .Where(r => r.EventId == eventId)
                .Include(r => r.Event)
                .Include(r => r.User)
                .ToList();


            return rate;
        }
        public async Task<IActionResult> Activate(string userId, int eventId)
        {
            var participation = await _context.Participation
                .Where(x => x.EventId == eventId && x.UserId == userId)
                .FirstOrDefaultAsync();

            if (participation == null)
            {
                return NotFound();
            }

            participation.IsApproved = Convert.ToInt16(!Convert.ToBoolean(participation.IsApproved));

            _context.Update(participation);
            await _context.SaveChangesAsync();



            return RedirectToAction("Details", new { id = eventId });
        }
        //komentari i ocjene
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rating(int eventId, int stars, string comment)
        {
            //Da li event postoji
            var @event = await _context.Event.Where(e => e.Id == eventId).FirstOrDefaultAsync();
            if (@event == null)
            {
                return NotFound();
            }


            //KO je user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //da li je ucestvoao
            var participation = await _context.Participation
                .Where(x => x.EventId == eventId && x.UserId == userId && x.IsApproved == Participation.APPROVED)
                .FirstOrDefaultAsync();

            if (participation == null)
            {
                return NotFound();
            }

          
            //Da li je user vec komentarisao
            var isCommmented = await _context.Rate
                .Where(x => x.EventId == eventId && x.UserId == userId)
                .FirstOrDefaultAsync();

            if (isCommmented != null)
            {
                return NotFound();
            }


            var rate = new Rate()
            {
                EventId = eventId,
                UserId = userId,
                Comment = comment,
                Stars = stars
            };

             _context.Rate.Add(rate);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = eventId });
        }


    }
}
