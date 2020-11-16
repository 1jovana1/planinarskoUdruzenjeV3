using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using planinarskoUdruzenjeV3.Models;

namespace planinarskoUdruzenjeV3.Controllers
{
    public class NewsController : Controller
    {
        private readonly PlaninarskoUdruzenjeContext _context;

        public NewsController(PlaninarskoUdruzenjeContext context)
        {
            _context = context;
        }

        public IActionResult Image(int id)
        {
            var fileToRetrieve = _context.File.Where(x=>x.NewsId == id).FirstOrDefault();
            if(fileToRetrieve == null)
            {
                var path = "~/images/wallpaperflare.com_wallpaper.jpg";
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
               news = news.Skip((p - 1) * pageSize).Take(pageSize);
               return View(await news.ToListAsync());
            }
            else
            {
                var news = _context.News.Skip((p - 1) * pageSize).Take(pageSize);
                ViewBag.TotalPages = (int)Math.Ceiling((decimal)_context.News.Count() / pageSize);
                return View(await news.ToListAsync());
            }



        }
       
        // GET: News/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News.Include(x=>x.File).FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }
            var createdByUserName = await _context.Users
                 .Where(x => x.Id ==(news.CreatedBy).ToString())
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
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Category,CreatedBy,CreatedAt")] News @news, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                //_context.Add(news);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Category,CreatedBy,CreatedAt")] News news)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(news);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewsExists(news.Id))
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
            return View(news);
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
