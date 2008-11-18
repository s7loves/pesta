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
using System.Text;
using System.Text.RegularExpressions;
using com.google.caja.lexer;
using System.Collections.Generic;

namespace Pesta
{
    /// <summary>
    /// Summary description for CssRewriter
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class CssRewriter
    {
        private static readonly Regex urlMatcher = new Regex("(url\\s*\\(\\s*['\"]?)([^\\)'\"]*)(['\"]?\\s*\\))", RegexOptions.IgnoreCase);

        public static String rewrite(String content, Uri source, LinkRewriter linkRewriter)
        {
            java.io.StringWriter sw = new java.io.StringWriter((content.Length * 110) / 100);
            rewrite(new java.io.StringReader(content), source, linkRewriter, sw, false);
            return sw.ToString();
        }

        public static List<String> rewrite(java.io.Reader content, Uri source,
          LinkRewriter rewriter, java.io.Writer writer, bool extractImports)
        {
            List<String> imports = new List<string>();
            CharProducer producer = CharProducer.Factory.create(content,
                new InputSource(new java.net.URI(source.ToString())));
            CssLexer lexer = new CssLexer(producer);
            try
            {
                bool inImport = false;
                while (lexer.hasNext())
                {
                    Token token = lexer.next();
                    if (extractImports)
                    {
                        if (token.type == CssTokenType.SYMBOL && token.text.ToLower().Equals("@import"))
                        {
                            inImport = true;
                            continue;
                        }
                        if (inImport)
                        {
                            if (token.type == CssTokenType.URI)
                            {
                                Match matcher = urlMatcher.Match(token.text);
                                if (matcher.Success)
                                {
                                    imports.Add(matcher.Groups[2].Value.Trim());
                                }
                            }
                            else if (token.type != CssTokenType.SPACE && token.type != CssTokenType.PUNCTUATION)
                            {
                                inImport = false;
                            }
                        }
                        if (!inImport)
                        {
                            writer.write(token.text);
                        }
                    }
                    else
                    {
                        if (token.type == CssTokenType.URI)
                        {
                            writer.write(rewriteLink(token, source, rewriter));
                            continue;
                        }
                        writer.write(token.text);
                    }
                }
                writer.flush();
            }
            catch (ParseException pe)
            {
                pe.printStackTrace();
            }
            catch (Exception ioe)
            {
                throw ioe;
            }
            return imports;
        }

        private static String rewriteLink(Token token, Uri _base, LinkRewriter rewriter)
        {
            Match matcher = urlMatcher.Match(token.toString());
            if (!matcher.Success)
                return token.toString();

            return "url(\"" + rewriter.rewrite(matcher.Groups[2].Value.Trim(), _base) + "\")";
        }
    } 
}
