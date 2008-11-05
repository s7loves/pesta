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
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.IO;
using org.apache.shindig.gadgets.parse.caja;
using org.apache.shindig.gadgets.parse;
using org.apache.shindig.gadgets.rewrite;
using org.apache.shindig.gadgets;

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
            DefaultContentRewriter rewriter =
                new DefaultContentRewriter(BasicGadgetSpecFactory.Instance, ".*", "", "86400", "link,script,embed,img,style");
            this.rewriters = new List<ContentRewriter>();
            this.rewriters.Add(rewriter);
            this.htmlParser = new CajaHtmlParser();
        }

        public bool rewriteGadget(Gadget gadget)
        {
            String originalContent = gadget.getContent();
            if (originalContent == null)
            {
                // Nothing to rewrite.
                return false;
            }
            foreach (ContentRewriter rewriter in rewriters)
            {
                rewriter.rewrite(gadget);
            }

            return !originalContent.Equals(gadget.getContent());
        }

        public sResponse rewriteHttpResponse(sRequest req, sResponse resp)
        {
            MutableContent mc = new MutableContent(htmlParser);
            string originalContent = "";
            originalContent = resp.responseString;
            mc.setContent(originalContent);
            foreach (ContentRewriter rewriter in rewriters)
            {
                rewriter.rewrite(req, resp, mc);
            }

            String rewrittenContent = mc.getContent();
            if (rewrittenContent.Equals(originalContent))
            {
                return resp;
            }
            using (StreamWriter writer = new StreamWriter(resp.response.GetResponseStream()))
            {
                writer.Write(rewrittenContent);
            }
            return resp;
        }
    } 
}
