using System;
using System.IO;
using System.Web;
using System.IO.Compression;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using asp_editor_server.Models;

using ICSharpCode;

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
        public async Task<IActionResult> UploadFile(List<IFormFile> file)
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

        [HttpPost("importDoc")]
        public async Task<IActionResult> ImportDoc(List<IFormFile> file)
        {
            string fileName = "document.word.pb";
            string ext = Path.GetExtension(fileName);
            var unzipPath = Path.Combine("uploads/doc/unzip", fileName);

            List<int> serializedData = new List<int>();

            using (FileStream stream = new FileStream(unzipPath, FileMode.Open, FileAccess.Read))
            {
                stream.Seek(16, SeekOrigin.Begin);

                //using (ZLIBStream zs = new ZLIBStream(stream, CompressionMode.Decompress, true))
                //{
                //    int bytesLeidos = 0;

                //    byte[] buffer = new byte[1024];
                //    while ((bytesLeidos = zs.Read(buffer, 0, buffer.Length)) > 0)
                //    {
                //        for (int i = 0; i < bytesLeidos; i++)
                //        {
                //            serializedData.Add(buffer[i] & 0xFF);
                //        }
                //    }
                //}
                using (ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream ifis = new ICSharpCode.SharpZipLib.Zip.Compression.Streams.InflaterInputStream(stream)) {
                    int bytesLeidos = 0;

                    byte[] buffer = new byte[1024];
                    while ((bytesLeidos = ifis.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        for (int i = 0; i < bytesLeidos; i++)
                        {
                            serializedData.Add(buffer[i] & 0xFF);
                        }
                    }
                }
            }

            return Ok(new { serializedData });
        }
    }
}
