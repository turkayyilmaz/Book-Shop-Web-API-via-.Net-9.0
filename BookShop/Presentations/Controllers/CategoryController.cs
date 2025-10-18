using Microsoft.AspNetCore.Mvc;
using Services.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentations.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public CategoryController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            return Ok(await _serviceManager.CategoryService.GetAllCategoriesAsync(trackChanges: false));
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOneCategoryById([FromRoute] int Id)
        {
            var book = await _serviceManager.CategoryService.GetOneCategoryByIdAsync(id:Id, trackChanges:false);
            return Ok(book);
        }
    }
}
