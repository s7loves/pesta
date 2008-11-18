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

namespace Pesta
{
    /// <summary>
    /// Summary description for LinkSpec
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class LinkSpec
    {
        private readonly Uri _base;

        public LinkSpec(XmlElement element, Uri _base)
        {
            this._base = _base;
            rel = XmlUtil.getAttribute(element, "rel");
            if (rel == null)
            {
                throw new SpecParserException("Link/@rel is required!");
            }
            href = XmlUtil.getUriAttribute(element, "href");
            if (href == null)
            {
                throw new SpecParserException("Link/@href is required!");
            }
        }

        private LinkSpec(LinkSpec rhs, Substitutions substitutions)
        {
            rel = substitutions.substituteString(null, rhs.rel);
            _base = rhs._base;
            href = _base.resolve(substitutions.substituteUri(null, rhs.href));
        }

        /**
        * Link/@rel
        */
        private readonly String rel;
        public String getRel()
        {
            return rel;
        }

        /**
        * Link/@href
        */
        private readonly Uri href;
        public Uri getHref()
        {
            return href;
        }

        /**
        * Performs variable substitution on all visible elements.
        */
        public LinkSpec substitute(Substitutions substitutions)
        {
            return new LinkSpec(this, substitutions);
        }

        public override String ToString() 
        {
            return "<Link rel='" + rel + "' href='" + href.ToString() + "'/>";
        }
    } 
}
