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
using URI = java.net.URI;
using System.Web;

namespace Pesta
{
    /// <summary>
    /// Summary description for ProxyingLinkRewriter
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class ProxyingLinkRewriter : LinkRewriter
    {
        private readonly String prefix;

        private readonly ContentRewriterFeature rewriterFeature;

        private readonly URI gadgetUri;

        public ProxyingLinkRewriter(URI gadgetUri, ContentRewriterFeature rewriterFeature, String prefix)
        {
            this.prefix = prefix;
            this.rewriterFeature = rewriterFeature;
            this.gadgetUri = gadgetUri;
        }

        public String rewrite(String link, URI context)
        {
            link = link.Trim();
            // We shouldnt bother proxying empty URLs
            if (link.Length == 0)
            {
                return link;
            }

            try
            {
                URI linkUri = new URI(link);
                URI uri = context.resolve(linkUri);
                if (rewriterFeature.shouldRewriteURL(uri.toString()))
                {
                    String result = prefix
                    + HttpUtility.UrlEncode(uri.toString())
                    + "&gadget="
                    + HttpUtility.UrlEncode(gadgetUri.toString())
                    + "&fp="
                    + rewriterFeature.getFingerprint();
                    if (rewriterFeature.getExpires() != null)
                    {
                        result += "&" + ProxyBase.REFRESH_PARAM + "=" + rewriterFeature.getExpires().ToString();
                    }
                    return result;
                }
                else
                {
                    return uri.toString();
                }
            }
            catch
            {
                // Unrecoverable. Just return link
                return link;
            }
        }
    } 
}
