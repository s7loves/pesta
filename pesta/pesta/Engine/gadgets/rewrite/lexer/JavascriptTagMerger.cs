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
using System.Web;
using System.Net;
using System.Text;
using com.google.caja.lexer;

namespace Pesta
{
    /// <summary>
    /// Summary description for JavascriptTagMerger
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class JavascriptTagMerger : HtmlTagTransformer
    {
        private readonly static int MAX_URL_LENGTH = 1500;
        private readonly List<Object> scripts = new List<Object>();
        private readonly String concatBase;
        private readonly Uri relativeUrlBase;
        private bool isTagOpen = true;

        /**
         * @param concatBase Base url of the Concat servlet. Expected to be of the
         *                   form www.host.com/concat?
         * @param relativeUrlBase to resolve relative urls
         */
        public JavascriptTagMerger(GadgetSpec spec, ContentRewriterFeature rewriterFeature,
                                 String concatBase, Uri relativeUrlBase)
        {
            // Force the mime-type to mimic browser expectation so rewriters
            // can function properly
            this.concatBase = concatBase
                + ProxyBase.REWRITE_MIME_TYPE_PARAM
                + "=text/javascript&"
                + "gadget="
                + HttpUtility.UrlEncode(spec.getUrl().ToString())
                + "&fp="
                + rewriterFeature.getFingerprint()
                + '&';

            this.relativeUrlBase = relativeUrlBase;
        }

        public void accept(Token token, Token lastToken)
        {
            try
            {
                if (isTagOpen)
                {
                    if (lastToken != null &&
                    lastToken.type == HtmlTokenType.ATTRNAME &&
                    lastToken.toString().ToLower().Equals("src"))
                    {
                        scripts.Add(new Uri(stripQuotes(token.toString())));
                    }
                    else if (token.type == HtmlTokenType.UNESCAPED)
                    {
                        scripts.Add(token);
                    }
                }
            }
            catch (UriFormatException use)
            {
                throw use;
            }
        }

        public bool acceptNextTag(Token tagStart)
        {
            if (tagStart.toString().ToLower().Equals("<script"))
            {
                isTagOpen = true;
                return true;
            }
            else if (tagStart.toString().ToLower().Equals("</script"))
            {
                isTagOpen = false;
                return true;
            }
            return false;
        }

        public String close()
        {
            List<Uri> concat = new List<Uri>();
            StringBuilder builder = new StringBuilder(100);
            foreach (Object o in scripts)
            {
                if (o is Uri)
                {
                    concat.Add((Uri)o);
                }
                else
                {
                    flushConcat(concat, builder);
                    builder.Append("<script type=\"text/javascript\">")
                        .Append(((Token)o).toString()).Append("</script>");
                }
            }
            flushConcat(concat, builder);
            scripts.Clear();
            isTagOpen = true;
            return builder.ToString();
        }

        private void flushConcat(List<Uri> concat, StringBuilder builder)
        {
            if (concat.Count == 0)
            {
                return;
            }
            builder.Append("<script src=\"").Append(concatBase);
            int urlStart = builder.Length;
            int paramIndex = 1;
            try
            {
                for (int i = 0; i < concat.Count; i++)
                {
                    Uri srcUrl = concat[i];
                    if (!srcUrl.IsAbsoluteUri)
                    {
                        srcUrl = relativeUrlBase.MakeRelativeUri(srcUrl);
                    }
                    builder.Append(paramIndex).Append('=')
                    .Append(HttpUtility.UrlEncode(srcUrl.ToString()));
                    if (i < concat.Count - 1)
                    {
                        if (builder.Length - urlStart > MAX_URL_LENGTH)
                        {
                            paramIndex = 1;
                            builder.Append("\" type=\"text/javascript\"></script>\n");
                            builder.Append("<script src=\"").Append(concatBase);
                            urlStart = builder.Length;
                        }
                        else
                        {
                            builder.Append('&');
                            paramIndex++;
                        }
                    }
                }
                builder.Append("\" type=\"text/javascript\"></script>");
                concat.Clear();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private String stripQuotes(String s)
        {
            return s.Replace("\"", "").Replace("'", "");
        }
    } 
}
