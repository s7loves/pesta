using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

namespace Pesta
{
    public class HtmlRenderer
    {
        private readonly ContentFetcherFactory fetcher;
        private readonly PreloaderService preloader;
        private readonly ContentRewriterRegistry rewriter;

        public HtmlRenderer() 
        {
            this.fetcher = ContentFetcherFactory.Instance;
            this.preloader = new ConcurrentPreloaderService();
            this.rewriter = DefaultContentRewriterRegistry.Instance;
        }

        /**
        * Render the gadget into a string by performing the following steps:
        *
        * - Retrieve gadget specification information (GadgetSpec, MessageBundle, etc.)
        *
        * - Fetch any preloaded data needed to handle the request, as handled by Preloader.
        *
        * - Perform rewriting operations on the output content, handled by Rewriter.
        *
        * @param gadget The gadget for the rendering operation.
        * @return The rendered gadget content
        * @throws RenderingException if any issues arise that prevent rendering.
        */
        public String render(Gadget gadget) 
        {
            try 
            {
                View view = gadget.getCurrentView();
                GadgetContext context = gadget.getContext();
                GadgetSpec spec = gadget.getSpec();

                Preloads preloads = preloader.preload(context, spec);
                gadget.setPreloads(preloads);

                if (view.getHref() == null) 
                {
                    return rewriter.rewriteGadget(gadget, view.getContent());
                } 
                else
                {
                    // TODO: Add current url to GadgetContext to support transitive proxying.
                    UriBuilder uri = new UriBuilder(view.getHref());
                    uri.addQueryParameter("lang", context.getLocale().getLanguage());
                    uri.addQueryParameter("country", context.getLocale().getCountry());

                    sRequest request = new sRequest(uri.toUri())
                                            .setIgnoreCache(context.getIgnoreCache())
                                            .setOAuthArguments(new OAuthArguments(view))
                                            .setAuthType(view.getAuthType())
                                            .setSecurityToken(context.getToken())
                                            .setContainer(context.getContainer())
                                            .setGadget(spec.getUrl());
                    sResponse response = fetcher.fetch(request);
                    if (response.getHttpStatusCode() != (int)HttpStatusCode.OK)
                    {
                        throw new RenderingException("Unable to reach remote host. HTTP status " +
                        response.getHttpStatusCode());
                    }
                    return rewriter.rewriteGadget(gadget, response.responseString);
                }   
            }
            catch (GadgetException e)
            {
                throw new RenderingException(e.Message, e);
            }
        }
    }
}
