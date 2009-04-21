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
using System.Text.RegularExpressions;
using Jayrock.Json;
using Pesta.Engine.common;
using pestaServer.Models.common;
using pestaServer.Models.gadgets.process;
using pestaServer.Models.gadgets.spec;
using Uri=Pesta.Engine.common.uri.Uri;
using URI = System.Uri;
using System.Linq;

namespace pestaServer.Models.gadgets.render
{
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    public class Renderer
    {
        private readonly Processor processor;
        private readonly HtmlRenderer renderer;
        private readonly ContainerConfig containerConfig;
        private readonly LockedDomainService lockedDomainService;

        public Renderer() 
        {
            processor = Processor.Instance;
            renderer = new HtmlRenderer();
            containerConfig = JsonContainerConfig.Instance;
            lockedDomainService = HashLockedDomainService.Instance;
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
                                                  "' Available: " + String.Join(",",gadget.getSpec().getViews().Keys.ToArray()));
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
                    if (Regex.IsMatch(parents[i].ToString(), parent))
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