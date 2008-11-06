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
using System.Collections.Generic;

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
    internal class DataServiceServlet : ApiServlet, IHttpHandler
    {
        protected static String FORMAT_PARAM = "format";
        protected static String ATOM_FORMAT = "atom";

        public static String PEOPLE_ROUTE = "people";
        public static String ACTIVITY_ROUTE = "activities";
        public static String APPDATA_ROUTE = "appdata";

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            string method = request.HttpMethod;
            SecurityToken token = getSecurityToken(context); // BasicSecurityToken
            if (token == null)
            {
                sendSecurityError(response);
                return;
            }
            BeanConverter converter = getConverterForRequest(request);
            handleSingleRequest(request, response, token, converter);
        }

        private void handleSingleRequest(HttpRequest servletRequest, HttpResponse response, SecurityToken token, BeanConverter converter)
        {
            RestfulRequestItem requestItem = new RestfulRequestItem(servletRequest, token, converter);
            ResponseItem responseItem = getResponseItem(handleRequestItem(requestItem, servletRequest));

            if (responseItem != null && responseItem.getError() == null)
            {
                Object resp = responseItem.getResponse();
                // TODO: ugliness resulting from not using RestfulItem
                if (!(resp is DataCollection) && !(resp is RestfulCollection<object>))
                {
                    resp = new Dictionary<string, object>() { { "entry", resp } };
                }
                response.Output.Write(converter.convertToString(resp));
            }
            else
            {
                sendError(response, responseItem);
            }
        }

        BeanConverter getConverterForRequest(HttpRequest servletRequest)
        {
            String formatString = servletRequest.Params[FORMAT_PARAM];
            if (ATOM_FORMAT.Equals(formatString))
            {
                return xmlConverter;
            }
            return jsonConverter;
        }

        protected override void sendError(HttpResponse response, ResponseItem responseItem)
        {
            response.StatusCode = responseItem.getError().getHttpErrorCode();
            response.StatusDescription = responseItem.getErrorMessage();
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