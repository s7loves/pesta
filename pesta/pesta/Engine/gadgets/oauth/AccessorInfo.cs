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

using Pesta.Interop.oauth;

namespace Pesta.Engine.gadgets.oauth
{
    /// <summary>
    /// Summary description for AccessorInfo
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class AccessorInfo
    {
        public enum HttpMethod
        {
            GET,
            POST
        }

        public enum OAuthParamLocation
        {
            AUTH_HEADER,
            POST_BODY,
            URI_QUERY
        }

        private readonly OAuthAccessor accessor;
        private readonly OAuthStore.ConsumerInfo consumer;
        private readonly HttpMethod httpMethod;
        private readonly OAuthParamLocation paramLocation;

        public AccessorInfo(OAuthAccessor accessor, OAuthStore.ConsumerInfo consumer, HttpMethod httpMethod,
                            OAuthParamLocation paramLocation)
        {
            this.accessor = accessor;
            this.consumer = consumer;
            this.httpMethod = httpMethod;
            this.paramLocation = paramLocation;
        }

        public OAuthParamLocation getParamLocation()
        {
            return paramLocation;
        }

        public OAuthAccessor getAccessor()
        {
            return accessor;
        }

        public OAuthStore.ConsumerInfo getConsumer()
        {
            return consumer;
        }

        public HttpMethod getHttpMethod()
        {
            return httpMethod;
        }
    }
}