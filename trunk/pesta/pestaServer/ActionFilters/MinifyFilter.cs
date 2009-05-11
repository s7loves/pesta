using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace pestaServer.ActionFilters
{
    public class MinifyFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;

            HttpResponseBase response = filterContext.HttpContext.Response;

        }
    }
}