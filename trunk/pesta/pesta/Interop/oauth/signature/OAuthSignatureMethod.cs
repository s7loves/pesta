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

namespace Pesta.Interop.oauth.signature
{
    /// <summary>
    /// Summary description for OAuthSignatureMethod
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public abstract class OAuthSignatureMethod
    {
        /** Add a signature to the message. 
         * @throws URISyntaxException 
         * @throws IOException */
        public void sign(OAuthMessage message)
        {
            message.addParameter(new OAuth.Parameter("oauth_signature", getSignature(message)));
        }

        /**
         * Check whether the message has a valid signature.
         * @throws URISyntaxException 
         *
         * @throws OAuthProblemException
         *             the signature is invalid
         */
        public void validate(OAuthMessage message)
        {
            message.requireParameters(new string[] { "oauth_signature" });
            String signature = message.getSignature();
            String baseString = getBaseString(message);
            if (!isValid(signature, baseString))
            {
                OAuthProblemException problem = new OAuthProblemException(
                    "signature_invalid");
                problem.setParameter("oauth_signature", signature);
                problem.setParameter("oauth_signature_base_string", baseString);
                problem.setParameter("oauth_signature_method", message
                                                                   .getSignatureMethod());
                throw problem;
            }
        }

        protected String getSignature(OAuthMessage message)
        {
            String baseString = getBaseString(message);
            String signature = getSignature(baseString);
            // Logger log = Logger.getLogger(getClass().getName());
            // if (log.isLoggable(Level.FINE)) {
            // log.fine(signature + "=getSignature(" + baseString + ")");
            // }
            return signature;
        }

        protected virtual void initialize(String name, OAuthAccessor accessor)
        {
            String secret = accessor.consumer.consumerSecret;
            if (name.EndsWith(_ACCESSOR))
            {
                // This code supports the 'Accessor Secret' extensions
                // described in http://oauth.pbwiki.com/AccessorSecret
                String key = OAuthConsumer.ACCESSOR_SECRET;
                Object accessorSecret = accessor.getProperty(key);
                if (accessorSecret == null)
                {
                    accessorSecret = accessor.consumer.getProperty(key);
                }
                if (accessorSecret != null)
                {
                    secret = accessorSecret.ToString();
                }
            }
            if (secret == null)
            {
                secret = "";
            }
            setConsumerSecret(secret);
        }

        public static readonly String _ACCESSOR = "-Accessor";

        /** Compute the signature for the given base string. */
        protected abstract String getSignature(String baseString);

        /** Decide whether the signature is valid. */
        protected abstract bool isValid(String signature, String baseString);

        private String consumerSecret;

        private String tokenSecret;

        protected String getConsumerSecret()
        {
            return consumerSecret;
        }

        public virtual void setConsumerSecret(String consumerSecret)
        {
            this.consumerSecret = consumerSecret;
        }

        public String getTokenSecret()
        {
            return tokenSecret;
        }

        public virtual void setTokenSecret(String tokenSecret)
        {
            this.tokenSecret = tokenSecret;
        }

        public static String getBaseString(OAuthMessage message)
        {
            List<OAuth.Parameter> parameters;
            String url = message.URL;
            int q = url.IndexOf('?');
            if (q < 0)
            {
                parameters = message.getParameters();
            }
            else
            {
                // Combine the URL query string with the other parameters:
                parameters = new List<OAuth.Parameter>();
                parameters.AddRange(OAuth.decodeForm(message.URL.Substring(q + 1)));
                parameters.AddRange(message.getParameters());
                url = url.Substring(0, q);
            }
            return OAuth.percentEncode(message.method.ToUpper()) + '&'
                   + OAuth.percentEncode(normalizeUrl(url)) + '&'
                   + OAuth.percentEncode(normalizeParameters(parameters));
        }

        protected static String normalizeUrl(String url)
        {
            Uri uri = new Uri(url);
            String authority = uri.Authority.ToLower();
            String scheme = uri.Scheme.ToLower();

            bool dropPort = (scheme.Equals("http") && uri.Port == 80)
                            || (scheme.Equals("https") && uri.Port == 443);

            if (dropPort)
            {
                // find the last : in the authority
                int index = authority.LastIndexOf(":");
                if (index >= 0)
                {
                    authority = authority.Substring(0, index);
                }
            }

            // we know that there is no query and no fragment here.
            return scheme + "://" + authority + uri.AbsolutePath;
        }

        protected static String normalizeParameters(List<OAuth.Parameter> parameters)
        {
            if (parameters == null)
            {
                return "";
            }
            List<ComparableParameter> p = new List<ComparableParameter>(parameters.Count);
            foreach (OAuth.Parameter parameter in parameters)
            {
                if (!"oauth_signature".Equals(parameter.Key))
                {
                    p.Add(new ComparableParameter(parameter));
                }
            }
            p.Sort();
            return OAuth.formEncode(getParameters(p));
        }

        public static byte[] decodeBase64(String s)
        {
            char[] value = s.ToCharArray();
            return Convert.FromBase64CharArray(value, 0, value.Length);
        }

        public static String base64Encode(byte[] b)
        {
            return Convert.ToBase64String(b);
        }

        public static OAuthSignatureMethod newSigner(OAuthMessage message,
                                                     OAuthAccessor accessor)
        {
            message.requireParameters(new string[] { OAuth.OAUTH_SIGNATURE_METHOD });
            OAuthSignatureMethod signer = newMethod(message.getSignatureMethod(),
                                                    accessor);
            signer.setTokenSecret(accessor.tokenSecret);
            return signer;
        }

        /** The factory for signature methods. */
        public static OAuthSignatureMethod newMethod(String name,
                                                     OAuthAccessor accessor)
        {
            try
            {
                Type methodClass = NAME_TO_CLASS[name];
                if (methodClass != null)
                {
                    OAuthSignatureMethod method = (OAuthSignatureMethod)Activator.CreateInstance(methodClass);
                    method.initialize(name, accessor);
                    return method;
                }
                OAuthProblemException problem = new OAuthProblemException("signature_method_rejected");
                List<string> todo = new List<string>();
                foreach (var item in NAME_TO_CLASS.Keys)
                {
                    todo.Add(item);
                }
                String acceptable = OAuth.percentEncode(todo);
                if (acceptable.Length > 0)
                {
                    problem.setParameter("oauth_acceptable_signature_methods",
                                         acceptable.ToString());
                }
                throw problem;
            }
            catch (Exception e)
            {
                throw new OAuthException(e);
            }
        }

        /** Retrieve the original parameters from a sorted collection. */
        private static List<OAuth.Parameter> getParameters(List<ComparableParameter> parameters)
        {
            if (parameters == null)
            {
                return null;
            }
            List<OAuth.Parameter> list = new List<OAuth.Parameter>(parameters.Count);
            foreach (var parameter in parameters)
            {
                list.Add(parameter.value);
            }
            return list;
        }

        /**
         * Subsequently, newMethod(name) will attempt to instantiate the given
         * class, with no constructor parameters.
         */
        private static readonly Dictionary<string, Type> NAME_TO_CLASS = new Dictionary<string, Type>()
                                                                             {
                                                                                 {"HMAC-SHA1", typeof( HMAC_SHA1)},
                                                                                 {"PLAINTEXT", typeof(PLAINTEXT)},
                                                                                 {"RSA-SHA1", typeof(RSA_SHA1)},
                                                                                 {"HMAC-SHA1" + _ACCESSOR, typeof(HMAC_SHA1)},
                                                                                 {"PLAINTEXT" + _ACCESSOR, typeof(PLAINTEXT)}
                                                                             };

        /** An efficiently sortable wrapper around a parameter. */
        public class ComparableParameter : IComparable
        {
            public ComparableParameter(OAuth.Parameter value)
            {
                this.value = value;
                String n = ToString(value.Key);
                String v = ToString(value.Value);
                this.key = OAuth.percentEncode(n) + ' ' + OAuth.percentEncode(v);
                // ' ' is used because it comes before any character
                // that can appear in a percentEncoded string.
            }

            public OAuth.Parameter value;

            public readonly String key;

            public static String ToString(Object from)
            {
                return (from == null) ? null : from.ToString();
            }

            public int CompareTo(Object that)
            {
                return this.key.CompareTo(((ComparableParameter)that).key);
            }

            public override String ToString()
            {
                return key;
            }

        }


    }
}