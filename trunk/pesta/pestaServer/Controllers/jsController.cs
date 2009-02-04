using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Pesta.Utilities;
using pestaServer.Models.common;
using pestaServer.Models.gadgets;
using pestaServer.Models.gadgets.servlet;

namespace pestaServer.Controllers
{
    public class jsController : Controller
    {
        private readonly GadgetFeatureRegistry registry = GadgetFeatureRegistry.Instance;

        public void Index(string resourceName)
        {
            HttpRequest req = System.Web.HttpContext.Current.Request;
            HttpResponse resp = System.Web.HttpContext.Current.Response;

            // If an If-Modified-Since header is ever provided, we always say
            // not modified. This is because when there actually is a change,
            // cache busting should occur.
            if (req.Params["If-Modified-Since"] != null &&
                req.Params["v"] != null)
            {
                resp.StatusCode = 304;
                return;
            }

            if (resourceName.EndsWith(".js"))
            {
                // Lop off the suffix for lookup purposes
                resourceName = resourceName.Substring(0, resourceName.Length - ".js".Length);
            }

            HashKey<string> needed = new HashKey<string>();
            if (resourceName.Contains("__"))
            {
                foreach (string item in resourceName.Split(new[]{'_'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    needed.Add(item);
                }

            }
            else
            {
                needed.Add(resourceName);
            }

            String debugStr = req.Params["debug"];
            String container = req.Params["container"];
            String containerStr = req.Params["c"];

            bool debug = "1".Equals(debugStr);
            if (container == null)
            {
                container = ContainerConfig.DEFAULT_CONTAINER;
            }
            RenderingContext rcontext = "1".Equals(containerStr) ?
                                                                     RenderingContext.CONTAINER : RenderingContext.GADGET;

            ICollection<GadgetFeature> features = registry.getFeatures(needed);
            StringBuilder jsData = new StringBuilder();
            foreach (GadgetFeature feature in features)
            {
                foreach (JsLibrary lib in feature.getJsLibraries(rcontext, container))
                {
                    if (!lib._Type.Equals(JsLibrary.Type.URL))
                    {
                        if (debug)
                        {
                            jsData.Append(lib.DebugContent);
                        }
                        else
                        {
                            jsData.Append(lib.Content);
                        }
                        jsData.Append(";\n");
                    }
                }
            }

            if (jsData.Length == 0)
            {
                resp.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            if (req.Params["v"] != null)
            {
                // Versioned files get cached indefinitely
                HttpUtil.setCachingHeaders(resp);
            }
            else
            {
                // Unversioned files get cached for 1 hour.
                HttpUtil.setCachingHeaders(resp, 60 * 60);
            }
            resp.ContentType = "text/javascript; charset=utf-8";
            resp.ContentEncoding = Encoding.UTF8;
            resp.Output.Write(jsData.ToString());
        }

    }
}
