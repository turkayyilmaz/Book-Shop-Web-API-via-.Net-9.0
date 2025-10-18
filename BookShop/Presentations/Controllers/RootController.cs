using Entities.LinkModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
namespace Presentations.Controllers
{
    [ApiController]
    [Route("api")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class RootController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;
        public RootController(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }
        [HttpGet(Name = "GetRoot")]
        public async Task<IActionResult> GetRoot([FromHeader(Name = "Accept")] string mediaType)
        {
            if (mediaType.Contains("application/vnd.btkakademi.apiroot"))
            {
                var list = new List<Link>()
                {
                    new Link()
                    {
                        Href = _linkGenerator.GetUriByAction(HttpContext, nameof(GetRoot), values: new{}),
                        Rel = "self",
                        Method = "GET"
                    },
                    new Link()
                    {
                        // bookcontroller'da getbooks action'ı için link oluşturuyoruz, [HttpGet(name = "GetBooks")] olmalı dikkat
                        Href = _linkGenerator.GetUriByAction(HttpContext, nameof(BookController.GetBooks), values: new{}),
                        Rel = "self",
                        Method = "GET"
                    },
                    new Link()
                    {
                        // bookcontroller'da getbooks action'ı için link oluşturuyoruz, [HttpGet(name = "GetBooks")] olmalı dikkat
                        Href = _linkGenerator.GetUriByAction(HttpContext, nameof(BookController.CreateOneBook), values: new{}),
                        Rel = "self",
                        Method = "POST"
                    }
                };
                return Ok(list);
            }
            return NoContent();
        }
    }
}
