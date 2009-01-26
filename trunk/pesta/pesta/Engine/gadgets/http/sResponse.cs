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
using System.Text;
using System;
using System.Collections.Specialized;
using com.ibm.icu.text;

namespace Pesta.Engine.gadgets.http
{
    /// <summary>
    /// Represents the results of an HTTP content retrieval operation.
    /// HttpResponse objects are immutable in order to allow them to be safely used
    /// in concurrent caches and by multiple threads without worrying about
    /// concurrent modification.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class sResponse
    {
        public const int SC_CONTINUE = 100;
        public const int SC_SWITCHING_PROTOCOLS = 101;

        public const int SC_OK = 200;
        public const int SC_CREATED = 201;
        public const int SC_ACCEPTED = 202;
        public const int SC_NON_AUTHORITATIVE_INFORMATION = 203;
        public const int SC_NO_CONTENT = 204;
        public const int SC_RESET_CONTENT = 205;
        public const int SC_PARTIAL_CONTENT = 206;

        public const int SC_MULTIPLE_CHOICES = 300;
        public const int SC_MOVED_PERMANENTLY = 301;
        public const int SC_FOUND = 302;
        public const int SC_SEE_OTHER = 303;
        public const int SC_NOT_MODIFIED = 304;
        public const int SC_USE_PROXY = 305;
        public const int SC_TEMPORARY_REDIRECT = 307;

        public const int SC_BAD_REQUEST = 400;
        public const int SC_UNAUTHORIZED = 401;
        public const int SC_PAYMENT_REQUIRED = 402;
        public const int SC_FORBIDDEN = 403;
        public const int SC_NOT_FOUND = 404;
        public const int SC_METHOD_NOT_ALLOWED = 405;
        public const int SC_NOT_ACCEPTABLE = 406;
        public const int SC_PROXY_AUTHENTICATION_REQUIRED = 407;
        public const int SC_REQUEST_TIMEOUT = 408;
        public const int SC_CONFLICT = 409;
        public const int SC_GONE = 410;
        public const int SC_LENGTH_REQUIRED = 411;
        public const int SC_PRECONDITION_FAILED = 412;
        public const int SC_REQUEST_ENTITY_TOO_LARGE = 413;
        public const int SC_REQUEST_URI_TOO_LONG = 414;
        public const int SC_UNSUPPORTED_MEDIA_TYPE = 415;
        public const int SC_REQUESTED_RANGE_NOT_SATISFIABLE = 416;
        public const int SC_EXPECTATION_FAILED = 417;

        public const int SC_INTERNAL_SERVER_ERROR = 500;
        public const int SC_NOT_IMPLEMENTED = 501;
        public const int SC_BAD_GATEWAY = 502;
        public const int SC_SERVICE_UNAVAILABLE = 503;
        public const int SC_GATEWAY_TIMEOUT = 504;
        public const int SC_HTTP_VERSION_NOT_SUPPORTED = 505;

        // These content types can always skip encoding detection.
        private static readonly List<string> BINARY_CONTENT_TYPES = new List<string>
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
        private static readonly List<int> NEGATIVE_CACHING_EXEMPT_STATUS =
            new List<int> { SC_UNAUTHORIZED, SC_FORBIDDEN };

        // TTL to use when an error response is fetched. This should be non-zero to
        // avoid high rates of requests to bad urls in high-traffic situations.
        internal const long DEFAULT_NEGATIVE_CACHE_TTL = 30 * 1000;

        // Default TTL for an entry in the cache that does not have any cache
        // control headers.
        internal const long DEFAULT_TTL = 5L * 60L * 1000L;

        internal const String DEFAULT_ENCODING = "UTF-8";

        private const long negativeCacheTtl = DEFAULT_NEGATIVE_CACHE_TTL;

        private const long defaultTtl = DEFAULT_TTL;

        public readonly byte[] responseBytes;
        public String responseString = "";
        private readonly Dictionary<string, string> metadata;
        private readonly NameValueCollection headers;
        private readonly long date;
        private readonly int httpStatusCode;
        private readonly String encoding;

        /**
       * Construct an HttpResponse from a builder (called by HttpResponseBuilder.create).
       */
        public sResponse(HttpResponseBuilder builder)
        {
            httpStatusCode = builder.getHttpStatusCode();
            NameValueCollection headerCopy = new NameValueCollection {builder.getHeaders()};

            // Always safe, HttpResponseBuilder won't modify the body.
            responseBytes = builder.getResponse();

            Dictionary<String, String> metadataCopy = new Dictionary<string,string>(builder.getMetadata());
            metadata = metadataCopy;

            // We want to modify the headers to ensure that the proper Content-Type and Date headers
            // have been set. This allows us to avoid these expensive calculations from the cache.
            date = getAndUpdateDate(headerCopy);
            headers = headerCopy;
            encoding = getAndUpdateEncoding(headerCopy, responseBytes);
            Encoding encoder = Encoding.GetEncoding(encoding);
            if (responseBytes != null)
            {
            responseString = encoder.GetString(responseBytes);
            }

            // Strip BOM if present
            if (responseString.Length > 0 && responseString[0] == 0xFEFF)
            {
                responseString = responseString.Substring(1);
            }
        }

        private sResponse(int httpStatusCode, String body)
            : this(new HttpResponseBuilder()
                      .setHttpStatusCode(httpStatusCode)
                      .setResponseString(body))
        {
    
        }

        public sResponse(String body)
                : this(SC_OK, body)
        {

        }

        public static sResponse error()
        {
            return new sResponse(SC_INTERNAL_SERVER_ERROR, "");
        }

        public static sResponse timeout()
        {
            return new sResponse(SC_GATEWAY_TIMEOUT, "");
        }

        public static sResponse notFound()
        {
            return new sResponse(SC_NOT_FOUND, "");
        }

        public int getHttpStatusCode()
        {
            return httpStatusCode;
        }

        /**
        * @return the content length
        */
        public long getContentLength()
        {
            return responseBytes.Length;
        }

        /**
        * @return All headers for this object.
        */
        public NameValueCollection getHeaders()
        {
            return headers;
        }

        /**
        * @return All headers with the given name. If no headers are set for the given name, an empty
        * collection will be returned.
        */
        public string[] getHeaders(String name)
        {
            string[] retv = headers.GetValues(name);
            if (retv == null)
            {
                return new[] { "" };
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
            return headerList[0];
        }

        /**
        * @return additional data to embed in responses sent from the JSON proxy.
        */
        public Dictionary<string, string> getMetadata()
        {
            return metadata;
        }
        /**
   * Tries to find a valid date from the input headers.
   *
   * @return The value of the date header, in milliseconds, or -1 if no Date could be determined.
   */
        private static long getAndUpdateDate(NameValueCollection headers)
        {
            // Validate the Date header. Must conform to the HTTP date format.
            long timestamp = -1;
            String[] dates = headers["Date"] == null ? null : headers["Date"].Split(';');
            String dateStr = dates == null ? null : dates.Length == 0 ? null : dates[0];
            if (dateStr != null)
            {
                DateTime d;
                if (DateTime.TryParse(dateStr, out d))
                {
                    timestamp = d.Ticks;
                }
            }
            if (timestamp == -1)
            {
                timestamp = DateTime.Now.Ticks;
                headers.Add("Date", new DateTime(timestamp).ToString());
            }
            return timestamp;
        }

        /**
   * Attempts to determine the encoding of the body. If it can't be determined, we use
   * DEFAULT_ENCODING instead.
   *
   * @return The detected encoding or DEFAULT_ENCODING.
   */
        private static String getAndUpdateEncoding(NameValueCollection headers, byte[] body)
        {
            String values = headers["Content-Type"];
            String contentType = values == null ? null : values.Length == 0 ? null : values;
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
                        String charset = parts[1].Substring(offset + 8).ToUpper();
                        // Some servers include quotes around the charset:
                        //   Content-Type: text/html; charset="UTF-8"
                        if (charset[0] == '"')
                        {
                            charset = charset.Substring(1, charset.Length);
                        }
                        return charset;
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
                headers.Add("Content-Type",
                            contentType + "; charset=" + match.getName().ToUpper());
            }
            return match.getName().ToUpper();
        }
        /**
   * @return consolidated cache expiration time or -1
   */
        public long getCacheExpiration()
        {
            // We intentionally ignore cache-control headers for most HTTP error responses, because if
            // we don't we end up hammering sites that have gone down with lots of requests. Certain classes
            // of client errors (authentication) have more severe behavioral implications if we cache them.
            if (isError() && !NEGATIVE_CACHING_EXEMPT_STATUS.Contains(httpStatusCode))
            {
                return date + negativeCacheTtl;
            }

            // We technically shouldn't be caching certain 300 class status codes either, such as 302, but
            // in practice this is a better option for performance.
            if (isStrictNoCache())
            {
                return -1;
            }
            long maxAge = getCacheControlMaxAge();
            if (maxAge != -1)
            {
                return date + maxAge;
            }
            long expiration = getExpiresTime();
            if (expiration != -1)
            {
                return expiration;
            }
            return date + defaultTtl;
        }

        /**
         * @return Consolidated ttl or -1.
         */
        public long getCacheTtl()
        {
            long expiration = getCacheExpiration();
            if (expiration != -1)
            {
                return expiration - DateTime.UtcNow.Ticks;
            }
            return -1;
        }

        /**
         * @return True if this result is stale.
         */
        public bool isStale()
        {
            return getCacheTtl() <= 0;
        }

        /**
       * @return True if the status code is considered to be an error.
       */
        public bool isError()
        {
            return httpStatusCode >= 400;
        }

        /**
        * @return true if a strict no-cache header is set in Cache-Control or Pragma
        */
        public bool isStrictNoCache()
        {
            if (isError() && !NEGATIVE_CACHING_EXEMPT_STATUS.Contains(httpStatusCode))
            {
                return true;
            }
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
        private long getExpiresTime()
        {
            String expires = getHeader("Expires");
            if (!String.IsNullOrEmpty(expires))
            {
                DateTime expiresDate;
                if (DateTime.TryParse(expires,out expiresDate))
                {
                    return expiresDate.Ticks;
                }
            }
            return -1;
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
                            catch (FormatException)
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
            StringBuilder buf = new StringBuilder("HTTP/1.1 ").Append(httpStatusCode).Append("\r\n\r\n");
            for (int i = 0; i < headers.Count; i++)
            {
                string name = headers.GetKey(i);
                var values = headers.GetValues(i);
                if (values != null)
                {
                    foreach (string value in values)
                    {
                        buf.Append(name).Append(": ").Append(value).Append('\n');
                    }
                }
            }
            buf.Append("\r\n");
            buf.Append(responseString);
            buf.Append("\r\n");
            return buf.ToString();
        }

    }
}