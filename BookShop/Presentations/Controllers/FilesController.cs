using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentations.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            // file error yönetimi
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            // folder
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "Media");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            // path, hoca file'a ? ekledi, aksi halde dosya yüklemeden post edersen program çöküyor
            // böylece nullable yapınca error handling radarına giriyo ve hata fırlatıyor
            var path = Path.Combine(folder, file?.FileName);

            // stream
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            // response body, Created dönebilirsin ama hoca anonim veri gönderiyor
            return Ok(new
            {
                file = file.FileName,
                path = path,
                size = file.Length
            });
        }
        [HttpGet("download")]
        public async Task<IActionResult> Download(string fileName)
        {
            // File Path
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Media", fileName);
            // ContentType : (MIME)
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType)) // bu dosya türü olarak uygun mu diye bool döner
            {
                contentType = "application/octet-stream";
            }
            // Read, byte yani binary okuyoruz
            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));
        }
    }
}
