using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using pestaServer.Models.gadgets;
using pestaServer.Models.gadgets.http;
using pestaServer.Models.gadgets.render;
using pestaServer.Models.gadgets.servlet;

namespace pestaServer.Controllers
{
    public class ifrController : Controller
    {
        const int DEFAULT_CACHE_TTL = 300;  // seconds
        private HttpContext _context;

        public void Index()
        {
            HttpRequest req = System.Web.HttpContext.Current.Request;
            HttpResponse resp = System.Web.HttpContext.Current.Response;
            _context = System.Web.HttpContext.Current;

            // If an If-Modified-Since header is ever provided, we always say
            // not modified. This is because when there actually is a change,
            // cache busting should occur.
            if (req.Headers["If-Modified-Since"] != null &&
                !"1".Equals(req.Params["nocache"]) &&
                req.Params["v"] != null)
            {
                resp.StatusCode = (int)HttpStatusCode.NotModified;
                return;
            }

            render(req, resp);
        }

        private void render(HttpRequest req, HttpResponse resp)
        {
            if (!String.IsNullOrEmpty(req.Headers[sRequest.DOS_PREVENTION_HEADER]))
            {
                // Refuse to render for any request that came from us.
                // TODO: Is this necessary for any other type of request? Rendering seems to be the only one
                // that can potentially result in an infinite loop.
                resp.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            resp.ContentType = "text/html";
            resp.ContentEncoding = System.Text.Encoding.UTF8;

            GadgetContext context = new HttpGadgetContext(_context);
            Renderer renderer = new Renderer();
            RenderingResults results = renderer.render(context);
            switch (results.getStatus())
            {
                case RenderingResults.Status.OK:
                    if (context.getIgnoreCache())
                    {
                        HttpUtil.setCachingHeaders(resp, 0);
                    }
                    else if (req.Params["v"] != null)
                    {
                        // Versioned files get cached indefinitely
                        HttpUtil.setCachingHeaders(resp, true);
                    }
                    else
                    {
                        // Unversioned files get cached for 5 minutes.
                        // TODO: This should be configurable
                        HttpUtil.setCachingHeaders(resp, DEFAULT_CACHE_TTL, true);
                    }
                    resp.Output.Write(results.getContent());
                    break;
                case RenderingResults.Status.ERROR:
                    resp.Output.Write(results.getErrorMessage());
                    break;
                case RenderingResults.Status.MUST_REDIRECT:
                    //resp.sendRedirect(results.getRedirect().ToString());
                    break;
            }
        }
    }
}
