/*
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
using System.Collections.Generic;

namespace Pesta.Libraries.OAuth
{
    [Serializable]
    public class OAuthAccessor
    {
/*
        private static readonly long serialVersionUID = 5590788443138352999L;
*/

        public readonly OAuthConsumer consumer;
        public String requestToken;
        public String accessToken;
        public String TokenSecret { get; set; }

        public OAuthAccessor(OAuthConsumer consumer)
        {
            this.consumer = consumer;
            requestToken = null;
            accessToken = null;
            TokenSecret = null;
        }

        private readonly Dictionary<String, Object> properties = new Dictionary<String, Object>();

        public Object getProperty(String name)
        {
            return properties[name];
        }

        public void setProperty(String name, Object value)
        {
            properties.Add(name, value);
        }

        public OAuthMessage newRequestMessage(String method, String url,
                                              List<OAuth.Parameter> parameters)
        {
            if (method == null)
            {
                method = (String)getProperty("httpMethod") ?? ((String)consumer.getProperty("httpMethod") ?? "GET");
            }
            OAuthMessage message = new OAuthMessage(method, url, parameters);
            message.addRequiredParameters(this);
            return message;
        }
    }
}