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
using System.Text;
using System.Web;
using Jayrock.Json;
using org.apache.shindig.gadgets;
using org.apache.shindig.gadgets.parse;
using System.Runtime.Remoting.Messaging;

namespace Pesta
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class Gadget : MutableContent
    {
        private GadgetContext context;
        public GadgetContext Context
        {
            get
            {
                return context;
            }
        }

        private GadgetSpec spec;
        public GadgetSpec Spec
        {
            get
            {
                return spec;
            }
        }

        private ICollection<JsLibrary> jsLibraries;
        public ICollection<JsLibrary> JsLibraries
        {
            get
            {
                return jsLibraries;
            }
        }

        private Dictionary<Preload, IAsyncResult> preloads = new Dictionary<Preload, IAsyncResult>();
        public Dictionary<Preload, IAsyncResult> Preloads
        {
            get
            {
                return preloads;
            }
        }

        /**
        * Convenience function for getting the locale spec for the current context.
        *
        * Identical to:
        * Locale locale = gadget.getContext().getLocale();
        * gadget.getSpec().getModulePrefs().getLocale(locale);
        */
        public LocaleSpec getLocale()
        {
            return spec.getModulePrefs().getLocale(context.getLocale());
        }

        private View currentView;
        public View CurrentView
        {
            get
            {
                return currentView;
            }
        }
        /**
        * Attempts to extract the "current" view for this gadget.
        *
        * @param config The container configuration; used to look for any view name
        *        aliases for the container specified in the context.
        */
        View getView(ContainerConfig config)
        {
            String viewName = context.getView();
            View view = spec.getView(viewName);
            if (view == null)
            {
                JsonArray aliases = config.getJsonArray(context.getContainer(),
                        "gadgets.features/views/" + viewName + "/aliases");
                if (aliases != null)
                {
                    try
                    {
                        for (int i = 0; i < aliases.Length; i++)
                        {
                            viewName = aliases.GetString(i);
                            view = spec.getView(viewName);
                            if (view != null)
                            {
                                break;
                            }
                        }
                    }
                    catch (JsonException e)
                    {
                        view = null;
                    }
                }

                if (view == null)
                {
                    view = spec.getView(GadgetSpec.DEFAULT_VIEW);
                }
            }
            return view;
        }
        public Gadget(GadgetContext context, GadgetSpec spec,
            ICollection<JsLibrary> jsLibraries, ContainerConfig containerConfig, GadgetHtmlParser contentParser)
            : base(contentParser)
        {
            this.context = context;
            this.spec = spec;
            this.jsLibraries = jsLibraries;
            this.currentView = getView(containerConfig);
            if (this.currentView != null)
            {
                // View might be invalid or associated with no content (type=URL)
                setContent(this.currentView.getContent());
            }
            else
            {
                setContent(null);
            }
        }
    } 
}

