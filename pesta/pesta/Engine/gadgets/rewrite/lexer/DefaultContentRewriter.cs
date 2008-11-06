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
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.IO;
using System;
using System.Text;
using System.Web;
using org.apache.shindig.gadgets;

using org.apache.shindig.gadgets.rewrite;

namespace Pesta
{
    /// <summary>
    /// Default implementation of content rewriting.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class DefaultContentRewriter : ContentRewriter
    {
        private readonly GadgetSpecFactory _specFactory;
        private readonly String includeUrls;
        private readonly String excludeUrls;
        private readonly String expires;
        private readonly HashSet<String> includeTags;

        public DefaultContentRewriter(GadgetSpecFactory specFactory, String includeUrls,
                String excludeUrls, String expires, String includeTags)
        {
            this._specFactory = specFactory;
            this.includeUrls = includeUrls;
            this.excludeUrls = excludeUrls;
            this.expires = expires;
            this.includeTags = new HashSet<string>();
            /* foreach */
            foreach (String s in includeTags.Split(','))
            {
                if (s != null && s.Trim().Length > 0)
                {
                    this.includeTags.Add(s.Trim().ToLower());
                }
            }
        }

        public virtual RewriterResults rewrite(sRequest request, sResponse original, MutableContent content)
        {
            //try
            {
                java.io.ByteArrayOutputStream baos = new java.io.ByteArrayOutputStream((content.getContent().Length * 110) / 100);
                java.io.OutputStreamWriter output = new java.io.OutputStreamWriter(baos);
                String mimeType = original.response.ContentType;
                if (request.RewriteMimeType != null)
                {
                    mimeType = request.RewriteMimeType;
                }
                GadgetSpec spec = null;
                if (request.Gadget != null)
                {
                    spec = _specFactory.getGadgetSpec(request.Gadget.toJavaUri(), false);
                }
                rewrite(spec, request.Uri.toJavaUri(), new java.io.StringReader(content.getContent()),
                    mimeType, output);
            }
            //catch {}

            return RewriterResults.cacheableIndefinitely();
        }

        public virtual RewriterResults rewrite(Gadget gadget)
        {
            java.io.StringWriter sw = new java.io.StringWriter();
            GadgetSpec spec = gadget.Spec;
            if (rewrite(spec, spec.getUrl(), new java.io.StringReader(gadget.getContent()), "text/html", sw))
            {
                gadget.setContent(sw.toString());
            }
            return RewriterResults.cacheableIndefinitely();
        }

        private bool rewrite(GadgetSpec spec, java.net.URI source, java.io.Reader reader, String mimeType, java.io.Writer writer)
        {
            // Dont rewrite content if the spec is unavailable
            if (spec == null)
            {
                return false;
            }

            // Store the feature in the spec so we dont keep parsing it
            ContentRewriterFeature rewriterFeature = new ContentRewriterFeature(spec, includeUrls, excludeUrls, expires, includeTags);

            if (!rewriterFeature.isRewriteEnabled())
            {
                return false;
            }
            if (isHTML(mimeType))
            {
                Dictionary<String, HtmlTagTransformer> transformerMap = new Dictionary<string, HtmlTagTransformer>();

                if (ProxyUrl != null)
                {
                    LinkRewriter linkRewriter = CreateLinkRewriter(spec, rewriterFeature);
                    LinkingTagRewriter rewriter = new LinkingTagRewriter(linkRewriter, source);
                    HashSet<String> toProcess = new HashSet<string>();
                    foreach (var item in rewriter.getSupportedTags())
                    {
                        toProcess.Add(item);
                    }

                    toProcess.IntersectWith(rewriterFeature.getIncludedTags());

                    foreach (string tag in toProcess)
                    {
                        transformerMap[tag] = rewriter;
                    }
                    if (rewriterFeature.getIncludedTags().Contains("style"))
                    {
                        transformerMap["style"] = new StyleTagRewriter(source, linkRewriter);
                    }
                }
                if (ConcatUrl != null &&
                    rewriterFeature.getIncludedTags().Contains("script"))
                {
                    transformerMap["script"] = new JavascriptTagMerger(spec, rewriterFeature, ConcatUrl, source);

                }
                HtmlRewriter.rewrite(reader, source, transformerMap, writer);
                return true;
            }
            else if (isCSS(mimeType))
            {
                if (ProxyUrl != null)
                {
                    CssRewriter.rewrite(reader, source, CreateLinkRewriter(spec, rewriterFeature), writer);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private bool isHTML(String mime)
        {
            if (mime == null)
            {
                return false;
            }
            return (mime.ToLower().Contains("html"));
        }

        private bool isCSS(String mime)
        {
            if (mime == null)
            {
                return false;
            }
            return (mime.ToLower().Contains("css"));
        }

        protected String ProxyUrl
        {
            get
            {
                return HttpRuntime.AppDomainAppVirtualPath + "/gadgets/proxy.ashx?url=";
            }
        }


        protected String ConcatUrl
        {
            get 
            {
                return HttpRuntime.AppDomainAppVirtualPath + "/gadgets/concat.ashx?"; 
            }
        }


        protected internal LinkRewriter CreateLinkRewriter(GadgetSpec spec, ContentRewriterFeature rewriterFeature)
        {
            return new ProxyingLinkRewriter(spec.getUrl(), rewriterFeature, ProxyUrl);
        }
    } 
}

