#region License, Terms and Conditions
/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied. See the License for the
 * specific language governing permissions and limitations under the License.
 */
#endregion
using System;
using System.Net;
using System.Web;
using System.Web.Mvc;
using pestaServer.ActionFilters;
using pestaServer.Models.gadgets;
using pestaServer.Models.gadgets.http;
using pestaServer.Models.gadgets.render;
using pestaServer.Models.gadgets.servlet;

namespace pestaServer.Controllers
{
    public class ifrController : Controller
    {
        const int DEFAULT_CACHE_TTL = 300;  // seconds
        private HttpContext context;

        [CompressFilter]
        [ValidateInput(false)]
        public void Index()
        {
            HttpRequest req = System.Web.HttpContext.Current.Request;
            HttpResponse resp = System.Web.HttpContext.Current.Response;
            context = System.Web.HttpContext.Current;

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

            Render(req, resp);
        }

        private void Render(HttpRequest req, HttpResponse resp)
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

            GadgetContext gadgetContext = new HttpGadgetContext(context);
            Renderer renderer = new Renderer();
            RenderingResults results = renderer.Render(gadgetContext);
            switch (results.getStatus())
            {
                case RenderingResults.Status.OK:
                    if (gadgetContext.getIgnoreCache())
                    {
                        HttpUtil.SetCachingHeaders(resp, 0);
                    }
                    else if (req.Params["v"] != null)
                    {
                        // Versioned files get cached indefinitely
                        HttpUtil.SetCachingHeaders(resp, true);
                    }
                    else
                    {
                        // Unversioned files get cached for 5 minutes.
                        // TODO: This should be configurable
                        HttpUtil.SetCachingHeaders(resp, DEFAULT_CACHE_TTL, true);
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
