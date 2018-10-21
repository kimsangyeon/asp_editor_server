using System;
using System.IO;
using System.Web;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using asp_editor_server.Models;

namespace asp_editor_server.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost("uploadFile")]
        public async Task<IActionResult> Post(List<IFormFile> file)
        {
            long size = file.Sum(f => f.Length);
            var uploadFile = file[0];
            string fileName = Path.GetFileName(uploadFile.FileName);
            string ext = Path.GetExtension(fileName);
            var uploadPath = Path.Combine("uploads", fileName);
            
            using (var stream = new FileStream(uploadPath, FileMode.Create))
            {
                await uploadFile.CopyToAsync(stream);
            }

            return Ok(new { count = file.Count, size, uploadPath });
        }
    }
}
