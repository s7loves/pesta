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
using Pesta.Engine.protocol;
using Pesta.Engine.social.spi;
using pestaServer.ActionFilters;
using pestaServer.Models.social.service;

namespace pestaServer.Controllers
{
    public class rpcController : ApiController
    {
        private static readonly String REQUEST_PARAM = "request";

        private static readonly int SC_JSON_PARSE_ERROR = -32700;

        [CompressFilter]
        [AuthenticationFilter]
        public void Index()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpResponse response = System.Web.HttpContext.Current.Response;
            string method = request.HttpMethod;

            if (method == "POST")
            {
                ISecurityToken token = getSecurityToken(System.Web.HttpContext.Current, Request.RawUrl);
                if (token == null)
                {
                    sendSecurityError(response);
                    return;
                }

                setCharacterEncodings(request, response);
                Response.ContentType = "application/json";

                try
                {
                    String content;
                    using (StreamReader reader = new StreamReader(Request.InputStream))
                    {
                        content = reader.ReadToEnd();
                    }

                    if ((content.IndexOf('[') != -1) && content.IndexOf('[') < content.IndexOf('{'))
                    {
                        // Is a batch
                        JsonArray batch = JsonConvert.Import(content) as JsonArray;
                        dispatchBatch(batch, response, token);
                    }
                    else
                    {
                        JsonObject requestObj = JsonConvert.Import(content) as JsonObject;
                        dispatch(requestObj, response, token);
                    }
                }
                catch (Exception je)
                {
                    sendBadRequest(je, response);
                }
            }
            else if (method == "GET")
            {
                ISecurityToken token = getSecurityToken(System.Web.HttpContext.Current, Request.RawUrl);
                if (token == null)
                {
                    sendSecurityError(response);
                    return;
                }

                setCharacterEncodings(request, response);
                JsonObject requestObj = JsonConversionUtil.FromRequest(request);
                dispatch(requestObj, response, token);
            }
        }

        protected void dispatchBatch(JsonArray batch, HttpResponse response, ISecurityToken token)
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
            List<Object> result = new List<object>();
            for (int i = 0; i < batch.Length; i++)
            {
                JsonObject batchObj = batch.GetObject(i);
                String key = null;
                if (batchObj.Contains("id"))
                {
                    key = batchObj["id"] as String;
                }
                result.Add(getJsonResponse(key, getResponseItem(responses[i])));
            }
            response.Output.Write(jsonConverter.ConvertToString(result));
        }

        private void dispatch(JsonObject request, HttpResponse servletResponse, ISecurityToken token)
        {
            String key = null;
            if (request.Contains("id"))
            {
                key = request["id"] as String;
            }
            RpcRequestItem requestItem = new RpcRequestItem(request, token, jsonConverter);

            // Resolve each Future into a response.
            // TODO: should use shared deadline across each request
            ResponseItem response = getResponseItem(HandleRequestItem(requestItem));
            Object result = getJsonResponse(key, response);

            servletResponse.Output.Write(jsonConverter.ConvertToString(result));
        }

        Object getJsonResponse(String key, ResponseItem responseItem)
        {
            var result = new Dictionary<string, object>();
            if (key != null)
            {
                result.Add("id", key);
            }
            if (responseItem.getErrorCode() < 200 ||
                    responseItem.getErrorCode() >= 400)
            {
                result.Add("error", getErrorJson(responseItem));
            }
            else
            {
                Object response = responseItem.getResponse();
                if (response != null &&
                    response is DataCollection)
                {
                    result.Add("data", ((DataCollection)response).entry);
                }
                else if (response != null && (response is IRestfulCollection))
                {
                    var map = new Dictionary<string, object>();
                    var collection = (IRestfulCollection)response;
                    map.Add("startIndex", collection.startIndex);
                    map.Add("totalResults", collection.totalResults);
                    map.Add("list", collection.getEntry());
                    result.Add("data", map);
                }
                else
                {
                    result.Add("data", response);
                }
                
            }
            return result;
        }

        private static readonly Dictionary<int, String> errorTitles = new Dictionary<int, string>
                    {
                        {(int)HttpStatusCode.NotImplemented, "notImplemented"},
                        {(int)HttpStatusCode.Unauthorized, "unauthorized"},
                        {(int)HttpStatusCode.Forbidden, "forbidden"},
                        {(int)HttpStatusCode.BadRequest, "badRequest"},
                        {(int)HttpStatusCode.InternalServerError, "internalError"},
                        {(int)HttpStatusCode.ExpectationFailed, "limitExceeded"}
                    };

        private Object getErrorJson(ResponseItem responseItem)
        {
            Dictionary<String, Object> error = new Dictionary<String, Object>();
            error.Add("code", responseItem.getErrorCode());

            String message = errorTitles[responseItem.getErrorCode()];
            if (message == null)
            {
                message = responseItem.getErrorMessage();
            }
            else
            {
                if (!string.IsNullOrEmpty(responseItem.getErrorMessage()))
                {
                    message += ": " + responseItem.getErrorMessage();
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                error.Add("message", message);
            }

            if (responseItem.getResponse() != null)
            {
                error.Add("data", responseItem.getResponse());
            }

            return error;
        }

        protected override void sendError(HttpResponse servletResponse, ResponseItem responseItem)
        {
            try
            {
                Object error = getErrorJson(responseItem);
                Response.Output.Write(error.ToString());
            }
            catch (JsonException je)
            {
                // This really shouldn't ever happen
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                Response.StatusDescription = "Error generating error response " + je.Message;
            }
        }

        private void sendBadRequest(Exception t, HttpResponse response)
        {
            sendError(response, new ResponseItem((int)HttpStatusCode.BadRequest,
                "Invalid batch - " + t.Message));
        }

        private void sendJsonParseError(JsonException e, HttpResponse response)
        {
            sendError(response, new ResponseItem(SC_JSON_PARSE_ERROR,
                "Invalid JSON - " + e.Message));
        }
    }


}
