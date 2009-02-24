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
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Pesta.Engine.auth;
using Pesta.Engine.social;
using Pesta.Engine.social.spi;
using pestaServer.Models.social.core.util;
using pestaServer.Models.social.service;

namespace pestaServer.Controllers
{
    public abstract class apiController : Controller
    {
        protected static String DEFAULT_ENCODING = "UTF-8";
        private readonly IHandlerDispatcher dispatcher;
        protected BeanJsonConverter jsonConverter;
        protected BeanConverter xmlConverter;
        protected BeanConverter atomConverter;

        /// <summary>
        /// Initializes a new instance of the ApiServlet class.
        public apiController()
        {
            jsonConverter = new BeanJsonConverter();
            xmlConverter = new BeanXmlConverter();
            atomConverter = new BeanAtomConverter();
            dispatcher = new StandardHandlerDispatcher(new PersonHandler(), new ActivityHandler(), new AppDataHandler());
        }
        protected abstract void sendError(HttpResponse response, ResponseItem responseItem);

        protected ISecurityToken getSecurityToken(HttpContext context)
        {
            return new AuthInfo(context, context.Request.RawUrl).getSecurityToken();
        }

        protected void sendSecurityError(HttpResponse response)
        {
            sendError(response, new ResponseItem(ResponseError.UNAUTHORIZED,
                                                 "The request did not have a proper security token nor oauth message and unauthenticated "
                                                 + "requests are not allowed"));
        }

        /**
       * Delivers a request item to the appropriate DataRequestHandler.
       */
        protected IAsyncResult handleRequestItem(RequestItem requestItem)
        {
            DataRequestHandler handler = dispatcher.getHandler(requestItem.getService());
            if (handler == null)
            {
                throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED,
                                             "The service " + requestItem.getService() + " is not implemented");
            }
            
            return handler.handleItem(requestItem);
        }

        protected ResponseItem getResponseItem(IAsyncResult future)
        {
            ResponseItem response;
            try
            {
                // TODO: use timeout methods?
                AsyncResult result = (AsyncResult)future;
                DataRequestHandler.handleItemDelegate del = (DataRequestHandler.handleItemDelegate)result.AsyncDelegate;
                object res = del.EndInvoke(result);
                response = new ResponseItem(res);
            }
            catch (Exception e)
            {
                response = responseItemFromException(e);
            }

            return response;
        }

        protected ResponseItem responseItemFromException(Exception t)
        {
            if (t is SocialSpiException)
            {
                SocialSpiException spe = (SocialSpiException)t;
                return new ResponseItem(spe.getError(), spe.Message);
            }
            return new ResponseItem(ResponseError.INTERNAL_ERROR, t.Message);
        }

        protected void setCharacterEncodings(HttpRequest request, HttpResponse response)
        {
            if (string.IsNullOrEmpty(request.ContentEncoding.EncodingName))
            {
                request.ContentEncoding = Encoding.GetEncoding(DEFAULT_ENCODING);
            }
            response.ContentEncoding = Encoding.GetEncoding(DEFAULT_ENCODING);
        }

    }
}
