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
using System.IO;
using System.Text;

namespace Pesta.Libraries.OAuth
{
    public class OAuth
    {
        public static readonly int OAUTH_NONCE_LENGTH = 16;
        public static readonly String VERSION_1_0 = "1.0";

        /** The encoding used to represent characters as bytes. */
        public static readonly String ENCODING = "UTF-8";

        /** The MIME type for a sequence of OAuth parameters. */
        public static readonly String FORM_ENCODED = "application/x-www-form-urlencoded";

        public static readonly String OAUTH_CONSUMER_KEY = "oauth_consumer_key";
        public static readonly String OAUTH_TOKEN = "oauth_token";
        public static readonly String OAUTH_TOKEN_SECRET = "oauth_token_secret";
        public static readonly String OAUTH_SIGNATURE_METHOD = "oauth_signature_method";
        public static readonly String OAUTH_SIGNATURE = "oauth_signature";
        public static readonly String OAUTH_TIMESTAMP = "oauth_timestamp";
        public static readonly String OAUTH_NONCE = "oauth_nonce";
        public static readonly String OAUTH_VERSION = "oauth_version";

        public static readonly String HMAC_SHA1 = "HMAC-SHA1";
        public static readonly String RSA_SHA1 = "RSA-SHA1";

        public static class Problems
        {
            public static readonly String TOKEN_NOT_AUTHORIZED = "token_not_authorized";
            public static readonly String INVALID_USED_NONCE = "invalid_used_nonce";
            public static readonly String SIGNATURE_INVALID = "signature_invalid";
            public static readonly String INVALID_EXPIRED_TOKEN = "invalid_expired_token";
            public static readonly String INVALID_CONSUMER_KEY = "invalid_consumer_key";
            public static readonly String CONSUMER_KEY_REFUSED = "consumer_key_refused";
            public static readonly String TIMESTAMP_REFUSED = "timestamp_refused";
            public static readonly String PARAMETER_REJECTED = "parameter_rejected";
            public static readonly String PARAMETER_ABSENT = "parameter_absent";
            public static readonly String VERSION_REJECTED = "version_rejected";
            public static readonly String SIGNATURE_METHOD_REJECTED = "signature_method_rejected";

            public static readonly String OAUTH_PARAMETERS_ABSENT = "oauth_parameters_absent";
            public static readonly String OAUTH_PARAMETERS_REJECTED = "oauth_parameters_rejected";
            public static readonly String OAUTH_ACCEPTABLE_TIMESTAMPS = "oauth_acceptable_timestamps";
            public static readonly String OAUTH_ACCEPTABLE_VERSIONS = "oauth_acceptable_versions";
        }

        /** Return true if the given Content-Type header means FORM_ENCODED. */
        public static bool isFormEncoded(String contentType)
        {
            if (contentType == null)
            {
                return false;
            }
            int semi = contentType.IndexOf(";");
            if (semi >= 0)
            {
                contentType = contentType.Substring(0, semi);
            }
            return contentType.Trim().ToLower().Equals(FORM_ENCODED);
        }

        /**
         * Construct a form-urlencoded document containing the given sequence of
         * name/value pairs. Use OAuth percent encoding (not exactly the encoding
         * mandated by HTTP).
         */
        public static String formEncode(List<Parameter> parameters)
        {
            MemoryStream b = new MemoryStream();
            formEncode(parameters, b);
            return Encoding.Default.GetString(b.ToArray());
        }

        /**
         * Write a form-urlencoded document into the given stream, containing the
         * given sequence of name/value pairs.
         */
        public static void formEncode(List<Parameter> parameters, Stream into)
        {
            if (parameters != null)
            {
                bool first = true;
                byte[] towrite;
                foreach (Parameter parameter in parameters)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        towrite = Encoding.UTF8.GetBytes("&");
                        into.Write(towrite, 0, towrite.Length);
                    }
                    towrite = Encoding.UTF8.GetBytes(Rfc3986.Encode(ToString(parameter.Key)) +
                                                     "=" + Rfc3986.Encode(ToString(parameter.Value)));
                    into.Write(towrite, 0, towrite.Length);
                }
            }
        }

        /** Parse a form-urlencoded document. */
        public static List<Parameter> decodeForm(String form)
        {
            List<Parameter> list = new List<Parameter>();
            if (!isEmpty(form))
            {
                foreach (String nvp in form.Split(new[]{'&'},StringSplitOptions.RemoveEmptyEntries))
                {
                    int equals = nvp.IndexOf('=');
                    String name;
                    String value;
                    if (equals < 0)
                    {
                        name = Rfc3986.Decode(nvp);
                        value = null;
                    }
                    else
                    {
                        name = Rfc3986.Decode(nvp.Substring(0, equals));
                        value = Rfc3986.Decode(nvp.Substring(equals + 1));
                    }
                    list.Add(new Parameter(name, value));
                }
            }
            return list;
        }

        /** Construct a &-separated list of the given values, percentEncoded. */
        public static String percentEncode(List<string> values)
        {
            StringBuilder p = new StringBuilder();
            foreach (string v in values)
            {
                if (p.Length > 0)
                {
                    p.Append("&");
                }
                p.Append(Rfc3986.Encode(ToString(v)));
            }
            return p.ToString();
        }

        /**
         * Construct a Map containing a copy of the given parameters. If several
         * parameters have the same name, the Map will contain the first value,
         * only.
         */
        public static Dictionary<String, String> newMap(IEnumerable<Parameter> from)
        {
            Dictionary<String, String> map = new Dictionary<string, string>();
            if (from != null)
            {
                foreach (var f in from)
                {
                    String key = ToString(f.Key);
                    if (!map.ContainsKey(key))
                    {
                        map.Add(key, ToString(f.Value));
                    }
                }
            }
            return map;
        }

        /** Construct a list of Parameters from name, value, name, value... */
        public static List<Parameter> newList(String[] parameters)
        {
            List<Parameter> list = new List<Parameter>(parameters.Length / 2);
            for (int p = 0; p + 1 < parameters.Length; p += 2)
            {
                list.Add(new Parameter(parameters[p], parameters[p + 1]));
            }
            return list;
        }

        /** A name/value pair. */
        public struct Parameter
        {
            public Parameter(String key, String value)
            {
                Key = key;
                Value = value;
            }

            public readonly String Key;

            public String Value;

            public override String ToString()
            {
                return Rfc3986.Encode(Key) + '=' + Rfc3986.Encode(Value);
            }
        }

        private static String ToString(Object from)
        {
            return (from == null) ? null : from.ToString();
        }

        /**
         * Construct a URL like the given one, but with the given parameters added
         * to its query string.
         */
        public static String addParameters(String url, String[] parameters)
        {
            return addParameters(url, newList(parameters));
        }

        public static String addParameters(String url,
                                           List<Parameter> parameters)
        {
            String form = formEncode(parameters);
            if (string.IsNullOrEmpty(form))
            {
                return url;
            }
            return url + ((url.IndexOf("?") < 0) ? '?' : '&') + form;
        }

        public static bool isEmpty(String str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}