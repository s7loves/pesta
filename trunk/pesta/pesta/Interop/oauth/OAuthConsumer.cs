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
using System.Collections.Generic;

/// <summary>
/// Summary description for OAuthConsumer
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
[Serializable]
public class OAuthConsumer
{
    private static readonly long serialVersionUID = -2258581186977818580L;

    public readonly String callbackURL;
    public readonly String consumerKey;
    public readonly String consumerSecret;
    public readonly OAuthServiceProvider serviceProvider;

    public OAuthConsumer(String callbackURL, String consumerKey,
            String consumerSecret, OAuthServiceProvider serviceProvider) 
    {
        this.callbackURL = callbackURL;
        this.consumerKey = consumerKey;
        this.consumerSecret = consumerSecret;
        this.serviceProvider = serviceProvider;
    }

    private readonly Dictionary<String, Object> properties = new Dictionary<String, Object>();

    public Object getProperty(String name) 
    {
        return properties[name];
    }

    public void setProperty(String name, Object value) 
    {
        properties.Add(name, value);
    }

    /**
     * The name of the property whose value is the <a
     * href="http://oauth.pbwiki.com/AccessorSecret">Accessor Secret</a>.
     */
    public static readonly String ACCESSOR_SECRET = "oauth_accessor_secret";
}
