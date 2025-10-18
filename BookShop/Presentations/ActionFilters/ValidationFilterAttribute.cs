using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentations.ActionFilters
{
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.RouteData.Values["controller"]; // controller adını al
            var action = context.RouteData.Values["action"]; // action adını al
            var param = context.ActionArguments
                .FirstOrDefault(x => x.Value.ToString().Contains("Dto")).Value; // Dto içeren parametreyi al
            if (param == null) // 400 BadRequest döndür
            {
                context.Result = new BadRequestObjectResult($"{controller} {action} Dto is null");
                return; // Dto null ise BadRequest döndür
            }
            if (!context.ModelState.IsValid) // ModelState geçerli değilse 422 doğrulanamadı döndür
            {
                context.Result = new UnprocessableEntityObjectResult(context.ModelState);
                return; // UnprocessableEntity döndür
            }
        }
    }
}
