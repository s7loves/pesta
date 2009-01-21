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
using System.Web;
using System.Net;
using Pesta.Engine.gadgets;
using Pesta.Engine.gadgets.http;
using Pesta.Engine.gadgets.render;
using Pesta.Engine.gadgets.servlet;

namespace Pesta.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    internal class GadgetRenderingServlet : IHttpHandler
    {
        static readonly int DEFAULT_CACHE_TTL = 300;  // seconds
        private Renderer renderer = new Renderer();
        private HttpContext _context;

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest req = context.Request;
            HttpResponse resp = context.Response;
            _context = context;

            // If an If-Modified-Since header is ever provided, we always say
            // not modified. This is because when there actually is a change,
            // cache busting should occur.
            if (req.Headers["If-Modified-Since"] != null &&
                !"1".Equals(req.Params["nocache"]) &&
                req.Params["v"] != null)
            {
                resp.StatusCode =(int)HttpStatusCode.NotModified;
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}