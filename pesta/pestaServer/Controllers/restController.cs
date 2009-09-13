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
using System.Collections.Generic;
using System.Net;
using System.Web;
using pesta.Data;
using Pesta.Engine.auth;
using Pesta.Engine.protocol;
using Pesta.Engine.protocol.conversion;
using Pesta.Engine.social.spi;
using System.Linq;
using pestaServer.ActionFilters;
using pestaServer.Models.social.service;

namespace pestaServer.Controllers
{
    public class restController : ApiController
    {
        public static readonly IEnumerable<String> ALLOWED_CONTENT_TYPES =
                                          ContentTypes.ALLOWED_JSON_CONTENT_TYPES
                                          .Union(ContentTypes.ALLOWED_XML_CONTENT_TYPES)
                                          .Union(ContentTypes.ALLOWED_ATOM_CONTENT_TYPES);

        private static readonly String X_HTTP_METHOD_OVERRIDE = "X-HTTP-Method-Override";
        
        [CompressFilter]
        [AuthenticationFilter]
        public void Index()
        {
            var httpMethod = Request.HttpMethod;
            var servletRequest = System.Web.HttpContext.Current.Request;
            var servletResponse = System.Web.HttpContext.Current.Response;

            if (httpMethod == "GET" || httpMethod == "DELETE")
            {
                executeRequest(servletRequest, servletResponse);
            }
            else if (httpMethod == "PUT" || httpMethod == "POST")
            {
                try
                {
                    //checkContentTypes(ALLOWED_CONTENT_TYPES, servletRequest.ContentType);
                    executeRequest(servletRequest, servletResponse);
                }
                catch (ContentTypes.InvalidContentTypeException icte)
                {
                    sendError(servletResponse,
                        new ResponseItem((int)HttpStatusCode.BadRequest, icte.Message));
                }
            }

        }

        /// <summary>
        /// Actual dispatch handling for servlet requests
        /// </summary>
        /// <param name="servletRequest"></param>
        /// <param name="servletResponse"></param>
        private void executeRequest(HttpRequest servletRequest, HttpResponse servletResponse)
        {
            setCharacterEncodings(servletRequest, servletResponse);

            ISecurityToken token = getSecurityToken(System.Web.HttpContext.Current, servletRequest.RawUrl);
            if (token == null)
            {
                sendSecurityError(servletResponse);
                return;
            }

            BeanConverter converter = getConverterForRequest(servletRequest);

            handleSingleRequest(servletRequest, servletResponse, token, converter);
        }

        protected override void sendError(HttpResponse servletResponse, ResponseItem responseItem)
        {
            int errorCode = responseItem.getErrorCode();
            if (errorCode < 0)
            {
                // Map JSON-RPC error codes into HTTP error codes as best we can
                // TODO: Augment the error message (if missing) with a default
                switch (errorCode)
                {
                    case -32700:
                    case -32602:
                    case -32600:
                        // Parse error, invalid params, and invalid request 
                        errorCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case -32601:
                        // Procedure doesn't exist
                        errorCode = (int)HttpStatusCode.NotImplemented;
                        break;
                    case -32603:
                    default:
                        // Internal server error, or any application-defined error
                        errorCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }
            }
            servletResponse.ContentType = ContentTypes.OUTPUT_TEXT_CONTENT_TYPE;
            servletResponse.StatusCode = responseItem.getErrorCode();
            servletResponse.StatusDescription = responseItem.getErrorMessage();
            servletResponse.Output.Write(responseItem.getErrorMessage());
        }

        private void handleSingleRequest(HttpRequest servletRequest,
                HttpResponse servletResponse, ISecurityToken token, BeanConverter converter)
        {
            RestfulRequestItem requestItem = new RestfulRequestItem(servletRequest, token, converter);
            var asyncResult = HandleRequestItem(requestItem);
            
            // handle just the one case where we can't find a handler
            if (asyncResult == null)
            {
                sendError(servletResponse, new ResponseItem((int)HttpStatusCode.InternalServerError,
                                    "The service " + requestItem.getService() + " is not implemented"));
                return;
            }
            ResponseItem responseItem = getResponseItem(asyncResult);

            servletResponse.ContentType = converter.GetContentType();
            if (responseItem.getErrorCode() >= 200 && responseItem.getErrorCode() < 400) 
            {
                Object resp = responseItem.getResponse();
                // put single object responses into restfulcollection
                if (!(resp is DataCollection) && !(resp is IRestfulCollection))
                {
                    switch (requestItem.getService())
                    {
                        case IHandlerDispatcher.ACTIVITY_ROUTE:
                            if (resp is Activity)
                            {
                                resp = new RestfulCollection<Activity>((Activity)resp);
                            }

                            break;
                        case IHandlerDispatcher.PEOPLE_ROUTE:
                            if (resp is Person)
                            {
                                resp = new RestfulCollection<Person>((Person)resp);
                            }
                            break;
                        case IHandlerDispatcher.APPDATA_ROUTE:
                            resp = new DataCollection(new Dictionary<string, Dictionary<string, string>> { { "entry", (Dictionary<string, string>)resp } });
                            break;
                        default:
                            resp = new Dictionary<string, object> { { "entry", resp } };
                            break;
                    }
                }

                servletResponse.Output.Write(converter.ConvertToString(resp, requestItem));
            }
            else
            {
                sendError(servletResponse, responseItem);
            }
        }

        private BeanConverter getConverterForRequest(HttpRequest servletRequest)
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
                contentType = servletRequest.ContentType;
            }
            catch (Exception t)
            {
                //this happens while testing
                //if (logger.isLoggable(Level.FINE)) {
                //  logger.fine("Unexpected error : content type is null " + t.toString());
                // }
            }


            if (!string.IsNullOrEmpty(contentType))
            {
                if (ContentTypes.ALLOWED_JSON_CONTENT_TYPES.Contains(contentType))
                {
                    converter = jsonConverter;
                }
                else if (ContentTypes.ALLOWED_ATOM_CONTENT_TYPES.Contains(contentType))
                {
                    converter = atomConverter;
                }
                else if (ContentTypes.ALLOWED_XML_CONTENT_TYPES.Contains(contentType))
                {
                    converter = xmlConverter;
                }
                else if (formatString == null)
                {
                    // takes care of cases where content!= null but is ""
                    converter = jsonConverter;
                }
            }
            else if (!string.IsNullOrEmpty(formatString))
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
    }
}
