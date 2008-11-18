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
using org.apache.shindig.gadgets.rewrite;
using System.Text;
using com.google.caja.lexer;

namespace Pesta
{
    /// <summary>
    /// Summary description for StyleTagRewriter
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class StyleTagRewriter : HtmlTagTransformer
    {
        private Uri source;
        private LinkRewriter linkRewriter;

        private StringBuilder sb;

        public StyleTagRewriter(Uri source, LinkRewriter linkRewriter)
        {
            this.source = source;
            this.linkRewriter = linkRewriter;
            sb = new StringBuilder(500);
        }

        public void accept(Token token, Token lastToken)
        {
            if (token.type == HtmlTokenType.UNESCAPED)
            {
                sb.Append(CssRewriter.rewrite(token.toString(), source, linkRewriter));
            }
            else
            {
                sb.Append(HtmlRewriter.producePreTokenSeparator(token, lastToken));
                sb.Append(token.toString());
                sb.Append(HtmlRewriter.producePostTokenSeparator(token, lastToken));
            }
        }

        public bool acceptNextTag(Token tagStart)
        {
            return false;
        }

        public String close()
        {
            String result = sb.ToString();
            sb.Length = 0;
            return result;
        }
    } 
}
