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
using System.Text;
using System.Web;
using System.Net;
using System.Collections.Generic;
using Pesta.Engine.gadgets;
using Pesta.Engine.gadgets.http;
using Pesta.Engine.gadgets.rewrite;
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
    internal class ProxyServlet : IHttpHandler
    {
        private ProxyHandler proxyHandler = ProxyHandler.Instance;

        
        public void ProcessRequest(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpResponseWrapper wrapper = new HttpResponseWrapper(response);
            try
            {
                proxyHandler.fetch(new ProxyRequestWrapper(context), wrapper);
            }
            catch (Exception ex)
            {
                outputError(ex, response);
                return;
            }
            response.End();
        }
        private void outputError(Exception excep, HttpResponse resp)
        {
            StringBuilder err = new StringBuilder();
            err.Append(excep.Source);
            err.Append(" proxy(");
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