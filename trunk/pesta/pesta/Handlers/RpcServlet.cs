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
using System.Net;
using Jayrock.Json;
using System.IO;
using System.Text;
using Jayrock.Json.Conversion;

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
    internal class RpcServlet : IHttpHandler
    {
        private static int MAX_REQUEST_SIZE = 1024 * 128;
        private JsonRpcHandler jsonHandler = JsonRpcHandler.Instance;

        public void ProcessRequest(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            int length = request.ContentLength;
            if (length <= 0)
            {
                response.StatusCode = (int)HttpStatusCode.LengthRequired;
                return;
            }
            if (length > MAX_REQUEST_SIZE)
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

            try
            {
                Encoding encoding = request.ContentEncoding;
                JsonObject req = JsonConvert.Import(encoding.GetString(body)) as JsonObject;

                JsonObject resp = jsonHandler.process(req);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "application/json; charset=utf-8";
                response.AddHeader("Content-Disposition", "attachment;filename=rpc.txt");
                response.Output.Write(resp.ToString());
            }
            catch (DecoderFallbackException e)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Output.Write("Unsupported input character set: " + e.Message);

            }
            catch (JsonException e)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Output.Write("Malformed JSON request: " + e.Message);
            }
            catch (RpcException e)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Output.Write(e.Message);
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