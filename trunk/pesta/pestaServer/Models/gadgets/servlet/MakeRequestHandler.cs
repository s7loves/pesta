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
using System.Net;
using Jayrock.Json;
using System.Text;
using Pesta.Engine.auth;
using pestaServer.Models.gadgets.http;
using pestaServer.Models.gadgets.oauth;
using pestaServer.Models.gadgets.rewrite;
using Uri=Pesta.Engine.common.uri.Uri;
using HttpRequestWrapper = pestaServer.Models.gadgets.http.HttpRequestWrapper;
using HttpResponseWrapper = pestaServer.Models.gadgets.http.HttpResponseWrapper;

namespace pestaServer.Models.gadgets.servlet
{
    /// <summary>
    /// Summary description for MakeRequestHandler
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class MakeRequestHandler : ProxyBase
    {
        private const string UNPARSEABLE_CRUFT = "throw 1; < don't be evil' >";
        private const string POST_DATA_PARAM = "postData";
        private const string METHOD_PARAM = "httpMethod";
        private const string HEADERS_PARAM = "headers";
        private const string NOCACHE_PARAM = "nocache";
        private const string AUTHZ_PARAM = "authz";
        private const string CONTENT_TYPE_PARAM = "contentType";
        private const string NUM_ENTRIES_PARAM = "numEntries";
        private const string DEFAULT_NUM_ENTRIES = "3";
        private const string GET_SUMMARIES_PARAM = "getSummaries";

        private readonly IRequestPipeline requestPipeline;
        private readonly IContentRewriterRegistry contentRewriterRegistry;
        public readonly static MakeRequestHandler Instance = new MakeRequestHandler();
        protected MakeRequestHandler()
        {
            contentRewriterRegistry = DefaultContentRewriterRegistry.Instance;
            this.requestPipeline = DefaultRequestPipeline.Instance;
        }

        public override void Fetch(HttpRequestWrapper request, HttpResponseWrapper response)
        {
            sRequest rcr = buildHttpRequest(request);

            // Serialize the response
            sResponse results = requestPipeline.execute(rcr);

            // Rewrite the response
            if (contentRewriterRegistry != null)
            {
                results = contentRewriterRegistry.rewriteHttpResponse(rcr, results);
            }

            // Serialize the response
            String output = convertResponseToJson(rcr.getSecurityToken(), request, results);

            // Find and set the refresh interval
            SetResponseHeaders(request, response.getResponse(), results);

            response.setStatus((int)HttpStatusCode.OK);
            response.setContentType("application/json");
            response.getResponse().ContentEncoding = Encoding.UTF8;
            response.Write(Encoding.UTF8.GetBytes(UNPARSEABLE_CRUFT + output));
        }

        /**
        * Generate a remote content request based on the parameters
        * sent from the client.
        * @throws GadgetException
        */
        private sRequest buildHttpRequest(HttpRequestWrapper request)
        {
            Uri url = ValidateUrl(request.getParameter(URL_PARAM));

            sRequest req = new sRequest(url)
                .setMethod(GetParameter(request, METHOD_PARAM, "GET"))
                .setPostBody(request.getRequest().ContentEncoding.GetBytes(GetParameter(request, POST_DATA_PARAM, "")))
                .setContainer(getContainer(request));

            String headerData = GetParameter(request, HEADERS_PARAM, "");
            if (headerData.Length > 0)
            {
                String[] headerList = headerData.Split('&');
                foreach(String header in headerList) 
                {
                    String[] parts = header.Split('=');
                    if (parts.Length != 2)
                    {
                        throw new GadgetException(GadgetException.Code.INTERNAL_SERVER_ERROR,
                                                  "Malformed header specified,");
                    }
                    req.addHeader(HttpUtility.UrlDecode(parts[0]), HttpUtility.UrlDecode(parts[1]));
                }
            }

            //removeUnsafeHeaders(req);

            req.setIgnoreCache("1".Equals(request.getParameter(NOCACHE_PARAM)));

            if (request.getParameter(GADGET_PARAM) != null)
            {
                req.Gadget = Uri.parse(request.getParameter(GADGET_PARAM));
            }

            // Allow the rewriter to use an externally forced mime type. This is needed
            // allows proper rewriting of <script src="x"/> where x is returned with
            // a content type like text/html which unfortunately happens all too often
            req.setRewriteMimeType(request.getParameter(REWRITE_MIME_TYPE_PARAM));

            // Figure out whether authentication is required
            AuthType auth = AuthType.Parse(GetParameter(request, AUTHZ_PARAM, null));
            req.AuthType = auth;
            if (auth != AuthType.NONE)
            {
                req.setSecurityToken(extractAndValidateToken(request.getContext()));
                req.setOAuthArguments(new OAuthArguments(auth, request.getRequest()));
            }
            return req;
        }


        /**
       * Removes unsafe headers from the header set.
       */
        private static void removeUnsafeHeaders(sRequest request) 
        {
            // Host must be removed.
            String[] badHeaders = new[] 
                                      {
                                          // No legitimate reason to over ride these.
                                          // TODO: We probably need to test variations as well.
                                          "Accept", "Accept-Encoding"
                                      };
            foreach(String bad in badHeaders) 
            {
                request.removeHeader(bad);
            }
        }
        /**
       * @param request
       * @return A valid token for the given input.
       */
        private static ISecurityToken extractAndValidateToken(HttpContext context)
        {
            ISecurityToken token = new AuthInfo(context, context.Request.RawUrl).getSecurityToken();
            if (token == null)
            {
                throw new Exception("Invalid security token");
            }
            return token;
        }

        /**
       * Format a response as JSON, including additional JSON inserted by
       * chained content fetchers.
       */
        private String convertResponseToJson(ISecurityToken authToken, HttpRequestWrapper request, sResponse results)
        {
            try
            {
                String originalUrl = request.getParameter(URL_PARAM);
                String body = results.responseString;
                if ("FEED".Equals(request.getParameter(CONTENT_TYPE_PARAM)))
                {
                    body = processFeed(originalUrl, request, body);
                }
                JsonObject resp = FetchResponseUtils.getResponseAsJson(results, body);

                if (authToken != null)
                {
                    String updatedAuthToken = authToken.getUpdatedToken();
                    if (updatedAuthToken != null)
                    {
                        resp.Put("st", updatedAuthToken);
                    }
                }
                // Use raw param as key as URL may have to be decoded
                return new JsonObject().Put(originalUrl, resp).ToString();
            }
            catch (JsonException)
            {
                return "";
            }
        }

        private String processFeed(String url, HttpRequestWrapper req, String xml)
        {
            bool getSummaries = Boolean.Parse(GetParameter(req, GET_SUMMARIES_PARAM, "false"));
            int numEntries = int.Parse(GetParameter(req, NUM_ENTRIES_PARAM, DEFAULT_NUM_ENTRIES));
            return new FeedProcessor().process(url, xml, getSummaries, numEntries).ToString();
        }
    }
}