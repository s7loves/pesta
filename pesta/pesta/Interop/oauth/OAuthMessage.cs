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
using System.Text.RegularExpressions;
using System.Text;
using Pesta.Interop.oauth.signature;
using Pesta.Utilities;

namespace Pesta.Interop.oauth
{
    /// <summary>
    /// Summary description for OAuthMessage
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class OAuthMessage
    {
        public OAuthMessage(String method, String URL, ICollection<OAuth.Parameter> parameters)
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

        private bool parametersAreComplete;

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

        public void addParameters(List<OAuth.Parameter> _parameters)
        {
            parameters.AddRange(_parameters);
            parameterMap = null;
        }

        public String getParameter(String name)
        {
            return getParameterMap().ContainsKey(name) ? getParameterMap()[name]: null;
        }

        public String getConsumerKey()
        {
            return getParameter(OAuth.OAUTH_CONSUMER_KEY);
        }

        public String getToken()
        {
            return getParameter(OAuth.OAUTH_TOKEN);
        }

        public String getSignatureMethod()
        {
            return getParameter(OAuth.OAUTH_SIGNATURE_METHOD);
        }

        public String getSignature()
        {
            return getParameter(OAuth.OAUTH_SIGNATURE);
        }

        protected Dictionary<string, string> getParameterMap()
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

        /** Construct a verbose description of this message and its origins. */
        public Dictionary<string, string> getDump()
        {
            Dictionary<string, string> into = new Dictionary<string, string>();
            dump(into);
            return into;
        }

        protected void dump(Dictionary<string, string> into)
        {
            into.Add("URL", URL);
            foreach (var item in getParameterMap())
            {
                into.Add(item.Key, item.Value);
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
            foreach (String required in names)
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
            if (!pMap.ContainsKey(OAuth.OAUTH_TOKEN) && accessor.accessToken != null)
            {
                addParameter(OAuth.OAUTH_TOKEN, accessor.accessToken);
            }
            OAuthConsumer consumer = accessor.consumer;
            if (!pMap.ContainsKey(OAuth.OAUTH_CONSUMER_KEY))
            {
                addParameter(OAuth.OAUTH_CONSUMER_KEY, consumer.consumerKey);
            }
            string signatureMethod;
            if (!pMap.TryGetValue(OAuth.OAUTH_SIGNATURE_METHOD, out signatureMethod))
            {
                signatureMethod = (string)consumer.getProperty(OAuth.OAUTH_SIGNATURE_METHOD) ?? OAuth.HMAC_SHA1;
                addParameter(OAuth.OAUTH_SIGNATURE_METHOD, signatureMethod);
            }
            if (!pMap.ContainsKey(OAuth.OAUTH_TIMESTAMP))
            {
                addParameter(OAuth.OAUTH_TIMESTAMP, UnixTime.ConvertToUnixTimestamp(DateTime.UtcNow).ToString());
            }
            if (!pMap.ContainsKey(OAuth.OAUTH_NONCE))
            {
                addParameter(OAuth.OAUTH_NONCE, Crypto.getRandomString(OAuth.OAUTH_NONCE_LENGTH));
            }
            
            sign(accessor);
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
            into.Append(" realm=\"").Append(Rfc3986.Encode(realm)).Append('"');
            beforeGetParameter();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    String name = ToString(parameter.Key);
                    if (name.StartsWith("oauth_"))
                    {
                        into.Append(", ");
                        into.Append(Rfc3986.Encode(name)).Append("=\"")
                            .Append(
                            Rfc3986.Encode(ToString(parameter
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
                        foreach (String nvp in Regex.Split(m.Groups[2].Value, "\\s*,\\s*"))
                        {
                            m = NVP.Match(nvp);
                            if (m.Success)
                            {
                                String name = Rfc3986.Decode(m.Groups[1].Value);
                                String value = Rfc3986.Decode(m.Groups[2].Value);
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

        private static String ToString(Object from)
        {
            return (from == null) ? null : from.ToString();
        }
    }
}