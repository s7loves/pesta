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
using System.Xml;
using System.Collections.Generic;
using System.Text;
using Uri=Pesta.Engine.common.uri.Uri;

namespace Pesta.Engine.gadgets.spec
{
    /// <summary>
    /// Summary description for OAuthSpec
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class OAuthSpec
    {
        /** Keys are service names, values are service descriptors */
        private readonly Dictionary<String, OAuthService> serviceMap;

        public OAuthSpec(XmlElement element, Uri _base)
        {
            serviceMap = new Dictionary<String, OAuthService>();
            XmlNodeList services = element.GetElementsByTagName("Service");
            for (int i = 0; i < services.Count; ++i)
            {
                XmlNode node = services[i];
                if (node.NodeType == XmlNodeType.Element)
                {
                    parseService((XmlElement)node, _base);
                }
            }
        }

        private void parseService(XmlElement serviceElement, Uri _base)
        {
            OAuthService service = new OAuthService(serviceElement, _base);
            serviceMap.Add(service.getName(), service);
        }

        public Dictionary<String, OAuthService> getServices()
        {
            return serviceMap;
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<OAuth>");
            foreach (String name in serviceMap.Keys)
            {
                sb.Append("<Service name='");
                sb.Append(name);
                sb.Append("'>");
                OAuthService service = serviceMap[name];
                sb.Append(service.getRequestUrl().ToString("Request"));
                sb.Append(service.getAccessUrl().ToString("Access"));
                sb.Append("<Authorization url='" +
                          service.getAuthorizationUrl().ToString() + "'/>");
                sb.Append("</Service>");
            }
            sb.Append("</OAuth>");
            return sb.ToString();
        }
    }
}