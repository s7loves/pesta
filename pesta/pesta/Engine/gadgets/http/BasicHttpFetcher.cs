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
using System.Data;
using System.Configuration;
using System.IO;
using System.Web;
using System.Net;


namespace Pesta
{
    /// <summary>
    /// Summary description for BasicHttpFetcher
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public sealed class BasicHttpFetcher : HttpFetcher
    {
        /**
         * Creates a new fetcher for fetching HTTP objects.  Not really suitable
         * for production use.  Someone should probably go and implement maxObjSize,
         * for one thing.  Use of an HTTP proxy for security is also necessary
         * for production deployment.
         *
         * @param maxObjSize Maximum size, in bytes, of object to fetch.  Except this
         * isn't actually implemented.
         */
        readonly static BasicHttpFetcher instance = new BasicHttpFetcher();
        static BasicHttpFetcher()
        {

        }
        BasicHttpFetcher()
        {
        }

        public static BasicHttpFetcher Instance
        {
            get
            {
                return instance;
            }
        }


        public sResponse fetch(sRequest request)
        {
            HttpCacheKey cacheKey = new HttpCacheKey(request);
            sResponse response = HttpCache.getResponse(cacheKey, request);
            if (response != null)
            {
                return response;
            }
            WebRequest fetcher = WebRequest.Create(request.req.RequestUri);
            if (request.req.ContentLength > 0)
            {
                fetcher.ContentLength = request.req.ContentLength;
                using (StreamReader reader = new StreamReader(request.req.GetRequestStream()))
                {
                    StreamWriter writer = new StreamWriter(fetcher.GetRequestStream());
                    writer.Write(reader.ReadToEnd());
                    writer.Close();
                }
            }
            response = makeResponse(fetcher);
            return HttpCache.addResponse(cacheKey, request, response);
        }

        private sResponse makeResponse(WebRequest fetcher)
        {
            HttpWebResponse resp = null;
            sResponse response = null;
            try
            {
                resp = (HttpWebResponse)fetcher.GetResponse();
            }
            catch (WebException ex)
            {
                resp = (HttpWebResponse)((WebException)ex).Response;
                if (resp == null)
                {
                    throw ex;
                }
            }
            finally
            {
                response = new sResponse(resp);
                resp.Close();
            }
            return response;
        }
    } 
}