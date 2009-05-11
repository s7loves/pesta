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
using System.IO;
using System.Net;
using System.Web;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using Pesta.Engine.auth;
using Pesta.Engine.common.util;
using Pesta.Engine.social;
using Pesta.Engine.social.spi;
using pestaServer.ActionFilters;
using pestaServer.Models.social.core.util;
using pestaServer.Models.social.service;

namespace pestaServer.Controllers
{
    public class RpcController : ApiController
    {
        //
        // GET: /rpc/
        [CompressFilter]
        public void Index()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpResponse response = System.Web.HttpContext.Current.Response;
            string method = request.HttpMethod;
            if (method == "POST")
            {
                ISecurityToken token = GetSecurityToken(System.Web.HttpContext.Current);
                if (token == null)
                {
                    SendSecurityError(response);
                    return;
                }

                setCharacterEncodings(request, response);
                response.ContentType = "application/json";

                try
                {
                    String content;
                    using (StreamReader reader = new StreamReader(request.InputStream))
                    {
                        content = reader.ReadToEnd();
                    }

                    if ((content.IndexOf('[') != -1) && content.IndexOf('[') < content.IndexOf('{'))
                    {
                        // Is a batch
                        JsonArray batch = JsonConvert.Import(content) as JsonArray;
                        DispatchBatch(batch, response, token);
                    }
                    else
                    {
                        JsonObject requestObj = JsonConvert.Import(content) as JsonObject;
                        Dispatch(requestObj, response, token);
                    }
                }
                catch (JsonException je)
                {
                    SendBadRequest(je, response);
                }
            }
            else if (method == "GET")
            {
                ISecurityToken token = GetSecurityToken(System.Web.HttpContext.Current);
                if (token == null)
                {
                    SendSecurityError(response);
                    return;
                }
                
                setCharacterEncodings(request, response);
                JsonObject requestObj = JsonConversionUtil.FromRequest(request);
                Dispatch(requestObj, response, token);
            }
        }

        private void DispatchBatch(JsonArray batch, HttpResponse servletResponse, ISecurityToken token)
        {
            // Use linked hash map to preserve order
            List<IAsyncResult> responses = new List<IAsyncResult>(batch.Length);

            // Gather all Futures.  We do this up front so that
            // the first call to get() comes after all futures are created,
            // which allows for implementations that batch multiple Futures
            // into single requests.
            for (int i = 0; i < batch.Length; i++)
            {
                JsonObject batchObj = batch.GetObject(i);
                RpcRequestItem requestItem = new RpcRequestItem(batchObj, token, jsonConverter);
                responses.Add(HandleRequestItem(requestItem));
            }

            // Resolve each Future into a response.
            // TODO: should use shared deadline across each request
            JsonArray result = new JsonArray();
            for (int i = 0; i < batch.Length; i++)
            {
                JsonObject batchObj = batch.GetObject(i);
                String key = null;
                if (batchObj.Contains("id"))
                {
                    key = batchObj["id"] as String;
                }
                result.Put(GetJsonResponse(key, GetResponseItem(responses[i])));
            }
            servletResponse.Output.Write(result.ToString());
        }

        private void Dispatch(JsonObject request, HttpResponse servletResponse, ISecurityToken token)
        {
            String key = null;
            if (request.Contains("id"))
            {
                key = request["id"] as String;
            }
            RpcRequestItem requestItem = new RpcRequestItem(request, token, jsonConverter);

            // Resolve each Future into a response.
            // TODO: should use shared deadline across each request
            ResponseItem response = GetResponseItem(HandleRequestItem(requestItem));
            JsonObject result = GetJsonResponse(key, response);

            servletResponse.Output.Write(result.ToString());
        }

        private static JsonObject GetJsonResponse(String key, ResponseItem responseItem)
        {
            JsonObject result = new JsonObject();
            if (key != null)
            {
                result.Put("id", key);
            }
            if (responseItem.getError() != null)
            {
                result.Put("error", GetErrorJson(responseItem));
            }
            else
            {
                Object response = responseItem.getResponse();
                JsonObject converted = (JsonObject)BeanJsonConverter.ConvertToJson(response);

                if (response is DataCollection)
                {
                    if (converted.Contains("entry"))
                    {
                        result.Put("data", converted["entry"]);
                    }
                }
                else if (response != null &&
                    response.GetType().IsGenericType &&
                    response.GetType().GetGenericTypeDefinition() == typeof(RestfulCollection<>))
                {
                    JsonObject map = new JsonObject();
                    IRestfulCollection collection = (IRestfulCollection) response;
                    map.Put("startIndex", collection.getStartIndex());
                    map.Put("totalResults", collection.getTotalResults());
                    map.Put("list", converted["entry"]);
                    result.Put("data", map);
                }
                else
                {
                    result.Put("data", converted);
                }
            }
            return result;
        }

        private void SendBadRequest(Exception t, HttpResponse response)
        {
            SendError(response, new ResponseItem(ResponseError.BAD_REQUEST,
                                                 "Invalid batch - " + t.Message));
        }

        private static JsonObject GetErrorJson(ResponseItem responseItem)
        {
            JsonObject error = new JsonObject();
            error.Put("code", responseItem.getError().getHttpErrorCode());

            String message = responseItem.getError().ToString();
            if (!String.IsNullOrEmpty(responseItem.getErrorMessage()))
            {
                message += ": " + responseItem.getErrorMessage();
            }
            error.Put("message", message);
            return error;
        }

        protected override void SendError(HttpResponse response, ResponseItem responseItem)
        {
            try
            {
                JsonObject error = GetErrorJson(responseItem);
                response.Output.Write(error.ToString());
            }
            catch (JsonException je)
            {
                // This really shouldn't ever happen
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.StatusDescription = "Error generating error response " + je.Message;
            }
        }
    }
}
