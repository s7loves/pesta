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
using Pesta.Engine.gadgets.variables;

namespace Pesta.Engine.gadgets.spec
{
    /// <summary>
    /// Summary description for Icon
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class Icon
    {
        /**
        * Icon@mode
        * Probably better labeled "encoding"; currently only base64 is supported.
        * If mode is not set, content must be a url. Otherwise, content is
        * a mode-encoded image with a mime type equal to type.
        */
        private readonly String mode;
        public String getMode()
        {
            return mode;
        }

        /**
        * Icon@type
        * Mime type of the icon
        */
        private readonly String type;
        public String getType()
        {
            return type;
        }

        /**
        * Icon#CDATA
        *
        * Message Bundles
        */
        private String content;
        public String getContent()
        {
            return content;
        }

        /**
        * Substitutes the icon fields according to the spec.
        *
        * @param substituter
        * @return The substituted icon
        */
        public Icon substitute(Substitutions substituter)
        {
            Icon icon = new Icon(this);
            icon.content = substituter.substituteString(Substitutions.Type.MESSAGE, content);
            return icon;
        }

        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("<Icon type='").Append(type).Append('\'');
            if (mode != null)
            {
                buf.Append(" mode='").Append(mode).Append('\'');
            }
            buf.Append('>')
                .Append(content)
                .Append("</Icon>");
            return buf.ToString();
        }

        /**
        * Currently does not validate icon data.
        * @param element
        */
        public Icon(XmlElement element)
        {
            mode = XmlUtil.getAttribute(element, "mode");
            if (mode != null && !mode.Equals("base64"))
            {
                throw new SpecParserException(
                    "The only valid value for Icon@mode is \"base64\"");
            }
            type = XmlUtil.getAttribute(element, "type", "");
            content = element.InnerText;
        }

        /**
        * Creates an icon for substitute()
        *
        * @param icon
        */
        private Icon(Icon icon)
        {
            mode = icon.mode;
            type = icon.type;
        }
    }
}