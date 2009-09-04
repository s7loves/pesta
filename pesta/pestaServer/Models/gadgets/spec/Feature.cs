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
using System.Collections.Generic;
using System.Xml;
using Pesta.Engine.common.xml;

namespace pestaServer.Models.gadgets.spec
{
    /// Summary description for Feature
    /// </summary>
    /// <remarks>
    /// <para>
    
    /// </para>
    /// </remarks>
    public class Feature
    {
        /**
   * Require@feature
   * Optional@feature
   */
        private readonly String name;
        public String getName() {
            return name;
        }

        /**
    * Require.Param
    * Optional.Param
    *
    * Flattened into a map where Param@name is the key and Param content is
    * the value.
    */
        private readonly Dictionary<String, String> parameters;
        public Dictionary<String, String> getParams() {
            return parameters;
        }

        /**
    * Whether this is a Require or an Optional feature.
    */
        private readonly bool required;
        public bool getRequired() {
            return required;
        }

        /**
    * Produces an xml representation of the feature.
    */
        public override String ToString() 
        {
            StringBuilder buf = new StringBuilder();
            buf.Append(required ? "<Require" : "<Optional")
                .Append(" feature=\"")
                .Append(name)
                .Append("\">");
            foreach (var entry in parameters) 
            {
                buf.Append("\n<Param name=\"")
                    .Append(entry.Key)
                    .Append("\">")
                    .Append(entry.Value)
                    .Append("</Param>");
            }
            buf.Append(required ? "</Require>" : "</Optional>");
            return buf.ToString();
        }

        /**
    * Creates a new Feature from an xml node.
    *
    * @param feature The feature to create
    * @throws SpecParserException When the Require or Optional tag is not valid
    */
        public Feature(XmlElement feature)
        {
            this.required = feature.Name.Equals("Require");
            String name = XmlUtil.getAttribute(feature, "feature");
            if (name == null) 
            {
                throw new SpecParserException(
                    (required ? "Require" : "Optional") +"@feature is required.");
            }
            this.name = name;
            XmlNodeList children = feature.GetElementsByTagName("Param");
            if (children.Count > 0) 
            {
                Dictionary<String, String> parameters = new Dictionary<String, String>(children.Count);
                for (int i = 0, j = children.Count; i < j; ++i) 
                {
                    XmlElement param = (XmlElement)children.Item(i);
                    String paramName = XmlUtil.getAttribute(param, "name");
                    if (paramName == null)
                    {
                        throw new SpecParserException("Param@name is required");
                    }
                    parameters.Add(paramName, param.InnerText);
                }
                this.parameters = parameters;
            } 
            else 
            {
                this.parameters = new Dictionary<string,string>();
            }
        }
    }
}