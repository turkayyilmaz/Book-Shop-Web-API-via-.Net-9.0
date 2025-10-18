using Microsoft.AspNetCore.Mvc;
using Presentations.ActionFilters;
using Services.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentations.Controllers
{
    [ApiController]
    [Route("api/book")]
    [ApiExplorerSettings(GroupName = "v2")]
    public class BookV2Controller : ControllerBase
    {
        private readonly IServiceManager _manager;
        public BookV2Controller(IServiceManager manager)
        {
            _manager = manager;
        }
        [HttpGet]
        public async Task<IActionResult> GetBooksAsync()
        {
            var books = await _manager.BookService.GetAllBooksAsync(false);
            var bookv2 = books.Select(b => new
            {
                b.BookId,
                b.Title,
                b.Description,
                b.Price,
                b.AuthorId,
                b.CategoryId,
                Summary = b.Description?.Length > 50 ? b.Description.Substring(0, 50) + "..." : b.Description
            });
            return Ok(bookv2);
        }
    }
}
