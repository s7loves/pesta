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
using pestaServer.Models.common;
using pestaServer.Models.gadgets.http;
using Uri=Pesta.Engine.common.uri.Uri;
using UriBuilder=Pesta.Engine.common.uri.UriBuilder;
using HttpRequestWrapper = pestaServer.Models.gadgets.http.HttpRequestWrapper;
using HttpResponseWrapper = pestaServer.Models.gadgets.http.HttpResponseWrapper;

namespace pestaServer.Models.gadgets.servlet
{
    /// <summary>
    /// Summary description for ProxyBase
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public abstract class ProxyBase
    {
        public const String URL_PARAM = "url";
        public const String REFRESH_PARAM = "refresh";
        public const String IGNORE_CACHE_PARAM = "nocache";
        public const String GADGET_PARAM = "gadget";
        public const String CONTAINER_PARAM = "container";
        // Old form container name, retained for legacy compatibility.
        public const String SYND_PARAM = "synd";

        // Public because of rewriter. Rewriter should be cleaned up.
        public const String REWRITE_MIME_TYPE_PARAM = "rewriteMime";
        protected String getContainer(HttpRequestWrapper request)
        {
            String container = GetParameter(request, CONTAINER_PARAM, null) ??
                               GetParameter(request, SYND_PARAM, ContainerConfig.DEFAULT_CONTAINER);
            return container;
        }

        protected static String GetParameter(HttpRequestWrapper request, String name, String defaultValue)
        {
            String ret = request.getParameter(name);
            return ret ?? defaultValue;
        }


        protected static void SetResponseHeaders(HttpRequestWrapper request, HttpResponse response, sResponse results)
        {
            int refreshInterval;
            if (results.isStrictNoCache())
            {
                refreshInterval = 0;
            }
            else if (request.getParameter(REFRESH_PARAM) != null)
            {
                int.TryParse(request.getParameter(REFRESH_PARAM), out refreshInterval);
            }
            else
            {
                refreshInterval = Math.Max(60 * 60, (int)(results.getCacheTtl() / 1000L));
            }
            HttpUtil.SetCachingHeaders(response, refreshInterval);
            // We're skipping the content disposition header for flash due to an issue with Flash player 10
            // This does make some sites a higher value phishing target, but this can be mitigated by
            // additional referer checks.
            if (!results.getHeader("Content-Type").ToLower().Equals("application/x-shockwave-flash"))
            {
                response.AddHeader("Content-Disposition", "attachment;filename=p.txt");
            }
        }

        protected static Uri ValidateUrl(String urlToValidate)
        {
            if (urlToValidate == null)
            {
                throw new Exception("url parameter is missing.");
            }
            try
            {
                UriBuilder url = UriBuilder.parse(urlToValidate);
                if (!"http".Equals(url.getScheme()) && !"https".Equals(url.getScheme()))
                {
                    throw new GadgetException(GadgetException.Code.INVALID_PARAMETER,
                                              "Invalid request url scheme in url: " + HttpUtility.UrlEncode(urlToValidate) +
                                              "; only \"http\" and \"https\" supported.");
                }
                if (string.IsNullOrEmpty(url.getPath()))
                {
                    url.setPath("/");
                }
                return url.toUri();
            }
            catch
            {
                throw new Exception("url parameter is not a valid url.");
            }
        }

        /**
           * Processes the given request.
           */
        abstract public void Fetch(HttpRequestWrapper request, HttpResponseWrapper response);

    }
}