using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using planinarskoUdruzenjeV3.Areas.Identity.Data;
using planinarskoUdruzenjeV3.Models;

namespace planinarskoUdruzenjeV3.Controllers
{
    public class NewsController : Controller
    {
        private readonly PlaninarskoUdruzenjeContext _context;
        private readonly UserManager<User> _userManager;

        public NewsController(PlaninarskoUdruzenjeContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Image(int id)
        {
            var fileToRetrieve = _context.File.Where(x=>x.NewsId == id && x.ContentType.StartsWith("image"))
                .FirstOrDefault();
            if (fileToRetrieve == null)
            {
                var path = "~/images/photo-1473984951266-787b955c9e0b.jpg";
                return File(path, "image/jpeg");
            }


            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }

        // GET: News
        public async Task<IActionResult> Index(string category, int p=1)
        {
            int pageSize = 6;
            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;

            ViewBag.Category = category; 
            if (category != null)
            {
               var news = _context.News.Where(x => x.Category == category);
               ViewBag.TotalPages = (int)Math.Ceiling((decimal)news.Count() / pageSize);
               news = news.OrderByDescending(e => e.Id).Skip((p - 1) * pageSize).Take(pageSize);
               return View(await news.ToListAsync());
            }
            else
            {
                var news = _context.News.OrderByDescending(e => e.Id).Skip((p - 1) * pageSize).Take(pageSize);
                ViewBag.TotalPages = (int)Math.Ceiling((decimal)_context.News.Count() / pageSize);
                return View(await news.ToListAsync());
            }



        }
       
        // GET: News/Details/5
        public async Task<IActionResult> Details(int id)
        {
            
            var news = await _context.News.Include(x=>x.File).FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            var createdByUserName = await _context.Users
                 .Where(x => x.Id == news.CreatedBy)
                 .FirstOrDefaultAsync();

            ViewBag.CreatedBy = createdByUserName != null ?
                createdByUserName.FirstName + " " + createdByUserName.LastName :
                "Nepoznato";

            return View(news);
        }
        [Authorize(Roles = "administrator")]
        // GET: News/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Category")] News @news, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                @news.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _context.News.Add(@news);
                //add file
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

                            @news.File.Add(file);
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@news);
        }

        // GET: News/Edit/5
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.Include(x=>x.File).SingleOrDefaultAsync(x => x.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        // POST: News/Edit/5
        [Authorize(Roles = "administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Category")] News news, List<IFormFile> files, int[] oldFiles)
        {
            var _news = await _context.News.FindAsync(id); 

            if (_news == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _news.Title = news.Title;
                _news.Description = news.Description;
                _news.Category = news.Category;

                foreach (var oldFileId in oldFiles)
                {
                    if (oldFileId != 0)
                    {
                        var file = await _context.File.FindAsync(oldFileId);
                        if (file != null)
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

                            _news.File.Add(file);
                        }
                    }

                }
                _context.Update(_news);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(_news);
        }

        // GET: News/Delete/5
        [Authorize(Roles = "administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/Delete/5
        [Authorize(Roles = "administrator")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Delete foreign key
            var files = _context.File.Where(x => x.NewsId == id);
            _context.File.RemoveRange(files);
            //Delete news
            var news = await _context.News.FindAsync(id);
            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NewsExists(int id)
        {
            return _context.News.Any(e => e.Id == id);
        }
    }
}
