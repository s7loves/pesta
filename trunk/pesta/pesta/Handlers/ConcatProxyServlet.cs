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
using System.Text;
using Pesta.Engine.gadgets.http;
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
    internal class ConcatProxyServlet : IHttpHandler
    {

        private ProxyHandler proxyHandler = ProxyHandler.Instance;

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            if (request.Headers["If-Modified-Since"] != null)
            {
                response.StatusCode = (int)HttpStatusCode.NotModified;
                return;
            }
            if (request.Params[ProxyBase.REWRITE_MIME_TYPE_PARAM] != null)
            {
                response.ContentType = request.Params[ProxyBase.REWRITE_MIME_TYPE_PARAM];
            }
            if (request.Params[ProxyBase.REFRESH_PARAM] != null)
            {
                int ttl = 0;
                int.TryParse(request.Params[ProxyBase.REFRESH_PARAM], out ttl);
                HttpUtil.setCachingHeaders(response, ttl);
            }
            response.AddHeader("Content-Disposition", "attachment;filename=p.txt");
            HttpResponseWrapper wrapper = new HttpResponseWrapper(response);
            for (int i = 1; i < int.MaxValue; i++)
            {
                string url = request.Params[i.ToString()];
                if (url == null)
                    break;

                try
                {
                    wrapper.Write(Encoding.UTF8.GetBytes("/* ---- Start " + url + " ---- */"));
                    proxyHandler.fetch(new RequestWrapper(context, url, true), wrapper);
                    wrapper.Write(Encoding.UTF8.GetBytes("/* ---- End " + url + " ---- */"));
                }
                catch (Exception ex)
                {
                    outputError(ex, url, response);
                    return;
                }
            }
            response.End();
        }

        private class RequestWrapper : HttpRequestWrapper
        {
            protected String url;
            public RequestWrapper(HttpContext context, String url, bool isConcat)
                : base(context, isConcat)
            {
                this.url = url;
            }

            public override String getParameter(String paramName)
            {
                if (ProxyHandler.URL_PARAM.Equals(paramName))
                {
                    return url;
                }
                return base.getParameter(paramName);
            }
        }

        private void outputError(Exception excep, String url, HttpResponse resp)
        {
            StringBuilder err = new StringBuilder();
            err.Append(excep.Source);
            err.Append(" concat(");
            err.Append(url);
            err.Append(") ");
            err.Append(excep.Message);

            // Log the errors here for now. We might want different severity levels
            // for different error codes.
            resp.StatusCode = (int)HttpStatusCode.BadRequest;
            resp.StatusDescription = err.ToString();
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