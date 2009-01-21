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
using System.Net;
using Pesta.Engine.gadgets.http;
using Pesta.Engine.gadgets.rewrite;
using Uri=Pesta.Engine.common.uri.Uri;

namespace Pesta.Engine.gadgets.servlet
{
    /// <summary>
    /// Summary description for ProxyHandler
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class ProxyHandler : ProxyBase
    {
        private static List<string> DISALLOWED_RESPONSE_HEADERS = new List<string>(){
                                                                                        "set-cookie", "content-length", "content-encoding", "etag", "last-modified" ,"accept-ranges",
                                                                                        "vary", "expires", "date", "pragma", "cache-control"
                                                                                    };
        private LockedDomainService lockedDomainService;
        private IContentRewriterRegistry contentRewriterRegistry;
        private readonly IHttpFetcher fetcher;
        public static readonly ProxyHandler Instance = new ProxyHandler();
        protected ProxyHandler()
        {
            //
            // TODO: Add constructor logic here
            //
            fetcher = BasicHttpFetcher.Instance;
            lockedDomainService = HashLockedDomainService.Instance;
            contentRewriterRegistry = DefaultContentRewriterRegistry.Instance;
        }

        private bool getIgnoreCache(HttpRequestWrapper request)
        {
            String ignoreCache = request.getParameter(IGNORE_CACHE_PARAM);
            if (ignoreCache == null)
            {
                return false;
            }
            return !ignoreCache.Equals("0");
        }

        public override void fetch(HttpRequestWrapper request, HttpResponseWrapper response)
        {
            if (request.getHeaders("If-Modified-Since") != null)
            {
                if (!request.isConcat)
                    response.setStatus((int)HttpStatusCode.NotModified);
                return;
            }

            String host = request.getHeaders("Host");
            if (!lockedDomainService.isSafeForOpenProxy(host))
            {
                // Force embedded images and the like to their own domain to avoid XSS
                // in gadget domains.
                return;
            }

            sRequest rcr = buildHttpRequest(request);
            sResponse results = fetcher.fetch(rcr);
            if (contentRewriterRegistry != null)
            {
                results = contentRewriterRegistry.rewriteHttpResponse(rcr, results);
            }

            if (!request.isConcat)
            {
                setResponseHeaders(request, response.getResponse(), results);
                for (int i = 0; i < results.getHeaders().Count; i++)
                {
                    String name = results.getHeaders().GetKey(i);
                    if (!DISALLOWED_RESPONSE_HEADERS.Contains(name.ToLower()))
                    {
                        foreach (String value in results.getHeaders().GetValues(i))
                        {
                            response.AddHeader(name, value);
                        }
                    }
                }
            }

            if (request.getParameter("rewriteMime") != null)
            {
                response.setContentType(request.getParameter("rewriteMime"));
            }

            if (results.getHttpStatusCode() != (int)HttpStatusCode.OK)
            {
                response.setStatus((int)results.getHttpStatusCode());
            }
            else
            {
                response.setStatus((int)HttpStatusCode.OK);
            }
            response.Write(results.responseBytes);
        }


        private sRequest buildHttpRequest(HttpRequestWrapper request)
        {
            Uri url = validateUrl(request.getParameter(URL_PARAM));

            sRequest req = new sRequest(url);

            req.Container = getContainer(request);
            if (request.getParameter(GADGET_PARAM) != null)
            {
                req.setGadget(Uri.parse(request.getParameter(GADGET_PARAM)));
            }

            // Allow the rewriter to use an externally forced mime type. This is needed
            // allows proper rewriting of <script src="x"/> where x is returned with
            // a content type like text/html which unfortunately happens all too often
            req.RewriteMimeType = request.getParameter(REWRITE_MIME_TYPE_PARAM);

            req.setIgnoreCache(getIgnoreCache(request));
            // If the proxy request specifies a refresh param then we want to force the min TTL for
            // the retrieved entry in the cache regardless of the headers on the content when it
            // is fetched from the original source.
            if (request.getParameter(REFRESH_PARAM) != null)
            {
                int ttl = 0;
                int.TryParse(request.getParameter(REFRESH_PARAM), out ttl);
                req.CacheTtl = ttl;
            }

            return req;
        }
    }
}