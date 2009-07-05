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
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Pesta.Engine.auth;
using Pesta.Engine.protocol;
using Pesta.Engine.protocol.conversion;
using Pesta.Engine.social;
using Pesta.Engine.social.spi;
using pestaServer.Models.social.service;

namespace pestaServer.Controllers
{
    public abstract class ApiController : Controller
    {
        protected static readonly String FORMAT_PARAM = "format";
        protected static readonly String JSON_FORMAT = "json";
        protected static readonly String ATOM_FORMAT = "atom";
        protected static readonly String XML_FORMAT = "xml";
        private const String DEFAULT_ENCODING = "UTF-8";
        private readonly IHandlerDispatcher dispatcher;
        protected readonly BeanJsonConverter jsonConverter;
        protected readonly BeanConverter xmlConverter;
        protected readonly BeanConverter atomConverter;

        public ApiController()
        {
            jsonConverter = new BeanJsonConverter();
            xmlConverter = new BeanXmlConverter();
            atomConverter = new BeanAtomConverter();
            dispatcher = new StandardHandlerDispatcher(
                new PersonHandler(), 
                new ActivityHandler(), 
                new AppDataHandler(),
                new MessageHandler());
        }

        public static void checkContentTypes(IEnumerable<String> allowedContentTypes, String contentType)
        {
            ContentTypes.checkContentTypes(allowedContentTypes, contentType, true);
        }

        protected static ISecurityToken getSecurityToken(HttpContext context, string rawUrl)
        {
            return new AuthInfo(context, rawUrl).getSecurityToken();
        }

        protected abstract void sendError(HttpResponse servletResponse, ResponseItem responseItem);

        protected void sendSecurityError(HttpResponse servletResponse)
        {
            sendError(servletResponse, new ResponseItem((int)HttpStatusCode.Unauthorized,
                    "The request did not have a proper security token nor oauth message and unauthenticated "
                        + "requests are not allowed"));
        }
        protected IAsyncResult HandleRequestItem(RequestItem requestItem)
        {
            DataRequestHandler handler = dispatcher.getHandler(requestItem.getService());
            return handler == null ? null : handler.handleItem(requestItem);
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
            catch (Exception ie)
            {
                response = responseItemFromException(ie);
            }
            return response;
        }

        protected ResponseItem responseItemFromException(Exception ex)
        {
            if (ex is ProtocolException)
            {
                ProtocolException pe = (ProtocolException)ex;
                return new ResponseItem(pe.getCode(), pe.Message, pe.getResponse());
            }
            return new ResponseItem((int)HttpStatusCode.InternalServerError, ex.Message);
        }

        protected void setCharacterEncodings(HttpRequest servletRequest, HttpResponse servletResponse)
        {
            if (servletRequest.ContentEncoding == null)
            {
                servletRequest.ContentEncoding = Encoding.UTF8;
            }
            servletResponse.ContentEncoding = Encoding.UTF8;
        }
    }
}
