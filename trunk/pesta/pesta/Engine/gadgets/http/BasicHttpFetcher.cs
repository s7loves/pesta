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


namespace Pesta.Engine.gadgets.http
{
    /// <summary>
    /// Summary description for BasicHttpFetcher
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class BasicHttpFetcher : IHttpFetcher
    {
          private const int CONNECT_TIMEOUT_MS = 5000;
          private const int READ_TIMEOUT_MS = 20000;

        /**
         * Creates a new fetcher for fetching HTTP objects.  Not really suitable
         * for production use.  Someone should probably go and implement maxObjSize,
         * for one thing.  Use of an HTTP proxy for security is also necessary
         * for production deployment.
         *
         * @param maxObjSize Maximum size, in bytes, of object to fetch.  Except this
         * isn't actually implemented.
         */
        public readonly static BasicHttpFetcher Instance = new BasicHttpFetcher();
        protected BasicHttpFetcher()
        {
        }

        public sResponse fetch(sRequest request)
        {
            try
            {
                return makeResponse(request.req);
            }
            catch (Exception)
            {
                return sResponse.error();
            }
            
        }

        private sResponse makeResponse(WebRequest fetcher)
        {
            HttpWebResponse resp;
            fetcher.Timeout = CONNECT_TIMEOUT_MS;
            try
            {
                resp = (HttpWebResponse)fetcher.GetResponse();
            }
            catch (WebException ex)
            {
                resp = (HttpWebResponse)ex.Response;
            }
            int responseCode = (int)HttpStatusCode.GatewayTimeout;
            MemoryStream memoryStream = new MemoryStream(0x10000);
            WebHeaderCollection headers;
            if (resp != null)
            {
                headers = resp.Headers;
                responseCode = (int)resp.StatusCode;
                byte[] buffer = new byte[0x1000];
                using (Stream responseStream = resp.GetResponseStream())
                {
                    int bytes;
                    responseStream.ReadTimeout = READ_TIMEOUT_MS;
                    while ((bytes = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytes);
                    }
                    memoryStream.Close();
                }
            }
            else
            {
                headers = new WebHeaderCollection(); 
            }
            
            byte[] body = memoryStream.ToArray();
            return new HttpResponseBuilder()
                .setHttpStatusCode(responseCode)
                .setResponse(body)
                .addAllHeaders(headers)
                .create();
        }
    }
}