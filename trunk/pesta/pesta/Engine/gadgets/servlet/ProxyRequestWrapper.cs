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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace Pesta
{
    /// <summary>
    /// Summary description for ProxyRequestWrapper
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class ProxyRequestWrapper : HttpRequestWrapper
    {
        protected static Regex CHAINED_SYNTAX_PATTERN = new Regex("^[^?]+/proxy/([^?/]*)/(.*)$", RegexOptions.Compiled);
        protected static Regex PARAMETER_PAIR_PATTERN = new Regex("([^&=]+)=([^&=]*)", RegexOptions.Compiled);

        protected bool usingChainedSyntax;
        protected Dictionary<String, String> extractedParameters;

        public ProxyRequestWrapper(HttpContext context)
            : base(context)
        {
            Match chainedMatcher = CHAINED_SYNTAX_PATTERN.Match(request.Url.ToString());
            usingChainedSyntax = chainedMatcher.Success;
            if (usingChainedSyntax)
            {
                extractedParameters = new Dictionary<String, String>();

                Match paramMatcher = PARAMETER_PAIR_PATTERN.Match(chainedMatcher.Groups[1].Value);
                while (paramMatcher.NextMatch() != null)
                {
                    extractedParameters.Add(HttpUtility.UrlDecode(paramMatcher.Groups[1].Value),
                        HttpUtility.UrlDecode(paramMatcher.Groups[2].Value));
                }

                extractedParameters.Add(ProxyHandler.URL_PARAM, chainedMatcher.Groups[2].Value);
            }
            else
            {
                extractedParameters = null;
            }
        }

        /**
        * @return True if the request is using the chained syntax form.
        */
        public bool isUsingChainedSyntax()
        {
            return usingChainedSyntax;
        }

        public override String getParameter(String name)
        {
            if (usingChainedSyntax)
            {
                return extractedParameters[name];
            }
            else
            {
                return base.getParameter(name);
            }
        }
    } 
}
