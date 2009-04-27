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
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using pestaServer.ActionFilters;
using pestaServer.Models.gadgets.servlet;

namespace pestaServer.Controllers
{
    public class metadataController : Controller
    {
        private const int POST_REQUEST_MAX_SIZE = 1024 * 128;
        private const string GET_REQUEST_REQ_PARAM = "req";
        private const string GET_REQUEST_CALLBACK_PARAM = "callback";
        private static readonly Regex GET_REQUEST_CALLBACK_PATTERN = new Regex("[A-Za-z_\\.]+", RegexOptions.Compiled);
        private readonly JsonRpcHandler jsonHandler = JsonRpcHandler.Instance;

        [CompressFilter]
        public void Index()
        {
            HttpRequest request = System.Web.HttpContext.Current.Request;
            HttpResponse response = System.Web.HttpContext.Current.Response;

            if (request.HttpMethod == "GET")
            {
                String reqValue;
                String callbackValue;

                try
                {
                    reqValue = validateParameterValue(request, GET_REQUEST_REQ_PARAM);
                    callbackValue = validateParameterValue(request, GET_REQUEST_CALLBACK_PARAM);
                    if (!GET_REQUEST_CALLBACK_PATTERN.Match(callbackValue).Success)
                    {
                        throw new Exception("Wrong format for parameter '" +
                                            GET_REQUEST_CALLBACK_PARAM + "' specified. Expected: " +
                                            GET_REQUEST_CALLBACK_PATTERN);
                    }

                }
                catch
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                Result result = process(request, response, Encoding.UTF8.GetBytes(reqValue));
                response.Output.Write(result.isSuccess()
                                          ? callbackValue + "(" + result.getOutput() + ")"
                                          : result.getOutput());
            }
            else if (request.HttpMethod == "POST")
            {
                int length = request.ContentLength;
                if (length <= 0)
                {
                    response.StatusCode = (int)HttpStatusCode.LengthRequired;
                    return;
                }
                if (length > POST_REQUEST_MAX_SIZE)
                {
                    response.StatusCode = (int)HttpStatusCode.RequestEntityTooLarge;
                    return;
                }

                MemoryStream memoryStream = new MemoryStream(0x10000);
                byte[] buffer = new byte[0x1000];
                int bytes;
                while ((bytes = request.InputStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytes);
                }
                memoryStream.Close();
                byte[] body = memoryStream.ToArray();
                if (body.Length != length)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                Result result = process(request, response, body);
                response.Output.Write(result.getOutput());
                response.End();
            }
        }

        private String validateParameterValue(HttpRequest request, String parameter)
        {
            String result = request.Params[parameter];
            if (result == null)
            {
                throw new Exception("No parameter '" + parameter + "' specified.");
            }
            return result;
        }

        private Result process(HttpRequest request, HttpResponse response, byte[] body)
        {
            try
            {
                Encoding encoding = getRequestCharacterEncoding(request);
                JsonObject req = JsonConvert.Import(HttpUtility.UrlDecode(encoding.GetString(body))) as JsonObject;
                JsonObject resp = jsonHandler.process(req);
                response.StatusCode = (int)HttpStatusCode.OK;
                // charset should match encoding?
                response.ContentType = "application/json; charset=utf-8";
                response.AddHeader("Content-Disposition", "attachment;filename=rpc.txt");
                return new Result(resp.ToString(), true);
            }
            catch (JsonException e)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new Result("Malformed JSON request: " + e.Message, false);
            }
            catch (RpcException e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return new Result(e.Message, false);
            }
            catch (Exception e)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new Result("Bad request: " + e.Message, false);
            }
        }

        private Encoding getRequestCharacterEncoding(HttpRequest request)
        {
            String encoding = request.ContentEncoding.WebName ?? "UTF-8";

            return Encoding.GetEncoding(encoding);
        }

        private struct Result
        {
            private readonly String output;
            private readonly bool success;

            public Result(String output, bool success)
            {
                this.output = output;
                this.success = success;
            }

            public String getOutput()
            {
                return output;
            }

            public bool isSuccess()
            {
                return success;
            }
        }
    }
}
