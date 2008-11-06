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

namespace Pesta
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    internal class GadgetRenderingServlet : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            HttpRequest req = context.Request;
            HttpResponse resp = context.Response;

            // If an If-Modified-Since header is ever provided, we always say
            // not modified. This is because when there actually is a change,
            // cache busting should occur.
            if (req.Headers["If-Modified-Since"] != null &&
                !"1".Equals(req.Params["nocache"]) &&
                req.Params["v"] != null)
            {
                resp.StatusCode = 304;
                return;
            }
            GadgetRenderingTask renderProvider = new GadgetRenderingTask();

            try
            {
                renderProvider.process(context);
            }
            catch (WebException e)
            {
                if (e.Response == null && e.Status == WebExceptionStatus.Timeout)
                {
                    resp.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    resp.StatusDescription = e.Message;
                }
            }
            catch (Exception ex)
            {
                resp.StatusCode = (int)HttpStatusCode.InternalServerError;
                resp.StatusDescription = ex.Message;
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