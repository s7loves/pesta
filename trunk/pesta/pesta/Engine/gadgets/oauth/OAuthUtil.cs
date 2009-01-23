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
using Pesta.Interop.oauth;

namespace Pesta.Engine.gadgets.oauth
{
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    public class OAuthUtil
    {
        public static String getParameter(OAuthMessage message, String name)
        {
            return message.getParameter(name);
        }

        public static List<OAuth.Parameter> getParameters(OAuthMessage message)
        {
            return message.getParameters();
        }

        public static void requireParameters(OAuthMessage message, string[] names)
        {
            message.requireParameters(names);
        }

        public static String formEncode(List<OAuth.Parameter> parameters)
        {
            return OAuth.formEncode(parameters);
        }

        public static String addParameters(String url, List<OAuth.Parameter> parameters) 
        {
            return OAuth.addParameters(url, parameters);
        }

        public static OAuthMessage newRequestMessage(OAuthAccessor accessor, String method, String url,
                        List<OAuth.Parameter> parameters)
        {
            return accessor.newRequestMessage(method, url, parameters);
        }
    }
}
