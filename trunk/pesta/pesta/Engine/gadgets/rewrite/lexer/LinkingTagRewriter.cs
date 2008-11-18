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
using System.Text;
using org.apache.shindig.gadgets.rewrite;
using com.google.caja.lexer;

namespace Pesta
{
    /// <summary>
    /// Summary description for LinkingTagRewriter
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class LinkingTagRewriter : HtmlTagTransformer
    {
        private readonly Uri relativeBase;
        private readonly LinkRewriter linkRewriter;
        private readonly Dictionary<String, HashSet<String>> tagAttributeTargets;
        private readonly StringBuilder builder;
        private HashSet<String> currentTagAttrs;

        public static Dictionary<String, HashSet<String>> getDefaultTargets()
        {
            Dictionary<String, HashSet<String>> targets = new Dictionary<string, HashSet<string>>();
            targets.Add("img", new HashSet<String>() { "src" });
            targets.Add("embed", new HashSet<String>() { "src" });
            targets.Add("link", new HashSet<String>() { "href" });
            return targets;
        }

        public LinkingTagRewriter(LinkRewriter linkRewriter, Uri relativeBase)
        {
            this.linkRewriter = linkRewriter;
            this.relativeBase = relativeBase;
            this.tagAttributeTargets = getDefaultTargets();
            builder = new StringBuilder();
        }

        public LinkingTagRewriter(Dictionary<String, HashSet<String>> tagAttributeTargets,
                                        LinkRewriter linkRewriter, Uri relativeBase)
        {
            this.tagAttributeTargets = tagAttributeTargets;
            this.linkRewriter = linkRewriter;
            this.relativeBase = relativeBase;
            builder = new StringBuilder();
        }

        public ICollection<String> getSupportedTags()
        {
            return tagAttributeTargets.Keys;
        }

        public void accept(Token token,
          Token lastToken)
        {
            if (token.type == HtmlTokenType.TAGBEGIN)
            {
                tagAttributeTargets.TryGetValue(token.toString().Substring(1).ToLower(), out currentTagAttrs);
            }

            if (currentTagAttrs != null &&
                lastToken != null &&
                lastToken.type == HtmlTokenType.ATTRNAME &&
                currentTagAttrs.Contains(lastToken.toString().ToLower()))
            {
                String link = stripQuotes(token.toString());
                builder.Append("=\"");
                builder.Append(linkRewriter.rewrite(link, relativeBase));
                builder.Append('\"');
                return;
            }
            builder.Append(HtmlRewriter.producePreTokenSeparator(token, lastToken));
            builder.Append(token.toString());
            builder.Append(HtmlRewriter.producePostTokenSeparator(token, lastToken));
        }

        public bool acceptNextTag(Token tagStart)
        {
            return false;
        }

        public String close()
        {
            String result = builder.ToString();
            currentTagAttrs = null;
            builder.Length = 0;
            return result;
        }

        private String stripQuotes(String s)
        {
            return s.Replace("\"", "").Replace("'", "");
        }
    } 
}
