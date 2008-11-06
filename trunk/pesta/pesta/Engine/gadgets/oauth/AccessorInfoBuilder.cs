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

namespace Pesta
{
    /// <summary>
    /// Summary description for AccessorInfoBuilder
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class AccessorInfoBuilder
    {
        private OAuthStore.ConsumerInfo consumer;
        private String requestToken;
        private String accessToken;
        private String tokenSecret;
        private AccessorInfo.OAuthParamLocation location;
        private AccessorInfo.HttpMethod method;

        public AccessorInfoBuilder()
        {
        }

        public AccessorInfo create()
        {
            if (location == null)
            {
                throw new GadgetException(GadgetException.Code.INTERNAL_SERVER_ERROR, "no location");
            }
            if (consumer == null)
            {
                throw new GadgetException(GadgetException.Code.INTERNAL_SERVER_ERROR, "no consumer");
            }

            OAuthAccessor accessor = new OAuthAccessor(consumer.getConsumer());

            // request token/access token/token secret can all be null, for signed fetch, or if the OAuth
            // dance is just beginning
            accessor.requestToken = requestToken;
            accessor.accessToken = accessToken;
            accessor.tokenSecret = tokenSecret;
            return new AccessorInfo(accessor, consumer, method, location);
        }

        public void setConsumer(OAuthStore.ConsumerInfo consumer)
        {
            this.consumer = consumer;
        }

        public void setRequestToken(String requestToken)
        {
            this.requestToken = requestToken;
        }

        public void setAccessToken(String accessToken)
        {
            this.accessToken = accessToken;
        }

        public void setTokenSecret(String tokenSecret)
        {
            this.tokenSecret = tokenSecret;
        }

        public void setParameterLocation(AccessorInfo.OAuthParamLocation location)
        {
            this.location = location;
        }

        public void setMethod(AccessorInfo.HttpMethod method)
        {
            this.method = method;
        }
    } 
}
