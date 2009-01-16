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
using System.Text;
using Pesta.Engine.auth;

namespace Pesta.Engine.gadgets.http
{
    public abstract class AbstractHttpCache : HttpCache
    {
        public static readonly char KEY_SEPARATOR = ':';
        public static readonly String DEFAULT_KEY_VALUE = "0";

          // Implement these methods to create a concrete HttpCache class.
        protected abstract sResponse getResponseImpl(String key);
        protected abstract void addResponseImpl(String key, sResponse response);
        protected abstract sResponse removeResponseImpl(String key);

        public sResponse getResponse(sRequest request) 
        {
            if (isCacheable(request)) 
            {
                String keyString = createKey(request);
                sResponse cached = getResponseImpl(keyString);
                if (responseStillUsable(cached)) 
                {
                    return cached;
                }
            }
            return null;
        }

        public sResponse addResponse(sRequest request, sResponse response) 
        {
            if (isCacheable(request) && isCacheable(response))
            {
                // Both are cacheable. Check for forced cache TTL overrides.
                HttpResponseBuilder responseBuilder = new HttpResponseBuilder(response);
                int forcedTtl = request.CacheTtl;
                if (forcedTtl != -1)
                {
                    responseBuilder.setCacheTtl(forcedTtl);
                }

                response = responseBuilder.create();
                String keyString = createKey(request);
                addResponseImpl(keyString, response);
            }

            return response;
        }

        public sResponse removeResponse(sRequest request) 
        {
            String keyString = createKey(request);
            sResponse response = getResponseImpl(keyString);
            removeResponseImpl(keyString);
            if (responseStillUsable(response))
            {
                return response;
            }
            return null;
        }


        protected bool isCacheable(sResponse response)
        {
            return !response.isStrictNoCache();
        }

        protected bool isCacheable(sRequest request)
        {
            if (request.IgnoreCache)
            {
                return false;
            }
            if (!"GET".Equals(request.getMethod()) &&
                !"GET".Equals(request.getHeader("X-Method-Override")))
            {
                return false;
            }
            return true;
        }

        /**
         * Produce a key from the given request.
         *
         * Relevant pieces of the cache key:
         *
         * - request URI
         * - authentication type
         * - owner id
         * - viewer id
         * - owner of the token
         * - gadget url (from security token; we don't trust what's on the URI itself)
         * - instance id
         * - oauth service name
         * - oauth token name
         *
         * Except for the first two, all of these may be "0" depending on authentication rules. See
         * individual methods for details.
         */
        protected String createKey(sRequest request)
        {
            String uri = request.Uri.ToString();
            StringBuilder key = new StringBuilder(uri.Length * 2);
            key.Append(request.Uri);
            key.Append(KEY_SEPARATOR);
            key.Append(request.AuthType);
            key.Append(KEY_SEPARATOR);
            key.Append(getOwnerId(request));
            key.Append(KEY_SEPARATOR);
            key.Append(getViewerId(request));
            key.Append(KEY_SEPARATOR);
            key.Append(getTokenOwner(request));
            key.Append(KEY_SEPARATOR);
            key.Append(getAppUrl(request));
            key.Append(KEY_SEPARATOR);
            key.Append(getInstanceId(request));
            key.Append(KEY_SEPARATOR);
            key.Append(getServiceName(request));
            key.Append(KEY_SEPARATOR);
            key.Append(getTokenName(request));
            return key.ToString();
        }

        protected static String getOwnerId(sRequest request)
        {
            if (request.AuthType != AuthType.NONE &&
                request.OAuthArguments.SignOwner)
            {
                return request.SecurityToken.getOwnerId();
            }
            // Requests that don't use authentication can share the result.
            return DEFAULT_KEY_VALUE;
        }

        protected static String getViewerId(sRequest request)
        {
            if (request.AuthType != AuthType.NONE &&
                request.OAuthArguments.SignViewer)
            {
                return request.SecurityToken.getViewerId();
            }
            // Requests that don't use authentication can share the result.
            return DEFAULT_KEY_VALUE;
        }

        protected static String getTokenOwner(sRequest request)
        {
            SecurityToken st = request.SecurityToken;
            if (request.AuthType != AuthType.NONE &&
                st.getOwnerId() != null
                && st.getOwnerId().Equals(st.getViewerId())
                && request.OAuthArguments.MayUseToken())
            {
                return st.getOwnerId();
            }
            // Requests that don't use authentication can share the result.
            return DEFAULT_KEY_VALUE;
        }

        protected static String getAppUrl(sRequest request)
        {
            if (request.AuthType != AuthType.NONE)
            {
                return request.SecurityToken.getAppUrl();
            }
            // Requests that don't use authentication can share the result.
            return DEFAULT_KEY_VALUE;
        }

        protected static String getInstanceId(sRequest request)
        {
            if (request.AuthType != AuthType.NONE)
            {
                return request.SecurityToken.getModuleId().ToString();
            }
            // Requests that don't use authentication can share the result.
            return DEFAULT_KEY_VALUE;
        }

        protected static String getServiceName(sRequest request)
        {
            if (request.AuthType != AuthType.NONE)
            {
                return request.OAuthArguments.ServiceName;
            }
            // Requests that don't use authentication can share the result.
            return DEFAULT_KEY_VALUE;
        }

        protected static String getTokenName(sRequest request)
        {
            if (request.AuthType != AuthType.NONE)
            {
                return request.OAuthArguments.TokenName;
            }
            // Requests that don't use authentication can share the result.
            return DEFAULT_KEY_VALUE;
        }

        /**
         * Utility function to verify that an entry is cacheable and not expired
         * @return true If the response can be used.
         */
        protected bool responseStillUsable(sResponse response)
        {
            if (response == null)
            {
                return false;
            }
            return response.getCacheExpiration() > DateTime.UtcNow.Ticks;
        }

    }
}
