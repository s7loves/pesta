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
using org.apache.shindig.gadgets.parse;
using org.apache.shindig.gadgets.rewrite;
using org.apache.shindig.gadgets.parse.nekohtml;

namespace Pesta
{
    /// <summary>
    /// Summary description for DefaultContentRewriterRegistry
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class DefaultContentRewriterRegistry : ContentRewriterRegistry
    {
        protected readonly List<ContentRewriter> rewriters;
        protected readonly GadgetHtmlParser htmlParser;
        public static readonly DefaultContentRewriterRegistry Instance = new DefaultContentRewriterRegistry();
        protected DefaultContentRewriterRegistry()
        {
            this.rewriters = new List<ContentRewriter>
                                 {
                                     new DefaultContentRewriter(DefaultGadgetSpecFactory.Instance, ".*", "", "86400",
                                                                "style, link, img, script, embed"),
                                     new CajaContentRewriter(),
                                     new RenderingContentRewriter()
                                 };
            this.htmlParser = new NekoSimplifiedHtmlParser(new org.apache.xerces.dom.DOMImplementationImpl());
        }

        public String rewriteGadget(Gadget gadget, View currentView) 
        {
            if (currentView == null) 
            {
                // Nothing to rewrite.
                return null;
            }
            MutableContent mc = getMutableContent(gadget.getSpec(), currentView);

            foreach(ContentRewriter rewriter in rewriters) 
            {
                rewriter.rewrite(gadget, mc);
            }
            return mc.getContent();
        }

        public String rewriteGadget(Gadget gadget, String content) 
        {
            if (content == null) 
            {
            // Nothing to rewrite.
            return null;
            }

            MutableContent mc = getMutableContent(content);

            foreach(ContentRewriter rewriter in rewriters) 
            {
                rewriter.rewrite(gadget, mc);
            }

            return mc.getContent();
        }

        public sResponse rewriteHttpResponse(sRequest req, sResponse resp)
        {
            String originalContent = resp.responseString;
            MutableContent mc = getMutableContent(originalContent);

            foreach(ContentRewriter rewriter in rewriters)
            {
              rewriter.rewrite(req, resp, mc);
            }

            String rewrittenContent = mc.getContent();
            if (rewrittenContent.Equals(originalContent)) 
            {
              return resp;
            }

            return new HttpResponseBuilder(resp).setResponseString(rewrittenContent).create();
        }

        protected MutableContent getMutableContent(String content)
        {
            MutableContent mc = new MutableContent(htmlParser, content, null);
            return mc;
        }

        protected MutableContent getMutableContent(GadgetSpec spec, View v) 
        {
            // TODO - Consider using caching here to avoid parse costs
            MutableContent mc = new MutableContent(htmlParser, v.getContent(), null);
            return mc;
        }

        protected List<ContentRewriter> getRewriters()
        {
            return rewriters;
        }
    } 
}
