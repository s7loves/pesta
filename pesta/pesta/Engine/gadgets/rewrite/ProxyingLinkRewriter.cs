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
using System.Web;
using Pesta.Engine.gadgets.servlet;
using Uri=Pesta.Engine.common.uri.Uri;

namespace Pesta.Engine.gadgets.rewrite
{
    /// <summary>
    /// Summary description for ProxyingLinkRewriter
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class ProxyingLinkRewriter : ILinkRewriter
    {
        private readonly String prefix;

        private readonly ContentRewriterFeature rewriterFeature;

        private readonly Uri gadgetUri;

        public ProxyingLinkRewriter(Uri gadgetUri, ContentRewriterFeature rewriterFeature, String prefix)
        {
            this.prefix = prefix;
            this.rewriterFeature = rewriterFeature;
            this.gadgetUri = gadgetUri;
        }

        public String rewrite(String link, Uri context)
        {
            link = link.Trim();
            // We shouldnt bother proxying empty URLs
            if (link.Length == 0)
            {
                return link;
            }

            try
            {
                Uri linkUri = Uri.parse(link);
                Uri uri = context.resolve(linkUri);
                if (rewriterFeature.shouldRewriteURL(uri.ToString()))
                {
                    String result = prefix
                                    + HttpUtility.UrlEncode(uri.ToString())
                                    + ((gadgetUri == null) ? "" : "&gadget=" + HttpUtility.UrlEncode(gadgetUri.ToString()))
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
                    return uri.ToString();
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