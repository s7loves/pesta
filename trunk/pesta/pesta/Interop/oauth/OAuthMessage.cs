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
using System.Collections;
using System.Text.RegularExpressions;
using System.Text;

/// <summary>
/// Summary description for OAuthMessage
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public class OAuthMessage
{
    public OAuthMessage(String method, String URL, List<OAuth.Parameter> parameters) 
    {
        this.method = method;
        this.URL = URL;
        if (parameters == null) 
        {
            this.parameters = new List<OAuth.Parameter>();
        } 
        else 
        {
            this.parameters = new List<OAuth.Parameter>(parameters.Count);
            foreach (var entry in parameters)
            {
                this.parameters.Add(new OAuth.Parameter(ToString(entry.Key), ToString(entry.Value)));
            }
        }
    }

    public readonly String method;

    public readonly String URL;

    private readonly List<OAuth.Parameter> parameters;

    private Dictionary<string, string> parameterMap;

    private bool parametersAreComplete = false;

    public override String ToString() 
    {
        return "OAuthMessage(" + method + ", " + URL + ", " + parameters + ")";
    }

    /** A caller is about to get a parameter. */
    private void beforeGetParameter()
    {
        if (!parametersAreComplete) 
        {
            completeParameters();
            parametersAreComplete = true;
        }
    }

    /**
     * Finish adding parameters; for example read an HTTP response body and
     * parse parameters from it.
     */
    protected void completeParameters()
    {
    }

    public List<OAuth.Parameter> getParameters()
    {
        beforeGetParameter();
        return parameters;
    }

    public void addParameter(String key, String value) 
    {
        addParameter(new OAuth.Parameter(key, value));
    }

    public void addParameter(OAuth.Parameter parameter) 
    {
        parameters.Add(parameter);
        parameterMap = null;
    }

    public void addParameters(List<OAuth.Parameter> parameters) 
    {
        this.parameters.AddRange(parameters);
        parameterMap = null;
    }

    public String getParameter(String name)
    {
        return getParameterMap()[name];
    }

    public String getConsumerKey()
    {
        return getParameter("oauth_consumer_key");
    }

    public String getToken()
    {
        return getParameter("oauth_token");
    }

    public String getSignatureMethod()
    {
        return getParameter("oauth_signature_method");
    }

    public String getSignature()
    {
        return getParameter("oauth_signature");
    }

    protected Dictionary<string,string> getParameterMap()
    {
        beforeGetParameter();
        if (parameterMap == null) 
        {
            parameterMap = OAuth.newMap(parameters);
        }
        return parameterMap;
    }

    /** Get the body of the HTTP request or response. */
    public String getBodyAsString()
    {
        return null; // stub
    }

    /**
     * Get a stream from which to read the body of the HTTP request or response.
     * This is designed to support efficient streaming of a large response. If
     * you call this method before calling getBodyAsString, then subsequent
     * calls to either method will propagate an exception.
     */
    public java.io.ByteArrayInputStream getBodyAsStream()
    {
        return new java.io.ByteArrayInputStream(Encoding.GetEncoding("ISO-8859-1").GetBytes(getBodyAsString()));
    }

    /** Construct a verbose description of this message and its origins. */
    public Dictionary<string, string> getDump()
    {
        Dictionary<string, string> into = new Dictionary<string, string>();
        dump(into);
        return into;
    }

    protected void dump(Dictionary<string,string> into)
    {
        into.Add("URL", URL);
        try 
        {
            foreach (var item in getParameterMap())
            {
                into.Add(item.Key,item.Value);
            }
        } 
        catch 
        {
        }
    }

    /**
     * Verify that the required parameter names are contained in the actual
     * collection.
     * 
     * @throws OAuthProblemException
     *                 one or more parameters are absent.
     * @throws IOException
     */
    public void requireParameters(String[] names)
    {
        ICollection<string> present = getParameterMap().Keys;
        List<string> absent = new List<string>();
        foreach(String required in names) 
        {
            if (!present.Contains(required)) 
            {
                absent.Add(required);
            }
        }
        if (absent.Count != 0) 
        {
            OAuthProblemException problem = new OAuthProblemException(
                    "parameter_absent");
            problem.setParameter("oauth_parameters_absent", OAuth.percentEncode(absent));
            throw problem;
        }
    }

    /**
     * Add some of the parameters needed to request access to a protected
     * resource, if they aren't already in the message.
     * 
     * @throws IOException
     * @throws URISyntaxException
     */
    public void addRequiredParameters(OAuthAccessor accessor)
    {
        Dictionary<string, string> pMap = OAuth.newMap(parameters);
        if (!pMap.ContainsKey("oauth_token") && accessor.accessToken != null) 
        {
            addParameter("oauth_token", accessor.accessToken);
        }
        OAuthConsumer consumer = accessor.consumer;
        if (!pMap.ContainsKey("oauth_consumer_key")) 
        {
            addParameter("oauth_consumer_key", consumer.consumerKey);
        }
        string signatureMethod = null;
        if (!pMap.TryGetValue("oauth_signature_method", out signatureMethod)) 
        {
            signatureMethod = (string)consumer.getProperty("oauth_signature_method");
            if (signatureMethod == null) 
            {
                signatureMethod = "HMAC-SHA1";
            }
            addParameter("oauth_signature_method", signatureMethod);
        }
        if (!pMap.ContainsKey("oauth_timestamp")) 
        {
            addParameter("oauth_timestamp", DateTime.UtcNow.Ticks / 1000
                    + "");
        }
        if (!pMap.ContainsKey("oauth_nonce")) 
        {
            addParameter("oauth_nonce", DateTime.UtcNow.Ticks + "");
        }
        this.sign(accessor);
    }

    /**
     * Add a signature to the message.
     * 
     * @throws URISyntaxException
     */
    public void sign(OAuthAccessor accessor)
    {
        OAuthSignatureMethod.newSigner(this, accessor).sign(this);
    }

    /**
     * Check that the message is valid.
     * 
     * @throws IOException
     * @throws URISyntaxException
     * 
     * @throws OAuthProblemException
     *                 the message is invalid
     */
    public void validateMessage(OAuthAccessor accessor, OAuthValidator validator)
    {
        validator.validateMessage(this, accessor);
    }

    /**
     * Check that the message has a valid signature.
     * 
     * @throws IOException
     * @throws URISyntaxException
     * 
     * @throws OAuthProblemException
     *                 the signature is invalid
     * @deprecated use {@link OAuthMessage#validateMessage} instead.
     */
    public void validateSignature(OAuthAccessor accessor)
    {
        OAuthSignatureMethod.newSigner(this, accessor).validate(this);
    }

    /**
     * Construct a WWW-Authenticate or Authentication header value, containing
     * the given realm plus all the parameters whose names begin with "oauth_".
     */
    public String getAuthorizationHeader(String realm)
    {
        StringBuilder into = new StringBuilder(AUTH_SCHEME);
        into.Append(" realm=\"").Append(OAuth.percentEncode(realm)).Append('"');
        beforeGetParameter();
        if (parameters != null) 
        {
            foreach (var parameter in parameters)
            {
                String name = ToString(parameter.Key);
                if (name.StartsWith("oauth_")) 
                {
                    into.Append(", ");
                    into.Append(OAuth.percentEncode(name)).Append("=\"")
                            .Append(
                                    OAuth.percentEncode(ToString(parameter
                                            .Value))).Append('"');
                }
            }
        }
        return into.ToString();
    }

    /**
     * Parse the parameters from an OAuth Authorization or WWW-Authenticate
     * header. The realm is included as a parameter. If the given header doesn't
     * start with "OAuth ", return an empty list.
     */
    public static List<OAuth.Parameter> decodeAuthorization(String authorization) 
    {
        List<OAuth.Parameter> into = new List<OAuth.Parameter>();
        if (authorization != null) 
        {
            Match m = AUTHORIZATION.Match(authorization);
            if (m.Success) 
            {
                if (m.Groups[1].Value.ToLower().Equals(AUTH_SCHEME)) 
                {
                    foreach(String nvp in Regex.Split(m.Groups[2].Value,"\\s*,\\s*")) 
                    {
                        m = NVP.Match(nvp);
                        if (m.Success) 
                        {
                            String name = OAuth.decodePercent(m.Groups[1].Value);
                            String value = OAuth.decodePercent(m.Groups[2].Value);
                            into.Add(new OAuth.Parameter(name, value));
                        }
                    }
                }
            }
        }
        return into;
    }

    public static readonly String AUTH_SCHEME = "OAuth";

    static readonly Regex AUTHORIZATION = new Regex("\\s*(\\w*)\\s+(.*)", RegexOptions.Compiled);

    static readonly Regex NVP = new Regex("(\\S*)\\s*\\=\\s*\"([^\"]*)\"", RegexOptions.Compiled);

    protected static readonly java.util.List NO_PARAMETERS = new java.util.ArrayList();

    private static String ToString(Object from) 
    {
        return (from == null) ? null : from.ToString();
    }
}
