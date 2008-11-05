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
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using com.ibm.icu.text;

namespace Pesta
{
    /// <summary>
    /// Represents the results of an HTTP content retrieval operation.
    /// HttpResponse objects are immutable in order to allow them to be safely used
    /// in concurrent caches and by multiple threads without worrying about
    /// concurrent modification.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class sResponse
    {
        public HttpWebResponse response;
        public const int SC_OK = 200;
        public const int SC_MOVED_TEMPORARILY = 301;
        public const int SC_MOVED_PERMANENTLY = 302;
        public const int SC_UNAUTHORIZED = 401;
        public const int SC_FORBIDDEN = 403;
        public const int SC_NOT_FOUND = 404;
        public const int SC_INTERNAL_SERVER_ERROR = 500;
        public const int SC_TIMEOUT = 504;

        // These content types can always skip encoding detection.
        private static readonly List<string> BINARY_CONTENT_TYPES = new List<string>()
                {"image/jpeg", "image/png",
					"image/gif", "image/jpg", "application/x-shockwave-flash",
					"application/octet-stream", "application/ogg",
					"application/zip", "audio/mpeg", "audio/x-ms-wma",
					"audio/vnd.rn-realaudio", "audio/x-wav", "video/mpeg",
					"video/mp4", "video/quicktime", "video/x-ms-wmv",
					"video/x-flv", "video/flv", "video/x-ms-asf",
					"application/pdf"};

        // These HTTP status codes should always honor the HTTP status returned by
        // the remote host. All
        // other status codes are treated as errors and will use the
        // negativeCacheTtl value.
        private static readonly List<int> CACHE_CONTROL_OK_STATUS_CODES =
            new List<int>() { SC_OK, SC_UNAUTHORIZED, SC_FORBIDDEN };

        // TTL to use when an error response is fetched. This should be non-zero to
        // avoid high rates of requests to bad urls in high-traffic situations.
        internal const long DEFAULT_NEGATIVE_CACHE_TTL = 30 * 1000;

        // Default TTL for an entry in the cache that does not have any cache
        // control headers.
        internal const long DEFAULT_TTL = 5L * 60L * 1000L;

        internal const String DEFAULT_ENCODING = "UTF-8";

        private static long negativeCacheTtl = DEFAULT_NEGATIVE_CACHE_TTL;

        private static long defaultTtl = DEFAULT_TTL;

        public readonly byte[] responseBytes;
        public String responseString = "";
        private readonly Dictionary<string, string> metadata = new Dictionary<string, string>();

        private readonly DateTime date = DateTime.Now;

        public sResponse(HttpWebResponse resp)
        {
            this.response = resp;
            try
            {
                MemoryStream memoryStream = new MemoryStream(0x10000);
                byte[] buffer = new byte[0x1000];
                int bytes;
                using (Stream responseStream = resp.GetResponseStream())
                {
                    while ((bytes = responseStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memoryStream.Write(buffer, 0, bytes);
                    }
                    memoryStream.Close();
                }
                responseBytes = memoryStream.ToArray();
                Encoding encoding = Encoding.GetEncoding(resp.ContentEncoding == "" ? DEFAULT_ENCODING : resp.ContentEncoding);
                responseString = encoding.GetString(responseBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                resp.Close();
            }

            // Strip BOM if present
            if (responseString.Length > 0 && Convert.ToInt32(responseString[0]) == 0xFEFF)
            {
                responseString = responseString.Substring(1);
            }

            //responseBytes = Encoding.UTF8.GetBytes(responseString);
            // update metadata
        }

        public HttpStatusCode getHttpStatusCode()
        {
            return response.StatusCode;
        }

        /**
        * @return the content length
        */
        public long getContentLength()
        {
            return response.ContentLength;
        }

        /**
        * @return All headers for this object.
        */
        public NameValueCollection getHeaders()
        {
            return response.Headers;
        }

        /**
        * @return All headers with the given name. If no headers are set for the given name, an empty
        * collection will be returned.
        */
        public string[] getHeaders(String name)
        {
            string[] retv = response.Headers.GetValues(name);
            if (retv == null)
            {
                return new string[1] { "" };
            }
            return retv;
        }

        /**
        * @return The first set header with the given name or null if not set. If you need multiple
        *         values for the header, use getHeaders().
        */
        public string getHeader(String name)
        {
            string[] headerList = getHeaders(name);
            if (headerList == null)
            {
                return null;
            }
            else
            {
                return headerList[0];
            }
        }

        /**
        * @return additional data to embed in responses sent from the JSON proxy.
        */
        public Dictionary<string, string> getMetadata()
        {
            return metadata;
        }
        private static void getAndUpdateDate(NameValueCollection headers)
        {
            // Validate the Date header. Must conform to the HTTP date format.
            long timestamp = -1;
            string[] dates = headers.GetValues("Date");
            String dateStr = dates == null ? null : dates.Length == 0 ? null : dates[0];
            if (dateStr == null)
            {
                DateTime d;
                if (!DateTime.TryParse(dateStr, out d))
                {
                    timestamp = -1;
                }
            }
            if (timestamp <= 0)
            {
                headers.Add("Date", DateTime.Now.ToString());
            }
        }
        private static String getAndUpdateEncoding(NameValueCollection headers, byte[] body)
        {
            string[] values = headers.GetValues("Content-Type");
            String contentType = values == null ? null : values.Length == 0 ? null : values[0];
            if (contentType != null)
            {
                String[] parts = contentType.Split(';');
                if (BINARY_CONTENT_TYPES.Contains(parts[0]))
                {
                    return DEFAULT_ENCODING;
                }
                if (parts.Length == 2)
                {
                    int offset = parts[1].IndexOf("charset=");
                    if (offset != -1)
                    {
                        return parts[1].Substring(offset + 8).ToUpper();
                    }
                }
            }

            if (body == null || body.Length == 0)
            {
                return DEFAULT_ENCODING;
            }

            // If the header doesn't specify the charset, try to determine it by examining the content.
            CharsetDetector detector = new CharsetDetector();
            detector.setText(body);
            CharsetMatch match = detector.detect();

            if (contentType != null)
            {
                // Record the charset in the content-type header so that its value can be cached
                // and re-used. This is a BIG performance win.
                headers.Add("Content-Type", contentType + "; charset=" + match.getName().ToUpper());
            }
            return match.getName().ToUpper();
        }

        /**
        * @return consolidated cache expiration time or -1
        */
        public DateTime? getCacheExpiration()
        {
            // We intentionally ignore cache-control headers for most HTTP error responses, because if
            // we don't we end up hammering sites that have gone down with lots of requests.  Proper
            // support for caching of OAuth responses is more complex, for that we have to respect
            // cache-control headers for 401s and 403s.
            if (!CACHE_CONTROL_OK_STATUS_CODES.Contains((int)response.StatusCode))
            {
                return date.AddMilliseconds(negativeCacheTtl);
            }
            if (isStrictNoCache())
            {
                return null;
            }
            long maxAge = getCacheControlMaxAge();
            if (maxAge != -1)
            {
                return date.AddMilliseconds(maxAge);
            }
            DateTime? expiration = getExpiresTime();
            if (expiration != null)
            {
                return expiration;
            }
            return date.AddMilliseconds(defaultTtl);
        }

        /**
        * @return Consolidated ttl or -1.
        */
        public long getCacheTtl()
        {
            DateTime? expiration = getCacheExpiration();
            if (expiration != null)
            {
                return DateTime.Now.Subtract(expiration.Value).Seconds;
            }
            return -1;
        }

        /**
        * @return true if a strict no-cache header is set in Cache-Control or Pragma
        */
        public bool isStrictNoCache()
        {
            string cacheControl = getHeader("Cache-Control");
            if (!string.IsNullOrEmpty(cacheControl))
            {
                string[] directives = cacheControl.Split(',');
                foreach (string directive in directives)
                {
                    string dir = directive.Trim().ToLower();
                    if (dir.Equals("no-cache")
                        || dir.Equals("no-store")
                        || dir.Equals("private"))
                    {
                        return true;
                    }
                }
            }

            foreach (string pragma in getHeaders("Pragma"))
            {
                if (pragma.ToLower().Equals("no-cache"))
                {
                    return true;
                }
            }
            return false;
        }

        /**
        * @return the expiration time from the Expires header or -1 if not set
        */
        private DateTime? getExpiresTime()
        {
            String expires = getHeader("Expires");
            if (!string.IsNullOrEmpty(expires))
            {
                DateTime expiresDate = DateTime.Parse(expires);
                if (expiresDate != null)
                {
                    return expiresDate;
                }
            }
            return null;
        }

        /**
        * @return max-age value or -1 if invalid or not set
        */
        private long getCacheControlMaxAge()
        {
            String cacheControl = getHeader("Cache-Control");
            if (cacheControl != null)
            {
                String[] directives = cacheControl.Split(',');
                foreach (String directive in directives)
                {
                    string dir = directive.Trim().ToLower();
                    if (dir.StartsWith("max-age"))
                    {
                        String[] parts = dir.Split('=');
                        if (parts.Length == 2)
                        {
                            try
                            {
                                return long.Parse(parts[1]) * 1000;
                            }
                            catch (FormatException ignore)
                            {
                                return -1;
                            }
                        }
                    }
                }
            }
            return -1;
        }

        /**
        * Tries to find a valid date from the input headers.
        *
        * @return The value of the date header, in milliseconds, or -1 if no Date could be determined.
        */
        public override String ToString()
        {
            StringBuilder buf = new StringBuilder("HTTP/1.1 ").Append(response.StatusCode).Append("\r\n\r\n");
            for (int i = 0; i < response.Headers.Count; i++)
            {
                string name = response.Headers.GetKey(i);
                foreach (string value in response.Headers.GetValues(i))
                {
                    buf.Append(name).Append(": ").Append(value).Append('\n');
                }
            }
            buf.Append("\r\n");
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                buf.Append(sr.ReadToEnd());
            }
            buf.Append("\r\n");
            return buf.ToString();
        }

    } 
}

