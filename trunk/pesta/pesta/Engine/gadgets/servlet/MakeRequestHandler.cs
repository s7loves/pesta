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
using org.apache.shindig.gadgets;
using Jayrock.Json;

namespace Pesta
{
    /// <summary>
    /// Summary description for MakeRequestHandler
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class MakeRequestHandler : ProxyBase
    {
        private static String UNPARSEABLE_CRUFT = "throw 1; < don't be evil' >";
        private static String POST_DATA_PARAM = "postData";
        private static String METHOD_PARAM = "httpMethod";
        private static String NOCACHE_PARAM = "nocache";
        private static String AUTHZ_PARAM = "authz";
        private static String CONTENT_TYPE_PARAM = "contentType";
        private static String NUM_ENTRIES_PARAM = "numEntries";
        private static String DEFAULT_NUM_ENTRIES = "3";
        private static String GET_SUMMARIES_PARAM = "getSummaries";

        private ContentFetcherFactory contentFetcherFactory;
        private ContentRewriterRegistry contentRewriterRegistry;
        public readonly static MakeRequestHandler Instance = new MakeRequestHandler();
        protected MakeRequestHandler()
        {
            contentRewriterRegistry = DefaultContentRewriterRegistry.Instance;
            contentFetcherFactory = ContentFetcherFactory.Instance;
        }

        public void fetch(HttpRequestWrapper request, HttpResponseWrapper response)
        {
            sRequest rcr = buildHttpRequest(request);

            // Serialize the response
            sResponse results = contentFetcherFactory.fetch(rcr);
            if (results == null)
            {
                throw new Exception("no results");
            }

            // Rewrite the response
            if (contentRewriterRegistry != null)
            {
                results = contentRewriterRegistry.rewriteHttpResponse(rcr, results);
            }

            // Serialize the response
            String output = convertResponseToJson(rcr.SecurityToken, request, results);

            // Find and set the refresh interval
            setResponseHeaders(request, response.getResponse(), results);

            response.setStatus((int)HttpStatusCode.OK);
            response.setContentType("application/json; charset=utf-8");
            response.Write(System.Text.Encoding.UTF8.GetBytes(UNPARSEABLE_CRUFT + output));
        }

        /**
        * Generate a remote content request based on the parameters
        * sent from the client.
        * @throws GadgetException
        */
        private sRequest buildHttpRequest(HttpRequestWrapper request)
        {
            String encoding = request.getRequest().ContentEncoding.EncodingName;
            if (string.IsNullOrEmpty(encoding))
            {
                encoding = "UTF-8";
            }

            org.apache.shindig.common.uri.Uri url = validateUrl(request.getParameter(URL_PARAM));

            sRequest req = new sRequest(url).setPostBody(request.getRequest().ContentEncoding.GetBytes(getParameter(request, POST_DATA_PARAM, "")));

            req.Container = getContainer(request);
            req.req.Method = getParameter(request, METHOD_PARAM, "GET");

            //req.req.Connection = request.getRequest().Headers["Connection"];
            //req.req.KeepAlive = false;
            req.req.Referer = request.getRequest().UrlReferrer.ToString();
            req.req.UserAgent = request.getRequest().UserAgent;
            req.addHeader("Accept-Charset", request.getHeaders("Accept-Charset"));
            req.addHeader("Accept-Language", request.getHeaders("Accept-Language"));

            req.SetIgnoreCache("1".Equals(request.getParameter(NOCACHE_PARAM)));

            if (request.getParameter(GADGET_PARAM) != null)
            {
                req.Gadget = org.apache.shindig.common.uri.Uri.parse(request.getParameter(GADGET_PARAM));
            }

            // Allow the rewriter to use an externally forced mime type. This is needed
            // allows proper rewriting of <script src="x"/> where x is returned with
            // a content type like text/html which unfortunately happens all too often
            req.SetRewriteMimeType(request.getParameter(REWRITE_MIME_TYPE_PARAM));

            // Figure out whether authentication is required
            AuthType auth = AuthType.Parse(getParameter(request, AUTHZ_PARAM, null));
            req.AuthType = auth;
            if (auth != AuthType.NONE)
            {
                req.SecurityToken = extractAndValidateToken(request.getContext());
                req.OAuthArguments = new OAuthArguments(auth, request.getRequest());
            }
            return req;
        }

        /**
       * @param request
       * @return A valid token for the given input.
       */
        private SecurityToken extractAndValidateToken(HttpContext context)
        {
            SecurityToken token = new AuthInfo(context, context.Request.RawUrl).getSecurityToken();
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
        private String convertResponseToJson(SecurityToken authToken, HttpRequestWrapper request, sResponse results)
        {
            try
            {
                String originalUrl = request.getParameter(ProxyBase.URL_PARAM);
                String body = results.responseString;
                if ("FEED".Equals(request.getParameter(CONTENT_TYPE_PARAM)))
                {
                    body = processFeed(originalUrl, request, body);
                }
                JsonObject resp = getResponseAsJson(results, body);

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
            catch (JsonException e)
            {
                return "";
            }
        }

        private String processFeed(String url, HttpRequestWrapper req, String xml)
        {
            bool getSummaries = Boolean.Parse(getParameter(req, GET_SUMMARIES_PARAM, "false"));
            int numEntries = int.Parse(getParameter(req, NUM_ENTRIES_PARAM, DEFAULT_NUM_ENTRIES));
            return new FeedProcessor().process(url, xml, getSummaries, numEntries).ToString();
        }

        /**
       * Convert a response to a JSON object.  static so it can be used by HttpPreloaders as well.
       * 
       * The returned JSON object contains the following values:
       * rc: integer response code
       * body: string response body
       * headers: object, keys are header names, values are lists of header values
       * 
       * @param response the response body
       * @param body string to use as the body of the response.
       * @return a JSONObject representation of the response body.
       */
        public static JsonObject getResponseAsJson(sResponse response, String body)
        {
            JsonObject resp = new JsonObject();
            resp.Put("rc", response.getHttpStatusCode());
            resp.Put("body", body);
            JsonObject headers = new JsonObject();
            addHeaders(headers, response, "set-cookie");
            addHeaders(headers, response, "location");
            resp.Put("headers", headers);
            // Merge in additional response data
            foreach (var entry in response.getMetadata())
            {
                resp.Put(entry.Key, entry.Value);
            }
            return resp;
        }

        private static void addHeaders(JsonObject headers, sResponse response, String headerName)
        {
            string[] values = response.getHeaders(headerName);
            if (values != null)
            {
                headers.Put(headerName.ToLower(), new JsonArray(values));
            }
        }
    } 
}
