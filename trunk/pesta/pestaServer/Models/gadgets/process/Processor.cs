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
using Jayrock.Json;
using pestaServer.Models.common;
using pestaServer.Models.gadgets.spec;
using pestaServer.Models.gadgets.variables;

namespace pestaServer.Models.gadgets.process
{
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    public class Processor
    {
        private readonly IGadgetSpecFactory gadgetSpecFactory;
        private readonly VariableSubstituter substituter;
        private readonly ContainerConfig containerConfig;
        private readonly GadgetBlacklist blacklist;
        public static readonly Processor Instance = new Processor();
        protected Processor()
        {
            this.gadgetSpecFactory = DefaultGadgetSpecFactory.Instance;
            this.substituter = new VariableSubstituter();
            this.blacklist = new BasicGadgetBlacklist("");
            this.containerConfig = JsonContainerConfig.Instance;
        }

        /**
        * Process a single gadget. Creates a gadget from a retrieved GadgetSpec and context object,
        * automatically performing variable substitution on the spec for use elsewhere.
        *
        * @throws ProcessingException If there is a problem processing the gadget.
        */
        public Gadget process(GadgetContext context)
        {
            Uri url = context.getUrl();

            if (url == null)
            {
                throw new ProcessingException("Missing or malformed url parameter");
            }

            if (!url.Scheme.ToLower().Equals("http") && !url.Scheme.ToLower().Equals("https"))
            {
                throw new ProcessingException("Unsupported scheme (must be http or https).");
            }

            if (blacklist.isBlacklisted(context.getUrl()))
            {
                throw new ProcessingException("The requested gadget is unavailable");
            }

            try
            {
                GadgetSpec spec = gadgetSpecFactory.getGadgetSpec(context);
                spec = substituter.substitute(context, spec);

                return new Gadget()
                    .setContext(context)
                    .setSpec(spec)
                    .setCurrentView(getView(context, spec));
            } 
            catch (GadgetException e) 
            {
                throw new ProcessingException(e.Message, e);
            }
        }

        /**
        * Attempts to extract the "current" view for the given gadget.
        */
        private View getView(GadgetContext context, GadgetSpec spec) 
        {
            String viewName = context.getView();
            View view = spec.getView(viewName);
            if (view == null)
            {
                JsonArray aliases = containerConfig.getJsonArray(context.getContainer(),
                                                                 "gadgets.features/views/" + viewName + "/aliases");
                if (aliases != null)
                {
                    for (int i = 0, j = aliases.Length; i < j; ++i)
                    {
                        viewName = aliases.GetString(i);
                        if (!String.IsNullOrEmpty(viewName)) 
                        {
                            view = spec.getView(viewName);
                            if (view != null) 
                            {
                                break;
                            }
                        }
                    }
                }

                if (view == null) 
                {
                    view = spec.getView(GadgetSpec.DEFAULT_VIEW);
                }
            }
            return view;
        }
    }
}