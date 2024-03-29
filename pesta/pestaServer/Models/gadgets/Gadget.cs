﻿#region License, Terms and Conditions
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

using pestaServer.Models.gadgets.preload;
using pestaServer.Models.gadgets.spec;

namespace pestaServer.Models.gadgets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    
    /// </para>
    /// </remarks>
    public class Gadget
    {
        private GadgetContext context;
        private GadgetSpec spec;
        private IPreloads preloads;
        private View currentView;
        /**
         * @param context The request that the gadget is being processed for.
         */
        public Gadget setContext(GadgetContext context)
        {
            this.context = context;
            return this;
        }

        public GadgetContext getContext()
        {
            return context;
        }

        /**
         * @param spec The spec for the gadget that is being processed.
         */
        public Gadget setSpec(GadgetSpec spec)
        {
            this.spec = spec;
            return this;
        }

        public GadgetSpec getSpec()
        {
            return spec;
        }

        /**
         * @param preloads The preloads for the gadget that is being processed.
         */
        public Gadget setPreloads(IPreloads preloads)
        {
            this.preloads = preloads;
            return this;
        }

        public IPreloads getPreloads()
        {
            return preloads;
        }

        public Gadget setCurrentView(View currentView)
        {
            this.currentView = currentView;
            return this;
        }

        /**
         * @return The View applicable for the current request.
         */
        public View getCurrentView()
        {
            return currentView;
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

    }
}