using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using Jayrock.Json;
using URI = System.Uri;

namespace Pesta
{
    public class Renderer
    {
        private readonly Processor processor;
        private readonly HtmlRenderer renderer;
        private readonly ContainerConfig containerConfig;
        private readonly LockedDomainService lockedDomainService;

        public Renderer() 
        {
            this.processor = Processor.Instance;
            this.renderer = new HtmlRenderer();
            this.containerConfig = JsonContainerConfig.Instance;
            this.lockedDomainService = HashLockedDomainService.Instance;
        }

        /**
        * Attempts to render the requested gadget.
        *
        * @return The results of the rendering attempt.
        *
        * TODO: Localize error messages.
        */
        public RenderingResults render(GadgetContext context) 
        {
            if (!validateParent(context)) 
            {
                return RenderingResults.error("Unsupported parent parameter. Check your container code.");
            }

            try 
            {
                Gadget gadget = processor.process(context);

                if (gadget.getCurrentView() == null)
                {
                    return RenderingResults.error("Unable to locate an appropriate view in this gadget. " +
                                        "Requested: '" + gadget.getContext().getView() +
                                        "' Available: " + gadget.getSpec().getViews().Keys);
                }

                if (gadget.getCurrentView().getType() == View.ContentType.URL)
                {
                    return RenderingResults.mustRedirect(getRedirect(gadget));
                }

                GadgetSpec spec = gadget.getSpec();
                if (!lockedDomainService.gadgetCanRender(context.getHost(), spec, context.getContainer()))
                {
                    return RenderingResults.mustRedirect(getRedirect(gadget));
                }
                return RenderingResults.ok(renderer.render(gadget));
            }
            catch (RenderingException e) 
            {
                return logError(context.getUrl(), e);
            } 
            catch (ProcessingException e) 
            {
                return logError(context.getUrl(), e);
            } 
            catch (Exception e) 
            {
                if (e.GetBaseException() is GadgetException) 
                {
                    return logError(context.getUrl(), e.GetBaseException());
                }
                throw e;
            }
        }

        private RenderingResults logError(URI gadgetUrl, Exception t)
        {
            return RenderingResults.error(t.Message);
        }

        /**
        * Validates that the parent parameter was acceptable.
        *
        * @return True if the parent parameter is valid for the current container.
        */
        private bool validateParent(GadgetContext context) 
        {
            String container = context.getContainer();
            String parent = context.getParameter("parent");

            if (parent == null) 
            {
                // If there is no parent parameter, we are still safe because no
                // dependent code ever has to trust it anyway.
                return true;
            }

            try
            {
                JsonArray parents = containerConfig.getJsonArray(container, "gadgets.parent");
                if (parents == null) 
                {
                    return true;
                }
                // We need to check each possible parent parameter against this regex.
                for (int i = 0, j = parents.Length; i < j; ++i) 
                {
                    if (Regex.IsMatch(parents[i] as string, parent))
                    {
                        return true;
                    }
                }
            } 
            catch (JsonException e) 
            {

            }
            return false;
        }

        private Uri getRedirect(Gadget gadget) 
        {
            // TODO: This should probably just call UrlGenerator.getIframeUrl(), but really it should
            // never happen.
            View view = gadget.getCurrentView();
            if (view.getType() == View.ContentType.URL)
            {
                return gadget.getCurrentView().getHref();
            }
            // TODO
            return null;
        }
    }
}
