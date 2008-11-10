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
using System.Net;
using System.IO;
using System.Collections.Generic;


namespace Pesta
{
    /// <summary>
    /// Summary description for BasicGadgetSpecFactory
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class BasicGadgetSpecFactory : GadgetSpecFactory
    {
        private HttpFetcher fetcher;
        public static readonly BasicGadgetSpecFactory Instance = new BasicGadgetSpecFactory();
        protected BasicGadgetSpecFactory()
        {
            fetcher = BasicHttpFetcher.Instance;
            //
            // TODO: Add constructor logic here
            //
        }

        public GadgetSpec getGadgetSpec(GadgetContext context)
        {
            return getGadgetSpec(context.getUrl(), context.getIgnoreCache());
        }
        public GadgetSpec getGadgetSpec(Uri gadgetUri, bool ignoreCache)
        {
            if (ignoreCache)
            {
                return fetchObjectAndCache(gadgetUri, ignoreCache);
            }

            // add item to cache
            GadgetSpec cached = HttpRuntime.Cache[gadgetUri.ToString()] as GadgetSpec;
            if (cached == null)
            {
                return fetchObjectAndCache(gadgetUri, ignoreCache);
            }

            return cached;
        }

        private GadgetSpec fetchObjectAndCache(Uri url, bool ignoreCache)
        {
            sRequest request = new sRequest(url).SetIgnoreCache(false);
            sResponse response = fetcher.fetch(request);
            if (response.getHttpStatusCode() != HttpStatusCode.OK)
            {
                throw new GadgetException(GadgetException.Code.FAILED_TO_RETRIEVE_CONTENT,
                            "Unable to retrieve gadget xml. HTTP error " +
                            (int)response.getHttpStatusCode());
            }

            GadgetSpec spec = new GadgetSpec(url, response.responseString);
            // Find the type=HTML views that link to their content externally.
            List<View> hrefViewList = new List<View>();
            foreach (View v in spec.getViews().Values)
            {
                if (v.getType() != View.ContentType.URL && v.getHref() != null)
                {
                    hrefViewList.Add(v);
                }
            }

            // Retrieve all external view contents simultaneously.
            foreach (View v in hrefViewList)
            {
                HttpWebRequest req = WebRequest.Create(v.getHref().ToString()) as HttpWebRequest;
                using (HttpWebResponse response2 = req.GetResponse() as HttpWebResponse)
                {
                    if (response2.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("No response from: " + url.ToString());
                    }
                    using (StreamReader reader = new StreamReader(response2.GetResponseStream()))
                    {
                        v.setHrefContent(reader.ReadToEnd());
                    }
                }
            }
            foreach (View v in spec.getViews().Values)
            {
                if (v.getType() != View.ContentType.URL)
                {
                    // A non-null href at this point indicates that the retrieval of remote
                    // content has failed.
                    if (v.getHref() != null)
                    {
                        throw new Exception("Unable to retrieve remote gadget content.");
                    }
                }
            }
            HttpRuntime.Cache.Insert(url.ToString(), spec, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(5));
            return spec;
        }
    } 
}