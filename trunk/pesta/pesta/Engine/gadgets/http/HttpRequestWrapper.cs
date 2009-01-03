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
using System.Collections.Specialized;

namespace Pesta.Engine.gadgets.http
{
    /// <summary>
    /// Summary description for RequestWrapper
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class HttpRequestWrapper
    {
        protected HttpRequest request;
        protected HttpContext context;
        public bool isConcat = false;

        public HttpRequestWrapper(HttpContext context)
        {
            this.request = context.Request;
            this.context = context;
        }
        public HttpRequestWrapper(HttpContext context, bool isConcat)
        {
            this.request = context.Request;
            this.context = context;
            this.isConcat = isConcat;
        }

        public string getHeaders(string name)
        {
            return request.Headers[name];
        }

        public NameValueCollection getHeaders()
        {
            return request.Headers;
        }

        public HttpRequest getRequest()
        {
            return request;
        }
        public HttpContext getContext()
        {
            return context;
        }

        public virtual String getParameter(String paramName)
        {
            string[] entry = request.Params.GetValues(paramName);
            if (entry == null)
            {
                return null;
            }
            return entry[0];
        }
    }
}