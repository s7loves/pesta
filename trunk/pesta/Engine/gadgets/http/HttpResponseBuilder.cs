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
using System.Data;
using System.Configuration;
using System.Collections.Specialized;
using System.Web;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace Pesta
{
    /// <summary>
    /// Summary description for HttpResponseBuilder
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class HttpResponseBuilder
    {
        private HttpStatusCode httpStatusCode = HttpStatusCode.OK;
        private NameValueCollection headers = new NameValueCollection();
        private byte[] responseBytes;
        private Dictionary<string, string> metadata = new Dictionary<string, string>();

        public HttpResponseBuilder() { }

        public HttpResponseBuilder(HttpResponseBuilder builder)
        {
            httpStatusCode = builder.httpStatusCode;
            headers.Add(builder.headers);
            foreach (var item in builder.metadata)
            {
                metadata.Add(item.Key, item.Value);
            }

            responseBytes = builder.responseBytes;
        }

        public HttpResponseBuilder(sResponse response)
        {
            httpStatusCode = response.getHttpStatusCode();
            headers.Add(response.getHeaders());
            foreach (var item in response.getMetadata())
            {
                this.metadata.Add(item.Key, item.Value);
            }
            responseBytes = response.responseBytes;
        }

        /**
         * @param responseString The response string.  Converted to UTF-8 bytes and copied when set.
         */
        public HttpResponseBuilder setResponseString(String body)
        {
            responseBytes = Encoding.UTF8.GetBytes(body);
            return this;
        }

        /**   
         * @param responseBytes The response body. Copied when set.
         */
        public HttpResponseBuilder setResponse(byte[] responseBytes)
        {
            if (responseBytes != null)
            {
                this.responseBytes = new byte[responseBytes.Length];
                Array.Copy(responseBytes, 0, this.responseBytes, 0, responseBytes.Length);
            }
            return this;
        }

        /**
         * @param httpStatusCode The HTTP response status, defined on HttpResponse.
         */
        public HttpResponseBuilder setHttpStatusCode(HttpStatusCode httpStatusCode)
        {
            this.httpStatusCode = httpStatusCode;
            return this;
        }

        /**
         * Sets a single header value, overwriting any previously set headers with the same name.
         */
        public HttpResponseBuilder setHeader(String name, String value)
        {
            if (name != null)
            {
                headers.Add(name, value);
            }
            return this;
        }

        /**
         * Adds an entire map of headers to the response.
         */
        public HttpResponseBuilder addHeaders(NameValueCollection headers)
        {
            this.headers.Add(headers);
            return this;
        }

        /**
         * Remove all headers with the given name from the response.
         *
         * @return Any values that were removed from the response.
         */
        public void removeHeader(String name)
        {
            headers.Remove(name);
        }

        /**
        * @param cacheTtl The time to live for this response, in seconds.
        */
        public HttpResponseBuilder setCacheTtl(int cacheTtl)
        {
            headers.Remove("Pragma");
            headers.Remove("Expires");
            headers.Add("Cache-Control", "public,max-age=" + cacheTtl);
            return this;
        }

        /**
         * @param expriationTime The expiration time for this response, in seconds.
         */
        public HttpResponseBuilder setExpirationTime(TimeSpan expriationTime)
        {
            headers.Remove("Cache-Control");
            headers.Remove("Pragma");
            headers.Add("Expires", DateTime.Now.AddSeconds(expriationTime.TotalSeconds).ToString());
            return this;
        }

        /**
         * Sets cache-control headers indicating the response is not cacheable.
         */
        public HttpResponseBuilder setStrictNoCache()
        {
            headers.Add("Cache-Control", "no-cache");
            headers.Add("Pragma", "no-cache");
            headers.Remove("Expires");
            return this;
        }

        /**
         * Adds a new piece of metadata to the response.
         */
        public HttpResponseBuilder setMetadata(String key, String value)
        {
            metadata.Add(key, value);
            return this;
        }

        /**
         * Merges the given Map of metadata into the existing metadata.
         */
        public HttpResponseBuilder setMetadata(IDictionary<string, string> metadata)
        {
            foreach (var item in metadata)
            {
                this.metadata.Add(item.Key, item.Value);
            }
            return this;
        }

        private NameValueCollection getHeaders()
        {
            return headers;
        }

        private Dictionary<string, string> getMetadata()
        {
            return metadata;
        }

        private byte[] getResponse()
        {
            return responseBytes;
        }

        private HttpStatusCode getHttpStatusCode()
        {
            return httpStatusCode;
        }
    } 
}
