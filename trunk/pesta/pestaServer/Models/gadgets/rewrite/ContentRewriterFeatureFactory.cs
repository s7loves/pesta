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
using pestaServer.Models.gadgets.http;
using pestaServer.Models.gadgets.spec;
using Uri=Pesta.Engine.common.uri.Uri;
using URI = System.Uri;

namespace pestaServer.Models.gadgets.rewrite
{
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    class ContentRewriterFeatureFactory
    {
        private readonly IGadgetSpecFactory specFactory;
        private readonly String includeUrls;
        private readonly String excludeUrls;
        private readonly String expires;
        private readonly HashSet<String> includeTags;

        private ContentRewriterFeature defaultFeature;

        public readonly static ContentRewriterFeatureFactory Instance = new ContentRewriterFeatureFactory(DefaultGadgetSpecFactory.Instance, ".*", "", "86400", "link,script,embed,img,style");
        protected ContentRewriterFeatureFactory(
            IGadgetSpecFactory specFactory,
            String includeUrls,
            String excludeUrls,
            String expires,
            String includeTags) 
        {
            this.specFactory = specFactory;
            this.includeUrls = includeUrls;
            this.excludeUrls = excludeUrls;
            this.expires = expires;
            this.includeTags = new HashSet<String>();
            foreach(String s in includeTags.Split(','))
            {
                if (s != null && s.Trim().Length > 0) 
                {
                    this.includeTags.Add(s.Trim().ToLower());
                }
            }
            defaultFeature = new ContentRewriterFeature(null, includeUrls, excludeUrls, expires,
                                                        this.includeTags);
        }

        public ContentRewriterFeature getDefault()
        {
            return defaultFeature;
        }

        public ContentRewriterFeature get(sRequest request)
        {
            Uri gadgetUri = request.Gadget;
            GadgetSpec spec;
            if (gadgetUri != null)
            {
                URI gadgetJavaUri = gadgetUri.toJavaUri();
                try 
                {
                    spec = specFactory.getGadgetSpec(gadgetJavaUri, false);
                    if (spec != null) 
                    {
                        return get(spec);
                    }
                } 
                catch (GadgetException) 
                {
                    return defaultFeature;
                }
            }
            return defaultFeature;
        }

        public ContentRewriterFeature get(GadgetSpec spec)
        {
            ContentRewriterFeature rewriterFeature =
                (ContentRewriterFeature)spec.getAttribute("content-rewriter");
            if (rewriterFeature != null) 
                return rewriterFeature;
            rewriterFeature = new ContentRewriterFeature(spec, includeUrls, excludeUrls, expires, includeTags);
            spec.setAttribute("content-rewriter", rewriterFeature);
            return rewriterFeature;
        }
    }
}