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
using System.Data;
using System.Configuration;

using System.Web;


namespace Pesta
{
    /// <summary>
    /// Summary description for ProxyBase
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public abstract class ProxyBase
    {
        public static String URL_PARAM = "url";
        public static String REFRESH_PARAM = "refresh";
        public static String GADGET_PARAM = "gadget";
        public static String CONTAINER_PARAM = "container";
        // Old form container name, retained for legacy compatibility.
        public static String SYND_PARAM = "synd";

        // Public because of rewriter. Rewriter should be cleaned up.
        public static String REWRITE_MIME_TYPE_PARAM = "rewriteMime";
        protected String getContainer(HttpRequestWrapper request)
        {
            String container = getParameter(request, CONTAINER_PARAM, null);
            if (container == null)
            {
                container = getParameter(request, SYND_PARAM, ContainerConfig.DEFAULT_CONTAINER);
            }
            return container;
        }

        protected String getParameter(HttpRequestWrapper request, String name, String defaultValue)
        {
            String ret = request.getParameter(name);
            return ret == null ? defaultValue : ret;
        }


        protected void setResponseHeaders(HttpRequestWrapper request, HttpResponse response, sResponse results)
        {
            int refreshInterval = 0;
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
            HttpUtil.setCachingHeaders(response, refreshInterval);
            response.AddHeader("Content-Disposition", "attachment;filename=p.txt");
        }

        protected Uri validateUrl(String urlToValidate)
        {
            if (urlToValidate == null)
            {
                throw new Exception("url parameter is missing.");
            }
            try
            {
                UriBuilder url = new UriBuilder(urlToValidate);
                if (!"http".Equals(url.Scheme) && !"https".Equals(url.Scheme))
                {
                    throw new Exception("Invalid request url scheme; only " +
                        "\"http\" and \"https\" supported.");
                }
                if (url.Path == null || url.Path.Length == 0)
                {
                    url.Path = "/";
                }
                return url.Uri;
            }
            catch
            {
                throw new Exception("url parameter is not a valid url.");
            }
        }

    } 
}
