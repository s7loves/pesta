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
using System.IO;
using System.Text;
using System.Net;
using System;
using Pesta.Engine.auth;
using Pesta.Engine.common;
using Pesta.Engine.gadgets.oauth;
using Uri=Pesta.Engine.common.uri.Uri;


namespace Pesta.Engine.gadgets.http
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class sRequest
    {
        public static readonly String DOS_PREVENTION_HEADER = "X-shindig-dos";
        internal const String DEFAULT_CONTENT_TYPE = "application/x-www-form-urlencoded; charset=utf-8";
        internal const int CONNECT_TIMEOUT_MS = 10000;
        private Uri uri;
        private readonly HttpWebRequest req;
        
        // TODO: It might be useful to refactor these into a simple map of objects
        // and use sub classes
        // for more detailed data.

        // Cache control.
        private bool ignoreCache;
        private int cacheTtl;
        // this becoming redundant
        private byte[] postBody;
        // Whether to follow redirects
        private bool followRedirects;

        // Context for the request.
        private Uri gadget;
        private String container;
        private string method;

        // For signed fetch & OAuth
        private ISecurityToken securityToken;
        private OAuthArguments oauthArguments;
        private AuthType authType;

        private String rewriteMimeType;

        /// <summary>
        /// Construct a new request for the given uri.
        /// </summary>
        ///
        public sRequest(Uri uri)
        {
            req = (HttpWebRequest)WebRequest.Create(uri.ToString());
            //this.req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            //this.req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            req.KeepAlive = true;
            //this.req.Timeout = CONNECT_TIMEOUT_MS;
            container = ContainerConfig.DEFAULT_CONTAINER;
            followRedirects = true;
            req.AllowAutoRedirect = true;
            cacheTtl = -1;
            authType = AuthType.NONE;
            addHeader(DOS_PREVENTION_HEADER, "on");
        }

        /// <summary>
        /// Clone an existing HttpRequest.
        /// </summary>
        ///
        public sRequest(sRequest srequest)
        {
            req = srequest.req;
            uri = srequest.uri;
            ignoreCache = srequest.ignoreCache;
            cacheTtl = srequest.cacheTtl;
            gadget = srequest.gadget;
            container = srequest.container;
            securityToken = srequest.securityToken;
            if (srequest.postBody != null)
            {
                postBody = new byte[srequest.postBody.Length];
                Array.Copy(srequest.postBody, postBody, srequest.postBody.Length);
            }
            if (srequest.oauthArguments != null)
            {
                oauthArguments = new OAuthArguments(srequest.oauthArguments);
            }
            authType = srequest.authType;
            rewriteMimeType = srequest.rewriteMimeType;
            followRedirects = srequest.followRedirects;
        }

        /// <param name="_ignoreCache">Whether to ignore all caching for this request.</param>
        public sRequest setIgnoreCache(bool _ignoreCache)
        {
            this.ignoreCache = ignoreCache;
            if (ignoreCache)
            {
                // Bypass any proxy caches as well.
                req.Headers["Pragma"] = "no-cache";
            }
            return this;
        }

        public sRequest setPostBody(byte[] _postBody)
        {
            if (_postBody != null)
            {
                postBody = new byte[_postBody.Length];
                Array.Copy(_postBody, postBody, _postBody.Length);
            }
            return this;
        }

        public sRequest setMethod(String _method)
        {
            req.Method = _method;
            return this;
        }


        /// <param name="_cacheTtl">The amount of time to cache the result object for, in</param>
        public sRequest setCacheTtl(int _cacheTtl)
        {
            cacheTtl = _cacheTtl;
            return this;
        }


        /// <param name="_gadget">The gadget that caused this HTTP request to be necessary. May</param>
        public sRequest setGadget(Uri _gadget)
        {
            gadget = _gadget;
            return this;
        }

        public void setHeader(string name, string value)
        {
            req.Headers[name] = value;
        }

        /// <param name="_container">The container that this request originated from.</param>
        public sRequest setContainer(String _container)
        {
            container = _container;
            return this;
        }

        /// <summary>
        /// Assign the security token to use for making any form of authenticated
        /// request.
        /// </summary>
        ///
        public sRequest setSecurityToken(ISecurityToken _securityToken)
        {
            securityToken = _securityToken;
            return this;
        }


        /// <param name="_oauthArguments">arguments for OAuth/signed fetched</param>
        public sRequest setOAuthArguments(OAuthArguments _oauthArguments)
        {
            oauthArguments = _oauthArguments;
            return this;
        }


        /// <param name="_followRedirects">whether this request should automatically follow redirects.</param>
        public sRequest setFollowRedirects(bool _followRedirects)
        {
            followRedirects = _followRedirects;
            return this;
        }


        /// <param name="_authType">The type of authentication being used for this request.</param>
        public sRequest setAuthType(AuthType _authType)
        {
            this.authType = _authType;
            return this;
        }


        /// <param name="_rewriteMimeType">The assumed content type of the response to be rewritten.</param>
        public sRequest setRewriteMimeType(String _rewriteMimeType)
        {
            this.rewriteMimeType = _rewriteMimeType;
            return this;
        }

        public void addHeader(string name, string value)
        {
            switch (name)
            {
                case "Content-Type":
                    req.ContentType = value;
                    break;
                default:
                    req.Headers.Add(name, value);
                    break;
            }
        }
        /// <param name="name">The header to fetch</param>
        /// <returns>A list of headers with that name (may be empty).</returns>
        public string[] getHeaders(String name)
        {
            return req.Headers.GetValues(name);
        }

        /**
        * Remove all headers with the given name from the request.
        *
        * @return Any values that were removed from the request.
        */
        public void removeHeader(String name)
        {
            req.Headers.Remove(name);
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
            return req.Method;
        }
        /**
       * @return The post body as a string, assuming UTF-8 encoding.
       * TODO: We should probably tolerate other encodings, based on the
       *     Content-Type header.
       */
        public String getPostBodyAsString()
        {
            if (postBody == null)
            {
                return "";
            }
            return Encoding.UTF8.GetString(postBody);
        }
        public int getPostBodyLength()
        {
            return postBody == null ? 0 : postBody.Length;
        }

        public byte[] getPostBody()
        {
            return postBody;
        }

        public void setContentType(string type)
        {
            req.ContentType = type;
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

        public WebRequest getRequest()
        {
            return req;
        }

        public Uri getUri()
        {
            return Uri.fromJavaUri(req.RequestUri);
        }
        public void setUri(Uri _uri)
        {
            uri = _uri;
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


        public ISecurityToken getSecurityToken()
        {
            return securityToken;
        }


        public OAuthArguments getOAuthArguments()
        {
            return oauthArguments;
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
            buf.Append(getPostBodyAsString());
            return buf.ToString();
        }

    }
}