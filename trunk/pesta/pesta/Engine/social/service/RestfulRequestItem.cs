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
using System.IO;
using System.Collections.Generic;
using System.Web;
using Pesta.Engine.auth;

namespace Pesta.Engine.social.service
{
    /// <summary>
    /// Summary description for RestfulRequestItem
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class RestfulRequestItem : RequestItem
    {
        protected static String X_HTTP_METHOD_OVERRIDE = "X-HTTP-Method-Override";
        protected static String SOCIAL_RESTFUL_PATH = HttpRuntime.AppDomainAppVirtualPath.Equals("/") ? "/social/rest/" : HttpRuntime.AppDomainAppVirtualPath + "/social/rest/";
        private String url;

        private Dictionary<String, List<String>> parameters;

        private String postData;

        public RestfulRequestItem(String service, String method, ISecurityToken token, BeanConverter converter)
            : base(service, method, token, converter)
        {

        }

        public RestfulRequestItem(String path, String method, String postData, ISecurityToken token, BeanConverter converter)
            : base(getServiceFromPath(path), method, token, converter)
        {
            this.postData = postData;
            this.url = path;
            putUrlParamsIntoParameters();
        }

        public RestfulRequestItem(HttpRequest request, ISecurityToken token, BeanConverter converter)
            : base(getServiceFromPath(request.RawUrl), getMethod(request), token, converter)
        {
            this.url = request.RawUrl.Replace(SOCIAL_RESTFUL_PATH, "/");
            this.parameters = createParameterMap(request);

            try
            {
                using (Stream istream = request.InputStream)
                {
                    MemoryStream memoryStream = new MemoryStream(0x10000);
                    byte[] buffer = new byte[0x1000];
                    int bytes;
                    while ((bytes = istream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytes);
                    }
                    postData = request.ContentEncoding.GetString(memoryStream.ToArray());
                }
            }
            catch (IOException e)
            {
                throw new Exception("Could not get the post data from the request", e);
            }
        }

        static String getServiceFromPath(String pathInfo)
        {
            pathInfo = pathInfo.Replace(SOCIAL_RESTFUL_PATH, "");
            int indexOfNextPathSeparator = pathInfo.IndexOf('/');
            if (indexOfNextPathSeparator != -1)
            {
                return pathInfo.Substring(0, indexOfNextPathSeparator);
            }
            return pathInfo;
        }

        static String getMethod(HttpRequest request)
        {
            String overrided = request.Params[X_HTTP_METHOD_OVERRIDE];
            if (!string.IsNullOrEmpty(overrided))
            {
                return overrided;
            }
            else
            {
                return request.HttpMethod;
            }
        }

        private static Dictionary<String, List<string>> createParameterMap(HttpRequest servletRequest)
        {
            Dictionary<String, List<string>> parameters = new Dictionary<string, List<string>>();
            for (int i = 0; i < servletRequest.Params.Count; i++)
            {
                parameters.Add(servletRequest.Params.GetKey(i), new List<string>(servletRequest.Params.GetValues(i)));
            }
            return parameters;
        }

        /*
        * Takes any url params out of the url and puts them into the param map.
        * Usually the servlet request code does this for us but the batch request calls have to do it
        * by hand.
        */
        void putUrlParamsIntoParameters()
        {
            if (this.parameters == null)
            {
                this.parameters = new Dictionary<string, List<string>>();
            }

            String fullUrl = this.url;
            int queryParamIndex = fullUrl.IndexOf('?');

            if (queryParamIndex != -1)
            {
                this.url = fullUrl.Substring(0, queryParamIndex);

                String queryParams = fullUrl.Substring(queryParamIndex + 1);
                foreach (String param in queryParams.Split('&'))
                {
                    String[] paramPieces = param.Split('=');
                    List<string> paramList = this.parameters[paramPieces[0]];
                    if (paramList == null)
                    {
                        paramList = new List<string>();
                        this.parameters.Add(paramPieces[0], paramList);
                    }
                    if (paramPieces.Length == 2)
                    {
                        paramList.Add(paramPieces[1]);
                    }
                    else
                    {
                        paramList.Add("");
                    }
                }
            }
        }

        /**
        * This could definitely be cleaner.. TODO: Come up with a cleaner way to handle all of this
        * code.
        *
        * @param urlTemplate The template the url follows
        */
        public override void applyUrlTemplate(String urlTemplate)
        {
            this.putUrlParamsIntoParameters();
            String[] actualUrl = this.url.Split('/');
            String[] expectedUrl = urlTemplate.Split('/');

            for (int i = 0; i < actualUrl.Length; i++)
            {
                String actualPart = actualUrl[i];
                String expectedPart = expectedUrl[i];

                if (expectedPart.StartsWith("{"))
                {
                    if (expectedPart.EndsWith("}+"))
                    {
                        // The param can be a repeated field. Use ',' as default separator
                        this.parameters.Add(expectedPart.Substring(1, expectedPart.Length - 2),
                                            new List<string>(actualPart.Split(',')));
                    }
                    else
                    {
                        if (actualPart.IndexOf(',') != -1)
                        {
                            throw new ArgumentException("Cannot expect plural value " + actualPart
                                                        + " for singular field " + expectedPart + " in " + this.url);
                        }
                        this.parameters.Add(expectedPart.Substring(1, expectedPart.Length - 2),
                                            new List<string>(new string[] { actualPart }));
                    }
                }
            }
        }


        public override object getTypedParameter(String parameterName, Type dataTypeClass)
        {
            // We assume the the only typed parameter in a restful request is the post-content
            // and so we simply ignore the parameter name
            return getTypedParameters(dataTypeClass);
        }

        public override object getTypedParameters(Type dataTypeClass)
        {
            return converter.convertToObject(postData, dataTypeClass);
        }



        Dictionary<String, List<String>> getParameters()
        {
            return parameters;
        }

        void setParameter(String paramName, String paramValue)
        {
            // Ignore nulls
            if (paramValue == null)
            {
                return;
            }
            this.parameters.Add(paramName, new List<String>() { paramValue });
        }

        void setListParameter(String paramName, List<String> paramValue)
        {
            this.parameters.Add(paramName, paramValue);
        }

        /**
        * Return a single param value
        */
        public override String getParameter(String paramName)
        {
            List<String> paramValue = null;
            if (this.parameters.TryGetValue(paramName, out paramValue) && paramValue.Count != 0)
            {
                return paramValue[0];
            }
            return null;
        }

        public override String getParameter(String paramName, String defaultValue)
        {
            String result = getParameter(paramName);
            if (result == null)
            {
                return defaultValue;
            }
            return result;
        }

        /**
        * Return a list param value
        */
        public override List<String> getListParameter(String paramName)
        {
            List<String> stringList = null;
            if (!this.parameters.TryGetValue(paramName, out stringList))
            {
                return new List<string>();
            }
            if (stringList.Count == 1 && stringList[0].IndexOf(',') != -1)
            {
                stringList = new List<string>(stringList[0].Split(','));
                this.parameters.Add(paramName, stringList);
            }
            return stringList;
        }
    }
}