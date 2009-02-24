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
using com.google.caja.lexer;
using Uri=Pesta.Engine.common.uri.Uri;

namespace pestaServer.Models.gadgets.rewrite.lexer
{
    /// <summary>
    /// Summary description for HtmlRewriter
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class HtmlRewriter
    {
        private HtmlRewriter()
        {
        }

        public static String rewrite(String content, Uri source, Dictionary<String, IHtmlTagTransformer> transformers)
        {
            java.io.StringWriter sw = new java.io.StringWriter((content.Length * 110) / 100);
            rewrite(new java.io.StringReader(content), source, transformers, sw);
            return sw.toString();
        }

        public static void rewrite(java.io.Reader content, Uri source,
                                   Dictionary<String, IHtmlTagTransformer> transformers, java.io.Writer writer)
        {
            CharProducer producer = CharProducer.Factory.create(content, new InputSource(new java.net.URI(source.ToString())));
            HtmlLexer lexer = new HtmlLexer(producer);
            try
            {
                Token lastToken = null;
                Token currentTag = null;
                IHtmlTagTransformer currentTransformer = null;
                bool tagChanged;
                while (lexer.hasNext())
                {
                    tagChanged = false;
                    Token token = lexer.next() as Token;
                    if (token.type == HtmlTokenType.IGNORABLE)
                    {
                        continue;
                    }
                    if (token.type == HtmlTokenType.TAGBEGIN)
                    {
                        currentTag = token;
                        tagChanged = true;
                    }
                    if (tagChanged)
                    {
                        if (currentTransformer == null)
                        {
                            transformers.TryGetValue(currentTag.toString().Substring(1).ToLower(), out currentTransformer);
                        }
                        else
                        {
                            if (!currentTransformer.acceptNextTag(currentTag))
                            {
                                writer.write(currentTransformer.close());
                                transformers.TryGetValue(currentTag.toString().Substring(1).ToLower(), out currentTransformer);
                            }
                        }
                    }
                    if (currentTransformer == null)
                    {
                        writer.write(producePreTokenSeparator(token, lastToken));
                        writer.write(token.toString());
                        writer.write(producePostTokenSeparator(token, lastToken));
                    }
                    else
                    {
                        currentTransformer.accept(token, lastToken);
                    }
                    if (token.type == HtmlTokenType.TAGEND)
                    {
                        currentTag = null;
                    }
                    lastToken = token;
                }
                if (currentTransformer != null)
                {
                    writer.write(currentTransformer.close());
                }
                writer.flush();
            }
            catch (Exception pe)
            {
                throw pe;
            }

        }


        public static String producePreTokenSeparator(Token token, Token lastToken)
        {
            if (token.type == HtmlTokenType.ATTRNAME)
            {
                return " ";
            }
            if (token.type == HtmlTokenType.ATTRVALUE &&
                lastToken != null &&
                lastToken.type == HtmlTokenType.ATTRNAME)
            {
                return "=";
            }
            return "";
        }

        public static String producePostTokenSeparator(Token token, Token lastToken)
        {
            return "";
        }
    }
}