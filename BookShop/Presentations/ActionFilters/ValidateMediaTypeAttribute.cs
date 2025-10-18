using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentations.ActionFilters
{
    public class ValidateMediaTypeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Accept adında bir ifade var mı diye kontrol ediyoruz
            var acceptHeaderPresent = context.HttpContext.Request.Headers.ContainsKey("Accept");
            if (!acceptHeaderPresent)
            {
                context.Result = new BadRequestObjectResult($"Accept header is missing!");
                return;
            }
            var mediaType = context.HttpContext.Request.Headers["Accept"].FirstOrDefault();

            // using Microsoft.Net.Http.Headers olanı seç, systemi değil
            if (!MediaTypeHeaderValue.TryParse(mediaType, out MediaTypeHeaderValue? outMediaType))
            {
                context.Result = new BadRequestObjectResult($"Media Type not present. " + $"Please add Accept Header with required media type");
                return;
            }
            context.HttpContext.Items.Add("AcceptHeaderMediaType", outMediaType);
        }
    }
}
