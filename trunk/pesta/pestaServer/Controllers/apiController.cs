using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Pesta.Engine.auth;
using Pesta.Engine.social;
using Pesta.Engine.social.core.util;
using Pesta.Engine.social.service;
using Pesta.Engine.social.spi;

namespace pestaServer.Controllers
{
    public abstract class apiController : Controller
    {
        protected static String DEFAULT_ENCODING = "UTF-8";
        private HandlerDispatcher dispatcher;
        protected BeanJsonConverter jsonConverter;
        protected BeanConverter xmlConverter;
        protected BeanConverter atomConverter;

        /// <summary>
        /// Initializes a new instance of the ApiServlet class.
        public apiController()
        {
            jsonConverter = new BeanJsonConverter();
            xmlConverter = null;
            atomConverter = null;
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
            ResponseItem response = null;
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
