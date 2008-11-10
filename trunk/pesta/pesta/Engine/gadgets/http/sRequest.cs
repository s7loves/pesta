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
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Net;
using System;
using System.Web;
using System.Net.Cache;


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
    public class sRequest
    {
        internal const String DEFAULT_CONTENT_TYPE = "application/x-www-form-urlencoded; charset=utf-8";
        internal const int CONNECT_TIMEOUT_MS = 10000;
        public HttpWebRequest req;
        private Uri uri;
        // TODO: It might be useful to refactor these into a simple map of objects
        // and use sub classes
        // for more detailed data.

        // Cache control.
        private bool ignoreCache;
        private int cacheTtl;
        private byte[] postBody;
        // Whether to follow redirects
        private bool followRedirects;

        // Context for the request.
        private Uri gadget;
        private String container;
        private string method;

        // For signed fetch & OAuth
        private SecurityToken securityToken;
        private OAuthArguments oauthArguments;
        private AuthType authType;

        private String rewriteMimeType;

        /// <summary>
        /// Construct a new request for the given uri.
        /// </summary>
        ///
        public sRequest(Uri uri)
        {
            this.req = (HttpWebRequest)WebRequest.Create(uri.ToString());
            //this.req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            //this.req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            this.req.KeepAlive = true;
            //this.req.Timeout = CONNECT_TIMEOUT_MS;
            this.container = ContainerConfig.DEFAULT_CONTAINER;
            this.followRedirects = true;
            this.req.AllowAutoRedirect = true;
            this.cacheTtl = -1;
            this.uri = uri;
            authType = AuthType.NONE;
        }

        /// <summary>
        /// Clone an existing HttpRequest.
        /// </summary>
        ///
        public sRequest(sRequest srequest)
        {
            this.req = srequest.req;
            uri = srequest.uri;
            ignoreCache = srequest.ignoreCache;
            cacheTtl = srequest.cacheTtl;
            gadget = srequest.gadget;
            container = srequest.container;
            securityToken = srequest.securityToken;
            Array.Copy(srequest.postBody, this.postBody, srequest.postBody.Length);
            if (srequest.oauthArguments != null)
            {
                oauthArguments = new OAuthArguments(srequest.oauthArguments);
            }
            authType = srequest.authType;
            rewriteMimeType = srequest.rewriteMimeType;
            followRedirects = srequest.followRedirects;
        }

        /// <param name="ignoreCache_0">Whether to ignore all caching for this request.</param>
        public sRequest SetIgnoreCache(bool ignoreCache)
        {
            this.ignoreCache = ignoreCache;
            if (ignoreCache)
            {
                // Bypass any proxy caches as well.
                this.req.Headers["Pragma"] = "no-cache";
            }
            return this;
        }

        public sRequest setPostBody(byte[] postBody)
        {
            if (postBody != null)
            {
                this.postBody = new byte[postBody.Length];
                Array.Copy(postBody, this.postBody, postBody.Length);
            }
            return this;
        }

        public sRequest setMethod(String method)
        {
            this.method = method;
            return this;
        }


        /// <param name="cacheTtl_0">The amount of time to cache the result object for, in</param>
        public sRequest SetCacheTtl(int cacheTtl_0)
        {
            this.cacheTtl = cacheTtl_0;
            return this;
        }


        /// <param name="gadget_0">The gadget that caused this HTTP request to be necessary. May</param>
        public sRequest SetGadget(Uri gadget_0)
        {
            this.gadget = gadget_0;
            return this;
        }

        public void setHeader(string name, string value)
        {
            this.req.Headers[name] = value;
        }

        /// <param name="container_0">The container that this request originated from.</param>
        public sRequest SetContainer(String container_0)
        {
            this.container = container_0;
            return this;
        }

        /// <summary>
        /// Assign the security token to use for making any form of authenticated
        /// request.
        /// </summary>
        ///
        public sRequest SetSecurityToken(SecurityToken securityToken_0)
        {
            this.securityToken = securityToken_0;
            return this;
        }


        /// <param name="oauthArguments_0">arguments for OAuth/signed fetched</param>
        public sRequest SetOAuthArguments(OAuthArguments oauthArguments_0)
        {
            this.oauthArguments = oauthArguments_0;
            return this;
        }


        /// <param name="followRedirects_0">whether this request should automatically follow redirects.</param>
        public sRequest SetFollowRedirects(bool followRedirects_0)
        {
            this.followRedirects = followRedirects_0;
            return this;
        }


        /// <param name="authType_0">The type of authentication being used for this request.</param>
        public sRequest SetAuthType(AuthType authType_0)
        {
            this.authType = authType_0;
            return this;
        }


        /// <param name="rewriteMimeType_0">The assumed content type of the response to be rewritten.</param>
        public sRequest SetRewriteMimeType(String rewriteMimeType)
        {
            this.rewriteMimeType = rewriteMimeType;
            return this;
        }

        public void addHeader(string name, string value)
        {
            this.req.Headers.Add(name, value);
        }
        /// <param name="name">The header to fetch</param>
        /// <returns>A list of headers with that name (may be empty).</returns>
        public string[] getHeaders(String name)
        {
            return req.Headers.GetValues(name);
        }


        /// <returns>The first set header with the given name or null if not set. If</returns>
        public string getHeader(String name)
        {
            string[] entry = getHeaders(name);
            if (entry == null)
            {
                return null;
            }
            return entry[0];
        }
        public string getMethod()
        {
            return this.req.Method;
        }
        /**
       * @return The post body as a string, assuming UTF-8 encoding.
       * TODO: We should probably tolerate other encodings, based on the
       *     Content-Type header.
       */
        public String getPostBodyAsString()
        {
            return Encoding.UTF8.GetString(postBody);
        }
        public int getPostBodyLength()
        {
            return this.postBody.Length;
        }
        public byte[] getPostBody()
        {
            return postBody;
        }
        public string ContentType
        {
            get
            {
                string type = getHeader("Content-Type");
                if (type == null)
                {
                    return DEFAULT_CONTENT_TYPE;
                }
                return type;
            }
        }


        public bool IgnoreCache
        {
            get
            {
                return ignoreCache;
            }
        }


        public int CacheTtl
        {
            get
            {
                return cacheTtl;
            }
            set
            {
                cacheTtl = value;
            }
        }

        public Uri Uri
        {
            get
            {
                return uri;
            }
            set
            {
                uri = value;
            }
        }


        public Uri Gadget
        {
            get
            {
                return gadget;
            }
            set
            {
                gadget = value;
            }
        }


        public String Container
        {
            get
            {
                return container;
            }
            set
            {
                container = value;
            }
        }


        public SecurityToken SecurityToken
        {
            get
            {
                return securityToken;
            }
            set
            {
                securityToken = value;
            }
        }


        public OAuthArguments OAuthArguments
        {
            get
            {
                return oauthArguments;
            }
            set
            {
                oauthArguments = value;
            }
        }


        public bool FollowRedirects
        {
            get
            {
                return followRedirects;
            }
            set
            {
                followRedirects = value;
            }
        }


        public AuthType AuthType
        {
            get
            {
                return authType;
            }
            set
            {
                authType = value;
            }
        }


        public String RewriteMimeType
        {
            get
            {
                return rewriteMimeType;
            }
            set
            {
                rewriteMimeType = value;
            }
        }


        public override String ToString()
        {
            StringBuilder buf = new StringBuilder(req.Method);
            buf.Append(' ').Append(req.RequestUri.PathAndQuery).Append("\n\n");
            buf.Append("Host: ").Append(req.RequestUri.Authority).Append('\n');
            buf.Append("X-Shindig-AuthType: ").Append(authType.ToString()).Append('\n');
            for (int i = 0; i < req.Headers.Count; i++)
            {
                string name = req.Headers.GetKey(i);
                foreach (string value_ren in req.Headers.GetValues(i))
                {
                    buf.Append(name).Append(": ").Append(value_ren).Append('\n');
                }
            }
            buf.Append('\n');
            using (StreamReader sr = new StreamReader(req.GetRequestStream()))
            {
                buf.Append(sr.ReadToEnd());
            }
            return buf.ToString();
        }

    } 
}

