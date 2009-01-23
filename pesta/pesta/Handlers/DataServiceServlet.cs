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
using Pesta.Engine.auth;
using Pesta.Engine.social.service;
using Pesta.Engine.social.spi;

namespace Pesta.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    internal class DataServiceServlet : ApiServlet, IHttpHandler
    {
        protected static readonly String FORMAT_PARAM = "format";
        protected static readonly String ATOM_FORMAT = "atom";
        protected static readonly String XML_FORMAT = "xml";

        public static readonly String PEOPLE_ROUTE = "people";
        public static readonly String ACTIVITY_ROUTE = "activities";
        public static readonly String APPDATA_ROUTE = "appdata";
        public static readonly String CONTENT_TYPE = "CONTENT_TYPE";

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            string method = request.HttpMethod;
            ISecurityToken token = getSecurityToken(context); // BasicSecurityToken
            if (token == null)
            {
                sendSecurityError(response);
                return;
            }
            BeanConverter converter = getConverterForRequest(request);
            handleSingleRequest(request, response, token, converter);
        }

        private void handleSingleRequest(HttpRequest servletRequest, HttpResponse response, ISecurityToken token, BeanConverter converter)
        {
            RestfulRequestItem requestItem = new RestfulRequestItem(servletRequest, token, converter);
            ResponseItem responseItem = getResponseItem(handleRequestItem(requestItem, servletRequest));

            response.ContentType = converter.getContentType();
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
            String formatString = null;
            BeanConverter converter = null;
            String contentType = null;

            try
            {
                formatString = servletRequest.Params[FORMAT_PARAM];
            }
            catch (Exception t)
            {
                // this happens while testing
            }
            try
            {
                contentType = servletRequest.Headers[CONTENT_TYPE];
            }
            catch (Exception t)
            {
                //this happens while testing
            }

            if (contentType != null)
            {
                if (contentType.Equals("application/json"))
                {
                    converter = jsonConverter;
                }
                else if (contentType.Equals("application/atom+xml"))
                {
                    converter = atomConverter;
                }
                else if (contentType.Equals("application/xml"))
                {
                    converter = xmlConverter;
                }
                else if (formatString == null)
                {
                    // takes care of cases where content!= null but is ""
                    converter = jsonConverter;
                }
            }
            else if (formatString != null)
            {
                if (formatString.Equals(ATOM_FORMAT))
                {
                    converter = atomConverter;
                }
                else if (formatString.Equals(XML_FORMAT))
                {
                    converter = xmlConverter;
                }
                else
                {
                    converter = jsonConverter;
                }
            }
            else
            {
                converter = jsonConverter;
            }
            return converter;
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