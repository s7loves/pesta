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
using System.Web;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using Pesta.Engine.auth;
using Pesta.Engine.social.model;
using pestaServer.ActionFilters;
using pestaServer.Models.social.service;
using Pesta.Engine.social.spi;

namespace pestaServer.Controllers
{
    public class restController : apiController
    {
        protected static readonly String FORMAT_PARAM = "format";
        protected static readonly String ATOM_FORMAT = "atom";
        protected static readonly String XML_FORMAT = "xml";
        protected static readonly String JSON_BATCH_ROUTE = "jsonBatch";

        [CompressFilter]
        public void Index(string id1, string id2, string id3, string id4)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpResponse response = System.Web.HttpContext.Current.Response;
            ISecurityToken token = getSecurityToken(System.Web.HttpContext.Current); // BasicSecurityToken
            if (token == null)
            {
                sendSecurityError(response);
                return;
            }
            BeanConverter converter = getConverterForRequest(request);
            if (id1 == JSON_BATCH_ROUTE)
            {
                handleBatchRequest(request, response, token, converter);
            }
            else
            {
                handleSingleRequest(request, response, token, converter);
            }
        }
        private void handleSingleRequest(HttpRequest request, HttpResponse response, ISecurityToken token, BeanConverter converter)
        {
            RestfulRequestItem requestItem = new RestfulRequestItem(request, token, converter);
            ResponseItem responseItem = getResponseItem(handleRequestItem(requestItem));

            response.ContentType = converter.getContentType();
            if (responseItem != null && responseItem.getError() == null)
            {
                Object resp = responseItem.getResponse();
                // put single object responses into restfulcollection
                if (!(resp is DataCollection) && !(resp is IRestfulCollection))
                {
                    switch (requestItem.getService())
                    {
                        case IHandlerDispatcher.ACTIVITY_ROUTE:
                            if(resp is Activity)
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
                            resp = new DataCollection(new Dictionary<string, Dictionary<string,string>>{{"entry",(Dictionary<string,string>)resp}});
                            break;
                        default:
                            resp = new Dictionary<string, object>() {{"entry", resp}};
                            break;
                    }
                }
                response.Output.Write(converter.convertToString(resp, requestItem));
            }
            else
            {
                sendError(response, responseItem);
            }
        }

        private void handleBatchRequest(HttpRequest request, HttpResponse response, ISecurityToken token, BeanConverter converter)
        {
            RestfulRequestItem mainRequest = new RestfulRequestItem(request, token, converter);
            Dictionary<String, object> responses = new Dictionary<string, object>();
            Dictionary<String, String> requests = (Dictionary<String, String>)mainRequest.getTypedParameters(typeof(Dictionary<String, String>));
            foreach (var entry in requests)
            {
                string key = entry.Key;
                JsonObject req = (JsonObject)JsonConvert.Import(entry.Value);
                RestfulRequestItem requestItem = new RestfulRequestItem(req["url"].ToString(), req["method"].ToString(),"", token, converter);
                responses.Add(key, getResponseItem(handleRequestItem(requestItem)).getResponse());
            }
            response.Output.Write(converter.convertToString(new Dictionary<string, object> { { "error", false }, { "responses", responses } }, mainRequest));
        }

        BeanConverter getConverterForRequest(HttpRequest servletRequest)
        {
            String formatString = null;
            BeanConverter converter = null;
            String contentType = null;

            try
            {
                formatString = servletRequest.Params[FORMAT_PARAM];
                contentType = servletRequest.Headers[IHandlerDispatcher.CONTENT_TYPE];
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
    }
}
