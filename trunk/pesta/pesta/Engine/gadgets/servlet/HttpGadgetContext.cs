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
using System.Web;
using System.Collections.Generic;
using System.Collections.Specialized;
using Pesta.Engine.auth;
using Pesta.Utilities;
using URI = System.Uri;

namespace Pesta.Engine.gadgets.servlet
{
    /// <summary>
    /// Summary description for HttpGadgetContext
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class HttpGadgetContext : GadgetContext
    {
        public const String USERPREF_PARAM_PREFIX = "up_";
        private HttpContext context;
        private HttpRequest request;
        private String container;
        private bool? debug;
        private bool ignoreCache;
        private Locale locale;
        private int moduleId;
        private RenderingContext renderingContext;
        private URI url;
        private UserPrefs userPrefs;
        private String view;

        public HttpGadgetContext(HttpContext context)
        {
            this.request = context.Request;
            this.context = context;

            container = getContainer(request);
            debug = getDebug(request);
            ignoreCache = getIgnoreCache(request);
            locale = getLocale(request);
            moduleId = getModuleId(request);
            renderingContext = getRenderingContext(request);
            url = getUrl(request);
            userPrefs = getUserPrefs(request);
            view = getView(request);
        }


        public override String getParameter(String name)
        {
            return request.Params[name];
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


        public override String getHost() 
        {
            String host = request.Headers["Host"];
            if (host == null) 
            {
                return base.getHost();
            }
            return host;
        }

        
        public override String getUserIp() 
        {
            String ip = request.ServerVariables["REMOTE_ADDR"];
            if (ip == null)
            {
                return base.getUserIp();
            }
            return ip;
        }

        public override bool getIgnoreCache()
        {
            //if (ignoreCache == null)
            //{
            //     return base.getIgnoreCache();
            //}
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

        public override SecurityToken getToken()
        {
            return new AuthInfo(context, this.request.RawUrl).getSecurityToken();
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

        /// <param name="request">
        /// </param>
        /// <returns> The container, if set, or null.
        /// </returns>
        private static String getContainer(HttpRequest request)
        {
            String container = request.Params["container"];
            if (container == null)
            {
                // The parameter used to be called 'synd' FIXME: schedule removal
                container = request.Params["synd"];
            }
            return container;
        }

        /// <param name="request">
        /// </param>
        /// <returns> Debug setting, if set, or null.
        /// </returns>
        private static bool getDebug(HttpRequest request)
        {
            if (PestaSettings.GadgetDebug.ToLower().Equals("true"))
            {
                return true;
            }
            String debug = request.Params["debug"];
            if (debug == null)
            {
                return false;
            }
            else if ("0".Equals(debug))
            {
                return false;
            }
            return true;
        }

        /// <param name="request">
        /// </param>
        /// <returns> The ignore cache setting, if appropriate params are set, or null.
        /// </returns>
        private static bool getIgnoreCache(HttpRequest request)
        {
            String ignoreCache = request.Params["nocache"];
            if (ignoreCache == null)
            {
                return false;
            }
            else if ("0".Equals(ignoreCache))
            {
                return false;
            }
            return true;
        }

        /// <param name="request">
        /// </param>
        /// <returns> The locale, if appropriate parameters are set, or null.
        /// </returns>
        private static Locale getLocale(HttpRequest request)
        {
            String language = request.Params["lang"];
            String country = request.Params["country"];
            if (language == null && country == null)
            {
                return null;
            }
            else if (language == null)
            {
                language = "all";
            }
            else if (country == null)
            {
                country = "ALL";
            }
            return new Locale(language, country);
        }

        /// <param name="request">
        /// </param>
        /// <returns> module id, if specified
        /// </returns>
        private static int getModuleId(HttpRequest request)
        {
            String mid = request.Params["mid"];
            if (mid == null)
            {
                return 0;
            }
            return int.Parse(mid);
        }

        /// <param name="request">
        /// </param>
        /// <returns> The rendering context, if appropriate params are set, or null.
        /// </returns>
        private static RenderingContext getRenderingContext(HttpRequest request)
        {
            String c = request.Params["c"];
            if (c == null)
            {
                return null;
            }
            return c.Equals("1") ? RenderingContext.CONTAINER : RenderingContext.GADGET;
        }

        /// <param name="request">
        /// </param>
        /// <returns> The ignore cache setting, if appropriate params are set, or null.
        /// </returns>
        private static URI getUrl(HttpRequest request)
        {
            string[] url = request.Params.GetValues("url");
            if (url == null)
            {
                return null;
            }
            return new URI(url[0]);
        }

        /// <param name="req">
        /// </param>
        /// <returns> UserPrefs, if any are set for this request.
        /// </returns>
        private static UserPrefs getUserPrefs(HttpRequest req)
        {
            Dictionary<string,string> prefs = new Dictionary<string,string>();
            NameValueCollection paramNames = req.Params;
            if (paramNames == null)
            {
                return null;
            }
            foreach (string paramName in paramNames)
            {
                if (paramName.StartsWith(USERPREF_PARAM_PREFIX))
                {
                    string prefName = paramName.Substring(USERPREF_PARAM_PREFIX.Length);
                    prefs.Add(prefName, req.Params[paramName]);
                }
            }
            return new UserPrefs(prefs);
        }

        /// <param name="request">
        /// </param>
        /// <returns> The view, if specified, or null.
        /// </returns>
        private static String getView(HttpRequest request)
        {
            return request.Params["view"];
        }
    }
}