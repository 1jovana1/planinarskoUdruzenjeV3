using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using planinarskoUdruzenjeV3.Models;

namespace planinarskoUdruzenjeV3.Controllers
{
    public class FileController : Controller
    {
        private readonly PlaninarskoUdruzenjeContext _context;

        public FileController(PlaninarskoUdruzenjeContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            var fileToRetrieve = _context.File.Find(id);
            return File(fileToRetrieve.Content, fileToRetrieve.ContentType);
        }

        //public ActionResult Show(int id)
        //{
        //    var file = _context.File.Find(x => x.Id == id);
        //   return file;
        //}
    }
}
