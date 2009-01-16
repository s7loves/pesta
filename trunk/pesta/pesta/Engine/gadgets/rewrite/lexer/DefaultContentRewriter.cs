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
using System;
using System.Text;
using System.Web;
using java.io;
using org.apache.shindig.gadgets.rewrite;
using Pesta.Engine.gadgets.http;
using Pesta.Engine.gadgets.spec;
using Uri=Pesta.Engine.common.uri.Uri;

namespace Pesta.Engine.gadgets.rewrite.lexer
{
    /// <summary>
    /// Default implementation of content rewriting.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
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
            try
            {
                java.io.ByteArrayOutputStream baos = new java.io.ByteArrayOutputStream((content.getContent().Length * 110) / 100);
                java.io.OutputStreamWriter output = new java.io.OutputStreamWriter(baos);
                String mimeType = original.getHeader("Content-Type");
                if (request.RewriteMimeType != null)
                {
                    mimeType = request.RewriteMimeType;
                }
                GadgetSpec spec = null;
                if (request.Gadget != null)
                {
                    spec = _specFactory.getGadgetSpec(request.Gadget.toJavaUri(), false);
                }
                if (rewrite(spec, request.Uri,
                            content,
                            mimeType,
                            output))
                {
                    content.setContent(Encoding.Default.GetString(baos.toByteArray()));
                    return RewriterResults.cacheableIndefinitely();

                }
            }
            catch (GadgetException ge)
            {
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        public virtual RewriterResults rewrite(Gadget gadget, MutableContent content)
        {
            java.io.StringWriter sw = new java.io.StringWriter();
            GadgetSpec spec = gadget.getSpec();
            Uri _base = spec.getUrl();
            View view = gadget.getCurrentView();
            if (view != null && view.getHref() != null) 
            {
                _base = view.getHref();
            }
            if (rewrite(spec, _base, content, "text/html", sw)) 
            {
                content.setContent(sw.toString());
            }
            return null;
        }

        private bool rewrite(GadgetSpec spec, Uri source, MutableContent mc, String mimeType, java.io.Writer writer)
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
                HtmlRewriter.rewrite(new StringReader(mc.getContent()), source, transformerMap, writer);
                return true;
            }
            if (isCSS(mimeType))
            {
                if (ProxyUrl != null)
                {
                    CssRewriter.rewrite(new StringReader(mc.getContent()), source, CreateLinkRewriter(spec, rewriterFeature), writer, false);
                    return true;
                }
                return false;
            }
            return false;
        }

        private bool isHTML(String mime)
        {
            return mime != null && (mime.ToLower().Contains("html"));
        }

        private bool isCSS(String mime)
        {
            return mime != null && (mime.ToLower().Contains("css"));
        }

        protected String ProxyUrl
        {
            get
            {
                if (!HttpRuntime.AppDomainAppVirtualPath.Equals("/"))
                {
                    return HttpRuntime.AppDomainAppVirtualPath + "/gadgets/proxy.ashx?url=";
                }
                return "/gadgets/proxy.ashx?url=";
            }
        }


        protected String ConcatUrl
        {
            get
            {
                if (!HttpRuntime.AppDomainAppVirtualPath.Equals("/"))
                {
                    return HttpRuntime.AppDomainAppVirtualPath + "/gadgets/concat.ashx?"; 
                }
                return "/gadgets/concat.ashx?";
            }
        }


        protected internal LinkRewriter CreateLinkRewriter(GadgetSpec spec, ContentRewriterFeature rewriterFeature)
        {
            return new ProxyingLinkRewriter(spec.getUrl(), rewriterFeature, ProxyUrl);
        }
    }
}