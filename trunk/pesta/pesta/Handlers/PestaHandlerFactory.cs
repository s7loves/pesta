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
using Pesta.Handlers;

namespace Pesta
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class PestaHandlerFactory : IHttpHandlerFactory
    {
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            IHttpHandler handlerToReturn = null;

            string fullOrigionalpath = context.Request.AppRelativeCurrentExecutionFilePath;
            
            if (fullOrigionalpath.Contains("/gadgets/js/"))
            {
                handlerToReturn = new JsServlet();
            }
            else if (fullOrigionalpath.Contains("ifr.ashx"))
            {
                handlerToReturn = new GadgetRenderingServlet();
            }
            else if (fullOrigionalpath.Contains("concat.ashx"))
            {
                handlerToReturn = new ConcatProxyServlet();
            }
            else if (fullOrigionalpath.Contains("proxy.ashx"))
            {
                handlerToReturn = new ProxyServlet();
            }
            else if (fullOrigionalpath.Contains("makeRequest.ashx"))
            {
                handlerToReturn = new MakeRequestServlet();
            }
            else if (fullOrigionalpath.Contains("oauthcallback.ashx"))
            {
                handlerToReturn = new OAuthCallbackServlet();
            }
            else if (fullOrigionalpath.Contains("metadata.ashx"))
            {
                handlerToReturn = new RpcServlet();
            }
            else if (fullOrigionalpath.Contains("/social/rest/"))
            {
                handlerToReturn = new DataServiceServlet();
            }
            else if (fullOrigionalpath.Contains("/social/rpc"))
            {
                handlerToReturn = new JsonRpcServlet();
            }
            return handlerToReturn;
        }

        public void ReleaseHandler(IHttpHandler handler)
        {
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

    }
}
