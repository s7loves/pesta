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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pesta.Engine.gadgets.oauth;
using Pesta.Utilities;

namespace Pesta.Engine.gadgets.http
{
    public class DefaultRequestPipeline : IRequestPipeline
    {
        private readonly IHttpFetcher httpFetcher;
        private readonly Provider<OAuthRequest> oauthRequestProvider;
        public readonly static DefaultRequestPipeline Instance = new DefaultRequestPipeline();
        protected DefaultRequestPipeline() 
        {
            httpFetcher = BasicHttpFetcher.Instance;
            oauthRequestProvider = new Provider<OAuthRequest>();
        }

        public sResponse execute(sRequest request)
        {
            sResponse response;
            if (!request.IgnoreCache) 
            {
                response = DefaultHttpCache.Instance.getResponse(request);
                if (response != null && !response.isStale())
                {
                    return response;
                }
            }

            if (request.AuthType == AuthType.NONE)
            {
                response = httpFetcher.fetch(request);
            }
            else if (request.AuthType == AuthType.OAUTH || request.AuthType == AuthType.SIGNED)
            {
                response = oauthRequestProvider.get().fetch(request);
            }
            else
            {
                return sResponse.error();
            }

            if (!request.IgnoreCache)
            {
                DefaultHttpCache.Instance.addResponse(request, response);
            }
            return response;
        }
    }
}
