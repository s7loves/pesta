﻿/*
 * Copyright 2007 Netflix, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

namespace Pesta.Libraries.OAuth
{
    [Serializable]
    public class OAuthServiceProvider
    {
/*
        private static readonly long serialVersionUID = 3306534392621038574L;
*/
        public readonly String requestTokenURL;
        public readonly String userAuthorizationURL;
        public readonly String accessTokenURL;

        public OAuthServiceProvider(String requestTokenURL,
                                    String userAuthorizationURL, String accessTokenURL)
        {
            this.requestTokenURL = requestTokenURL;
            this.userAuthorizationURL = userAuthorizationURL;
            this.accessTokenURL = accessTokenURL;
        }
    }
}