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
using URI = java.net.URI;
using System.Collections.Generic;
using System.Xml;

namespace Pesta
{
    /// <summary>
    /// Summary description for OAuthService
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class OAuthService
    {
        private EndPoint requestUrl;
        private EndPoint accessUrl;
        private URI authorizationUrl;
        private String name;

        /**
        * Represents /OAuth/Service/Request elements.
        */
        public EndPoint getRequestUrl()
        {
            return requestUrl;
        }

        /**
        * Represents /OAuth/Service/Access elements.
        */
        public EndPoint getAccessUrl()
        {
            return accessUrl;
        }
        /**
        * Represents /OAuth/Service/Authorization elements.
        */
        public URI getAuthorizationUrl()
        {
            return authorizationUrl;
        }

        /**
        * Represents /OAuth/Service@name
        */
        public String getName()
        {
            return name;
        }

        /**
        * Method to use for requests to an OAuth request token or access token URL.
        */
        public class Method : EnumBaseType<Method>
        {
            /// <summary>
            /// Initializes a new instance of the Method class.
            /// </summary>
            public Method(int key, string value)
                : base(key, value)
            {
            }
            public static readonly Method GET = new Method(1, "GET");
            public static readonly Method POST = new Method(2, "POST");

            private static Dictionary<String, Method> METHODS
                = new Dictionary<String, Method>() { { "GET", Method.GET }, { "POST", Method.POST }, { "", Method.GET } };

            public static Method Parse(String value)
            {
                value = value.Trim();
                Method result = METHODS[value];
                if (result == null)
                {
                    throw new SpecParserException("Unknown OAuth method: " + value);
                }
                return result;
            }
        }

        /**
        * Location for OAuth parameters in requests to an OAuth request token,
        * access token, or resource URL.
        */
        public class Location : EnumBaseType<Location>
        {
            public Location(int key, string value)
                : base(key, value)
            {
                this.locationString = value;
            }
            public static readonly Location HEADER = new Location(1, "auth-header");
            public static readonly Location URL = new Location(2, "uri-query");
            public static readonly Location BODY = new Location(3, "post-body");

            private static Dictionary<String, Location> LOCATIONS
                = new Dictionary<string, Location>() { { HEADER.ToString(), HEADER }, { URL.ToString(), URL }, { BODY.ToString(), BODY }, { "", HEADER } };

            private String locationString;

            public override String ToString()
            {
                return locationString;
            }

            public static Location Parse(String value)
            {
                value = value.Trim();
                Location result = LOCATIONS[value];
                if (result == null)
                {
                    throw new SpecParserException("Unknown OAuth param_location: " + value);
                }
                return result;
            }
        }

        private static readonly String URL_ATTR = "url";
        private static readonly String PARAM_LOCATION_ATTR = "param_location";
        private static readonly String METHOD_ATTR = "method";

        /**
        * Description of an OAuth request token or access token URL.
        */
        public class EndPoint
        {
            public readonly URI url;
            public readonly Method method;
            public readonly Location location;

            public EndPoint(URI url, Method method, Location location)
            {
                this.url = url;
                this.method = method;
                this.location = location;
            }

            public String ToString(String element)
            {
                return '<' + element + " url='" + url.toString() + "' " +
                        "method='" + method + "' param_location='" + location + "'/>";
            }
        }

        public OAuthService(XmlElement serviceElement)
        {
            name = serviceElement.Attributes["name"].Value;
            XmlNodeList children = serviceElement.ChildNodes;
            for (int i = 0; i < children.Count; ++i)
            {
                XmlNode child = children[i];
                if (child.NodeType != XmlNodeType.Element)
                {
                    continue;
                }
                String childName = child.Name;
                if (childName.Equals("Request"))
                {
                    if (requestUrl != null)
                    {
                        throw new SpecParserException("Multiple OAuth/Service/Request elements");
                    }
                    requestUrl = parseEndPoint("OAuth/Service/Request", (XmlElement)child);
                }
                else if (childName.Equals("Authorization"))
                {
                    if (authorizationUrl != null)
                    {
                        throw new SpecParserException("Multiple OAuth/Service/Authorization elements");
                    }
                    authorizationUrl = parseAuthorizationUrl((XmlElement)child);
                }
                else if (childName.Equals("Access"))
                {
                    if (accessUrl != null)
                    {
                        throw new SpecParserException("Multiple OAuth/Service/Access elements");
                    }
                    accessUrl = parseEndPoint("OAuth/Service/Access", (XmlElement)child);
                }
            }
            if (requestUrl == null)
            {
                throw new SpecParserException("/OAuth/Service/Request is required");
            }
            if (accessUrl == null)
            {
                throw new SpecParserException("/OAuth/Service/Access is required");
            }
            if (authorizationUrl == null)
            {
                throw new SpecParserException("/OAuth/Service/Authorization is required");
            }
            if (requestUrl.location != accessUrl.location)
            {
                throw new SpecParserException(
                            "Access@location must be identical to Request@location");
            }
            if (requestUrl.method != accessUrl.method)
            {
                throw new SpecParserException(
                            "Access@method must be identical to Request@method");
            }
            if (requestUrl.location == Location.BODY &&
                            requestUrl.method == Method.GET)
            {
                throw new SpecParserException("Incompatible parameter location, cannot" +
                                    "use post-body with GET requests");
            }
        }

        /**
        * Constructor for testing only.
        */
        OAuthService()
        {
        }

        URI parseAuthorizationUrl(XmlElement child)
        {
            URI url = XmlUtil.getHttpUriAttribute(child, URL_ATTR);
            if (url == null)
            {
                throw new SpecParserException("OAuth/Service/Authorization @url is not valid: " +
                                    child.Attributes[URL_ATTR]);
            }
            return url;
        }


        EndPoint parseEndPoint(String where, XmlElement child)
        {
            URI url = XmlUtil.getHttpUriAttribute(child, URL_ATTR);
            if (url == null)
            {
                throw new SpecParserException("Not an HTTP url: " + child.Attributes[URL_ATTR]);
            }
            Location location = Location.Parse(child.Attributes[PARAM_LOCATION_ATTR].Value);
            Method method = Method.Parse(child.Attributes[METHOD_ATTR].Value);
            return new EndPoint(url, method, location);
        }
    } 
}
