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
using org.apache.shindig.gadgets;
using URI = java.net.URI;
using Jayrock.Json;
using Locale=java.util.Locale;

namespace Pesta
{
    /// <summary>
    /// Summary description for JsonRpcGadgetContext
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class JsonRpcGadgetContext : GadgetContext
    {
        private JsonObject context;
        private JsonObject gadget;

        private String container;
        private bool debug;
        private bool ignoreCache;
        private Locale locale;
        private int moduleId;
        private RenderingContext renderingContext;
        private URI url;
        private UserPrefs userPrefs;
        private String view;

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
            bool.TryParse(context["ignoreCache"] as string, out ignoreCache);
            container = context["container"] as string;
            bool.TryParse(context["debug"] as string, out debug);
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
            if (debug == false)
            {
                return base.getDebug();
            }
            return debug;
        }

        public override bool getIgnoreCache()
        {
            if (ignoreCache == false)
            {
                return base.getIgnoreCache();
            }
            return ignoreCache;
        }

        public override Locale getLocale()
        {
            if (locale == null)
            {
                return base.getLocale();
            }
            return locale;
        }

        public override int getModuleId()
        {
            if (moduleId == 0)
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
        private static int getModuleId(JsonObject json)
        {
            if (json.Contains("moduleId"))
            {
                int val = 0;
                int.TryParse(json["moduleId"].ToString(), out val);
                return val;
            }
            return 0;
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
            catch (UriFormatException e)
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
            java.util.Map p = new java.util.HashMap();
            foreach (string key in prefs.Names)
            {
                p.put(key, prefs[key]);
            }
            return new UserPrefs(p);
        }
    } 
}
