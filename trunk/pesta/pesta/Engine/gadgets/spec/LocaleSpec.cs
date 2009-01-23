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
using System.Xml;
using Pesta.Engine.common.xml;
using Uri=Pesta.Engine.common.uri.Uri;

namespace Pesta.Engine.gadgets.spec
{
    /// <summary>
    /// Summary description for LocaleSpec
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class LocaleSpec
    {
        /**
       * Locale@lang
       */
        private readonly String language;
        public String getLanguage()
        {
            return language;
        }

        /**
        * Locale@country
        */
        private readonly String country;
        public String getCountry()
        {
            return country;
        }

        /**
        * Locale@language_direction
        */
        private readonly String languageDirection;
        public String getLanguageDirection()
        {
            return languageDirection;
        }

        /**
        * Locale@messages
        */
        private readonly Uri messages;
        public Uri getMessages()
        {
            return messages;
        }

        /**
        * Locale/msg
        */
        private readonly MessageBundle messageBundle;
        public MessageBundle getMessageBundle()
        {
            return messageBundle;
        }

        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("<Locale")
                .Append(" lang='").Append(language).Append('\'')
                .Append(" country='").Append(country).Append('\'')
                .Append(" language_direction='").Append(languageDirection).Append('\'')
                .Append(" messages='").Append(messages).Append("'>\n");
            foreach (var entry in messageBundle.getMessages())
            {
                buf.Append("<msg name='").Append(entry.Key).Append("'>")
                    .Append(entry.Value)
                    .Append("</msg>\n");
            }
            buf.Append("</Locale>");
            return buf.ToString();
        }

        /**
         * @param element
         * @param specUrl The url that the spec is loaded from. messages is assumed
         *     to be relative to this path.
         * @throws SpecParserException If language_direction is not valid
         */
        public LocaleSpec(XmlElement element, Uri specUrl)
        {
            language = XmlUtil.getAttribute(element, "lang", "all").ToLower();
            country = XmlUtil.getAttribute(element, "country", "ALL").ToUpper();
            languageDirection = XmlUtil.getAttribute(element, "language_direction", "ltr");
            if (!("ltr".Equals(languageDirection) ||
                  "rtl".Equals(languageDirection)))
            {
                throw new SpecParserException(
                    "Locale@language_direction must be ltr or rtl");
            }
            String messages = XmlUtil.getAttribute(element, "messages");
            if (messages == null)
            {
                this.messages = Uri.parse("");
            }
            else
            {
                try
                {
                    /*URI thisuri = new URI(messages, UriKind.RelativeOrAbsolute);
                    if (thisuri.IsAbsoluteUri)
                    {
                        this.messages = specUrl.resolve(Uri.parse(messages));
                    }
                    else
                    {
                        this.messages = specUrl.resolve(Uri.parse(specUrl.toJavaUri(),thisuri));
                    }
                    */
                    this.messages = Uri.parse(messages);
                }
                catch (Exception e)
                {
                    throw new SpecParserException("Locale@messages url is invalid.");
                }
            }
            messageBundle = new MessageBundle(element);
        }
    }
}