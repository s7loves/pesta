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
using pestaServer.Models.gadgets.servlet;
using HttpRequestWrapper = pestaServer.Models.gadgets.http.HttpRequestWrapper;
using HttpResponseWrapper = pestaServer.Models.gadgets.http.HttpResponseWrapper;

namespace pestaServer.Controllers
{
    public class makeRequestController : Controller
    {
        private static readonly MakeRequestHandler makeRequestHandler = MakeRequestHandler.Instance;

        [CompressFilter]
        public void Index()
        {
            HttpRequestWrapper request = new HttpRequestWrapper(System.Web.HttpContext.Current);
            HttpResponseWrapper response = new HttpResponseWrapper(System.Web.HttpContext.Current.Response);
            try
            {
                makeRequestHandler.Fetch(request, response);
            }
            catch (Exception e)
            {
                OutputError(e, response.getResponse());
            }
        }

        private static void OutputError(Exception e, HttpResponse resp)
        {
            resp.StatusCode = (int)HttpStatusCode.BadRequest;
            resp.StatusDescription = e.Message;
        }
    }
}
