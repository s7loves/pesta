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
using Jayrock.Json;
using Pesta.Utilities;
using URI = System.Uri;

namespace pestaServer.Models.gadgets.servlet
{
    /// <summary>
    /// Summary description for JsonRpcGadgetContext
    /// </summary>
    /// <remarks>
    /// <para>
    
    /// </para>
    /// </remarks>
    public class JsonRpcGadgetContext : GadgetContext
    {
        private readonly JsonObject context;
        private readonly JsonObject gadget;

        private readonly String container;
        private bool? debug;
        private bool? ignoreCache;
        private readonly Locale locale;
        private readonly string moduleId;
        private readonly RenderingContext renderingContext;
        private readonly URI url;
        private readonly UserPrefs userPrefs;
        private readonly String view;

        /**
        * @param context Request global parameters.
        * @param gadget Values for the gadget being rendered.
        * @throws JSONException If parameters can't be extracted or aren't correctly formed.
        */
        public JsonRpcGadgetContext(JsonObject context, JsonObject gadget)
        {
            this.context = context;
            this.gadget = gadget;

            url = getUrl(gadget);
            moduleId = getModuleId(gadget);
            userPrefs = getUserPrefs(gadget);
            locale = getLocale(context);
            view = context["view"] as string;
            bool ic;
            bool.TryParse(context["ignoreCache"] as string, out ic);
            ignoreCache = ic;
            container = context["container"] as string;
            bool d;
            bool.TryParse(context["debug"] as string, out d);
            debug = d;
            renderingContext = RenderingContext.METADATA;
        }

        public override String getParameter(String name)
        {
            if (gadget.Contains(name))
            {
                return gadget[name] as string;
            }
            return context[name] as string;
        }

        public override String getContainer()
        {
            if (container == null)
            {
                return base.getContainer();
            }
            return container;
        }

        public override bool getDebug()
        {
            if (debug == null)
            {
                return base.getDebug();
            }
            return debug.Value;
        }

        public override bool getIgnoreCache()
        {
            if (ignoreCache == null)
            {
                return base.getIgnoreCache();
            }
            return ignoreCache.Value;
        }

        public override Locale getLocale()
        {
            if (locale == null)
            {
                return base.getLocale();
            }
            return locale;
        }

        public override string getModuleId()
        {
            if (string.IsNullOrEmpty(moduleId))
            {
                return base.getModuleId();
            }
            return moduleId;
        }

        public override RenderingContext getRenderingContext()
        {
            if (renderingContext == null)
            {
                return base.getRenderingContext();
            }
            return renderingContext;
        }

        public override URI getUrl()
        {
            if (url == null)
            {
                return base.getUrl();
            }
            return url;
        }

        public override UserPrefs getUserPrefs()
        {
            if (userPrefs == null)
            {
                return base.getUserPrefs();
            }
            return userPrefs;
        }

        public override String getView()
        {
            if (view == null)
            {
                return base.getView();
            }
            return view;
        }

        /**
        * @param obj
        * @return The locale, if appropriate parameters are set, or null.
        */
        private static Locale getLocale(JsonObject obj)
        {
            String language = obj["language"] as string;
            String country = obj["country"] as string;
            if (language == null || country == null)
            {
                return null;
            }
            return new Locale(language, country);
        }

        /**
        * @param json
        * @return module id from the request, or null if not present
        * @throws JSONException
        */
        private static string getModuleId(JsonObject json)
        {
            if (json.Contains("moduleId"))
            {
                return json["moduleId"].ToString();
            }
            return null;
        }

        /**
        *
        * @param json
        * @return URL from the request, or null if not present
        * @throws JSONException
        */
        private static URI getUrl(JsonObject json)
        {
            try
            {
                String url = json["url"] as string;
                return new URI(url);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /**
        * @param json
        * @return UserPrefs, if any are set for this request.
        * @throws JSONException
        */
        private static UserPrefs getUserPrefs(JsonObject json)
        {
            JsonObject prefs = json["prefs"] as JsonObject;
            if (prefs == null)
            {
                return null;
            }
            Dictionary<string,string> p = new Dictionary<string,string>();
            foreach (string key in prefs.Names)
            {
                p.Add(key, (String)prefs[key]);
            }
            return new UserPrefs(p);
        }
    }
}