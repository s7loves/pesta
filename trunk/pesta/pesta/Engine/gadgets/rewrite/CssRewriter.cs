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
using URI = java.net.URI;
using com.google.caja.lexer;

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

    public static String rewrite(String content, URI source, LinkRewriter linkRewriter) 
    {
        java.io.StringWriter sw = new java.io.StringWriter((content.Length * 110) / 100);
        rewrite(new java.io.StringReader(content), source, linkRewriter, sw);
        return sw.ToString();
    }

    public static void rewrite(java.io.Reader content, URI source, LinkRewriter rewriter, java.io.Writer writer) 
    {
        CharProducer producer = CharProducer.Factory.create(content,
        new InputSource(source));
        CssLexer lexer = new CssLexer(producer);
        try 
        {
            while (lexer.hasNext()) 
            {
                Token token = lexer.next();
                if (token.type == CssTokenType.URI) 
                {
                    writer.write(rewriteLink(token, source, rewriter));
                    continue;
                }
                writer.write(token.toString());
            }
            writer.flush();
        } 
        catch (ParseException pe) 
        {
            pe.printStackTrace();
        } 
        catch (java.io.IOException ioe) 
        {
            ioe.printStackTrace();
        }
    }

    private static String rewriteLink(Token token, URI _base, LinkRewriter rewriter) 
    {
        Match matcher = urlMatcher.Match(token.toString());
        if (!matcher.Success) 
            return token.toString();

        return "url(\"" + rewriter.rewrite(matcher.Groups[2].Value.Trim(), _base) + "\")";
    }
}
