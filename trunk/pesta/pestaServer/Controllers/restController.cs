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
    public class restController : ApiController
    {
        private const String FORMAT_PARAM = "format";
        private const String ATOM_FORMAT = "atom";
        private const String XML_FORMAT = "xml";
        private const String JSON_BATCH_ROUTE = "jsonBatch";

        [CompressFilter]
        public void Index(string id1, string id2, string id3, string id4)
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpResponse response = System.Web.HttpContext.Current.Response;
            ISecurityToken token = GetSecurityToken(System.Web.HttpContext.Current); // BasicSecurityToken
            if (token == null)
            {
                SendSecurityError(response);
                return;
            }
            BeanConverter converter = GetConverterForRequest(request);
            if (id1 == JSON_BATCH_ROUTE)
            {
                HandleBatchRequest(request, response, token, converter);
            }
            else
            {
                HandleSingleRequest(request, response, token, converter);
            }
        }
        private void HandleSingleRequest(HttpRequest request, HttpResponse response, ISecurityToken token, BeanConverter converter)
        {
            RestfulRequestItem requestItem = new RestfulRequestItem(request, token, converter);
            ResponseItem responseItem = GetResponseItem(HandleRequestItem(requestItem));

            response.ContentType = converter.GetContentType();
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
                            resp = new Dictionary<string, object> {{"entry", resp}};
                            break;
                    }
                }
                
                response.Output.Write(converter.ConvertToString(resp, requestItem));
            }
            else
            {
                SendError(response, responseItem);
            }
        }

        private void HandleBatchRequest(HttpRequest request, HttpResponse response, ISecurityToken token, BeanConverter converter)
        {
            RestfulRequestItem mainRequest = new RestfulRequestItem(request, token, converter);
            Dictionary<String, object> responses = new Dictionary<string, object>();
            Dictionary<String, String> requests = (Dictionary<String, String>)mainRequest.getTypedParameters(typeof(Dictionary<String, String>));
            foreach (var entry in requests)
            {
                string key = entry.Key;
                JsonObject req = (JsonObject)JsonConvert.Import(entry.Value);
                RestfulRequestItem requestItem = new RestfulRequestItem(req["url"].ToString(), req["method"].ToString(),"", token, converter);
                responses.Add(key, GetResponseItem(HandleRequestItem(requestItem)).getResponse());
            }
            response.Output.Write(converter.ConvertToString(new Dictionary<string, object> { { "error", false }, { "responses", responses } }, mainRequest));
        }

        BeanConverter GetConverterForRequest(HttpRequest servletRequest)
        {
            BeanConverter converter = null;

            string formatString = servletRequest.Params[FORMAT_PARAM];
            string contentType = servletRequest.Headers[IHandlerDispatcher.CONTENT_TYPE];

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

        protected override void SendError(HttpResponse response, ResponseItem responseItem)
        {
            response.StatusCode = responseItem.getError().getHttpErrorCode();
            response.StatusDescription = responseItem.getErrorMessage();
        }
    }
}
