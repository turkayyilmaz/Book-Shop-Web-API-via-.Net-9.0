using Entities.DTOs;
using Entities.Entities;
using Entities.Exceptions;
using Entities.RequestFeatures;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentations.ActionFilters;
using Services.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentations.Controllers
{
    [ServiceFilter(typeof(LogFilterAttribute))]
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    //[ResponseCache(CacheProfileName = "300SecondsDuration")] // 60 saniye boyunca cache'le
    //[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 70)]
    public class BookController : ControllerBase
    {
        private readonly IServiceManager _manager;
        public BookController(IServiceManager manager)
        {
            _manager = manager;
        }
        [Authorize(Roles = "User")]
        [HttpGet(Name = "GetBooks")]
        [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
        public async Task<IActionResult> GetBooks([FromQuery] BookParameters bookParameters)
        {
            var linkParameters = new LinkParameters()
            {
                BookParameters = bookParameters,
                HttpContext = HttpContext
            };
            // trackChanges: false => değişiklikleri takip etme böylece performans artar
            var result = await _manager.BookService.GetAllBooksAsync(linkParameters, trackChanges: false);
            // frontend'e header olarak metaData'yı gönderiyoruz böylece sayfalama bilgilerini alabilir frontendçiler
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));
            return result.linkResponse.HasLinks ? 
                Ok(result.linkResponse.LinkedEntites) :
                Ok(result.linkResponse.ShapedEntites);
        }
        [Authorize(Roles = "Editor, User, Administrator")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _manager.BookService.GetOneBookByIdAsync(id, trackChanges: false);
            return Ok(book);
        }
        [Authorize]
        [HttpGet("details")]
        public async Task<IActionResult> GetAllBooksWithDetailsAsync()
        {
            var books = await _manager.BookService.GetAllBookWithDetailsAsync(false);
            return Ok(books);
        }
        [Authorize(Roles = "Administrator, Editor")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost(Name = "CreateOneBook")]
        public async Task<IActionResult> CreateOneBook(BookDtoForCreate book)
        {
            await _manager.BookService.CreateOneBookAsync(book);
            //return CreatedAtAction("GetBook", new { id = book.BookId }, book);
            return StatusCode(201, book);
        }
        [Authorize(Roles = "Administrator, Editor")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOneBook(int id, BookDtoForUpdate book)
        {
            if (book == null)
                return BadRequest("Book is null");
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            await _manager.BookService.UpdateOneBookAsync(id, book, trackChanges: false);
            return NoContent();
        }
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOneBook(int id)
        {
            await _manager.BookService.DeleteOneBookAsync(id, false);
            return NoContent();
        }
        [HttpGet]
        [Route("test")]
        public IActionResult TestHata()
        {
            throw new Exception("deneme hatası");
        }
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PartiallyUpdateBook(int id,
            [FromBody] JsonPatchDocument<BookDtoForUpdate> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest("patchDoc is null");
            var (bookDto, bookEntity) = await _manager.BookService.GetOneBookForPatchAsync(id, trackChanges: true);
            if (bookEntity == null)
                return NotFound();
            patchDoc.ApplyTo(bookDto, ModelState);
            TryValidateModel(bookDto);
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            await _manager.BookService.SaveChangesForPatchAsync(bookDto, bookEntity);
            return NoContent();
        }
        [HttpOptions]
        public IActionResult GetBookOptions()
        {
            // hangi methodlar destekleniyor, Allow keydir, value ise desteklenen methodlar
            Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE, PATCH");
            return Ok();
        }
    }
}
