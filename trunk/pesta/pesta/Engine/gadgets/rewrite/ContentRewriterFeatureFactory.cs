using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using URI = System.Uri;

namespace Pesta
{
    class ContentRewriterFeatureFactory
    {
        private readonly GadgetSpecFactory specFactory;
        private readonly String includeUrls;
        private readonly String excludeUrls;
        private readonly String expires;
        private readonly HashSet<String> includeTags;

        private ContentRewriterFeature defaultFeature;

        public readonly static ContentRewriterFeatureFactory Instance = new ContentRewriterFeatureFactory(DefaultGadgetSpecFactory.Instance, ".*", "", "86400", "link,script,embed,img,style");
        protected ContentRewriterFeatureFactory(
                GadgetSpecFactory specFactory,
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
                catch (GadgetException ge) 
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
