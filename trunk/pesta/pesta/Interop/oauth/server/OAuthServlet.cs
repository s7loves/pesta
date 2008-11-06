/*
 * Copyright 2007 Netflix, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Web;
using System.Collections.Generic;
/// <summary>
/// Summary description for OAuthServlet
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public class OAuthServlet
{
    /**
     * Extract the parts of the given request that are relevant to OAuth.
     * Parameters include OAuth Authorization headers and the usual request
     * parameters in the query string and/or form encoded body. The header
     * parameters come first, followed by the rest in the order they came from
     * request.getParameterMap().
     * 
     * @param URL
     *            the official URL of this service; that is the URL a legitimate
     *            client would use to compute the digital signature. If this
     *            parameter is null, this method will try to reconstruct the URL
     *            from the HTTP request; which may be wrong in some cases.
     */
    public static OAuthMessage getMessage(HttpRequest request, String URL)
    {
        if (URL == null)
        {
            URL = request.Url.ToString();
        }
        int q = URL.IndexOf('?');
        if (q >= 0)
        {
            URL = URL.Substring(0, q);
            // The query string parameters will be included in
            // the result from getParameters(request).
        }
        return new OAuthMessage(request.HttpMethod, URL,
                getParameters(request));
    }

    public static List<OAuth.Parameter> getParameters(HttpRequest request) 
    {
        List<OAuth.Parameter> list = new List<OAuth.Parameter>();
        foreach (String header in request.Headers.GetValues("Authorization"))
        {
            foreach(OAuth.Parameter parameter in OAuthMessage.decodeAuthorization(header))
            {
                if (!parameter.Key.ToLower().Equals("realm"))
                {
                    list.Add(parameter);
                }
            }
        }
        for (int i = 0; i < request.Params.Count; i++)
        {
            String name = request.Params.GetKey(i).ToString();
            foreach(String value in request.Params.GetValues(i)) 
            {
                list.Add(new OAuth.Parameter(name, value));
            }
        }
        return list;
    }
}
