﻿#region License, Terms and Conditions
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
using Jayrock.Json;
using Jayrock.Json.Conversion;
using System.IO;
using System.Net;
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
    internal class JsonRpcServlet : ApiServlet, IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            string method = request.HttpMethod;
            if (method == "POST")
            {
                SecurityToken token = getSecurityToken(context);
                if (token == null)
                {
                    sendSecurityError(response);
                    return;
                }

                setCharacterEncodings(request, response);

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
                        dispatchBatch(batch, request, response, token);
                    }
                    else
                    {
                        JsonObject requestObj = JsonConvert.Import(content) as JsonObject;
                        dispatch(requestObj, request, response, token);
                    }
                }
                catch (JsonException je)
                {
                    sendBadRequest(je, response);
                }
            }
            else if (method == "GET")
            {
                SecurityToken token = getSecurityToken(context);
                if (token == null)
                {
                    sendSecurityError(response);
                    return;
                }

                try
                {
                    setCharacterEncodings(request, response);
                    JsonObject requestObj = JsonConversionUtil.fromRequest(request);
                    dispatch(requestObj, request, response, token);
                }
                catch (JsonException je)
                {
                    // FIXME
                }
            }
        }


        protected void dispatchBatch(JsonArray batch, HttpRequest servletRequest,
          HttpResponse servletResponse, SecurityToken token)
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
                responses.Add(handleRequestItem(requestItem, servletRequest));
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
                result.Put(getJSONResponse(key, getResponseItem(responses[i])));
            }
            servletResponse.Output.Write(result.ToString());
        }

        protected void dispatch(JsonObject request, HttpRequest servletRequest,
          HttpResponse servletResponse, SecurityToken token)
        {
            String key = null;
            if (request.Contains("id"))
            {
                key = request["id"] as String;
            }
            RpcRequestItem requestItem = new RpcRequestItem(request, token, jsonConverter);

            // Resolve each Future into a response.
            // TODO: should use shared deadline across each request
            ResponseItem response = getResponseItem(handleRequestItem(requestItem, servletRequest));
            JsonObject result = getJSONResponse(key, response);
            servletResponse.Output.Write(result.ToString());
        }

        private JsonObject getJSONResponse(String key, ResponseItem responseItem)
        {
            JsonObject result = new JsonObject();
            if (key != null)
            {
                result.Put("id", key);
            }
            if (responseItem.getError() != null)
            {
                result.Put("error", getErrorJson(responseItem));
            }
            else
            {
                Object response = responseItem.getResponse();
                JsonObject converted = (JsonObject)jsonConverter.convertToJson(response);

                if (response != null &&
                    response.GetType().IsGenericType &&
                    response.GetType().GetGenericTypeDefinition() == typeof(RestfulCollection<>))
                {
                    // FIXME this is a little hacky because of the field names in the RestfulCollection
                    converted.Put("list", converted.Remove("entry"));
                    result.Put("data", converted);
                }
                else if (response is DataCollection)
                {
                    if (converted.Contains("entry"))
                    {
                        result.Put("data", converted["entry"]);
                    }
                }
                else
                {
                    result.Put("data", converted);
                }
            }
            return result;
        }

        private void sendBadRequest(Exception t, HttpResponse response)
        {
            sendError(response, new ResponseItem(ResponseError.BAD_REQUEST,
                "Invalid batch - " + t.Message));
        }

        private JsonObject getErrorJson(ResponseItem responseItem)
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

        protected override void sendError(HttpResponse response, ResponseItem responseItem)
        {
            try
            {
                JsonObject error = getErrorJson(responseItem);
                response.Output.Write(error.ToString());
            }
            catch (JsonException je)
            {
                // This really shouldn't ever happen
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.StatusDescription = "Error generating error response " + je.Message;
            }
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