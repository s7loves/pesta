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
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using URI = System.Uri;

namespace Pesta.Engine.common.uri
{
    public class Uri 
    {
        private readonly String text;
        private readonly String scheme;
        private readonly String authority;
        private readonly String path;
        private readonly String query;
        private readonly String fragment;

        private readonly Dictionary<String, List<String>> queryParameters;

        /// <summary>
        /// Initializes a new instance of the Uri class.
        /// </summary>
        private Uri()
        {
        }

        public Uri(UriBuilder builder)
        {
            scheme = builder.getScheme();
            authority = builder.getAuthority();
            path = builder.getPath();
            query = builder.getQuery();
            fragment = builder.getFragment();
            queryParameters = new Dictionary<string,List<string>>(builder.getQueryParameters());

            StringBuilder _out = new StringBuilder();

            if (!String.IsNullOrEmpty(scheme))
            {
                _out.Append(scheme).Append(':');
            }
            if (!String.IsNullOrEmpty(authority)) 
            {
                _out.Append("//").Append(authority);
            }
            if (!String.IsNullOrEmpty(path)) 
            {
                _out.Append(path);
            }
            if (!String.IsNullOrEmpty(query)) 
            {
                _out.Append('?').Append(query);
            }
            if (!String.IsNullOrEmpty(fragment)) 
            {
                _out.Append('#').Append(fragment);
            }
            text = _out.ToString();
        }

        /**
        * Produces a new Uri from a text representation.
        *
        * @param text The text uri.
        * @return A new Uri, parsed into components.
        */
        public static Uri parse(String text) 
        {
            try 
            {
                if (text == "")
                {
                    return new Uri();
                }
                return fromJavaUri(new URI(text, UriKind.RelativeOrAbsolute));
            } 
            catch (UriFormatException e) 
            {
                throw new Exception(e.Message);
            }
        }

        public static Uri parse(URI baseuri, URI relativeuri)
        {
            try
            {
                return fromJavaUri(new URI(baseuri,relativeuri));
            }
            catch (UriFormatException e)
            {
                throw new Exception(e.Message);
            }
        }
        /**
        * Convert a System.Uri to a Uri.
        */
        public static Uri fromJavaUri(URI uri) 
        {
            if (uri.IsAbsoluteUri)
            {
                return new UriBuilder()
                    .setScheme(uri.Scheme)
                    .setAuthority(uri.Authority)
                    .setPath(uri.AbsolutePath)
                    .setQuery(uri.Query.Length != 0 ? uri.Query.Substring(1) : "") // skip the ?
                    .setFragment(uri.Fragment)
                    .toUri();
            }
            else
            {
                string[] data = uri.OriginalString.Split('?');
                System.Diagnostics.Debug.Assert(data.Length < 2);
                return new UriBuilder()
                    .setPath(data[0])
                    .setQuery(data.Length > 1? data[1]: "")
                    .toUri();
            }
            
        }

        /**
           * @return a System.Uri equal to this Uri.
           */
        public URI toJavaUri()
        {
            try 
            {
                return new URI(ToString());
            } 
            catch (UriFormatException e) 
            {
                // Shouldn't ever happen.
                throw new Exception(e.Message);
            }
        }

        /**
        * Resolves a given url relative to this url. Resolution rules are the same as for
        * {@code java.net.URI.resolve(URI)}
        *
        * @param other The url to resolve against.
        * @return The new url.
        */
        public Uri resolve(Uri relative) 
        {
            if (relative == null)
            {
                return null;
            }
            if (relative.isAbsolute())
            {
                return relative;
            }

            UriBuilder result;
            if (string.IsNullOrEmpty(relative.path) && relative.scheme == null
                && relative.authority == null && relative.query == null
                && relative.fragment != null)
            {
                // if the relative URI only consists of fragment,
                // the resolved URI is very similar to this URI,
                // except that it has the fragement from the relative URI.
                result = new UriBuilder(this);
                result.setFragment(relative.fragment);
            }
            else if (relative.scheme != null)
            {
                result = new UriBuilder(relative);
            }
            else if (relative.authority != null)
            {
                // if the relative URI has authority,
                // the resolved URI is almost the same as the relative URI,
                // except that it has the scheme of this URI.
                result = new UriBuilder(relative);
                result.setScheme(scheme);
            }
            else
            {
                // since relative URI has no authority,
                // the resolved URI is very similar to this URI,
                // except that it has the query and fragment of the relative URI,
                // and the path is different.
                result = new UriBuilder(this);
                result.setFragment(relative.fragment);
                result.setQuery(relative.query);
                String relativePath = (relative.path == null) ? "" : relative.path;
                if (relativePath.StartsWith("/"))
                { //$NON-NLS-1$
                    result.setPath(relativePath);
                }
                else
                {
                    // resolve a relative reference
                    int endindex = path.LastIndexOf('/') + 1;
                    result.setPath(normalizePath(path.Substring(0, endindex) + relativePath));
                }
            }
            Uri resolved = result.toUri();
            validate(resolved);
            return resolved;
        }

        private static void validate(Uri uri)
        {
            if (string.IsNullOrEmpty(uri.authority) &&
                string.IsNullOrEmpty(uri.path) &&
                string.IsNullOrEmpty(uri.query))
            {
                throw new ArgumentException("Invalid scheme-specific part"); 
            }
        }

        /**
        * Dervived from harmony
        * normalize path, and return the resulting string
        */
        private static String normalizePath(String path)
        {
            // count the number of '/'s, to determine number of segments
            int index = -1;
            int pathlen = path.Length;
            int size = 0;
            if (pathlen > 0 && path[0] != '/')
            {
                size++;
            }
            while ((index = path.IndexOf('/', index + 1)) != -1)
            {
                if (index + 1 < pathlen && path[index + 1] != '/')
                {
                    size++;
                }
            }

            String[] seglist = new String[size];
            bool[] include = new bool[size];

            // break the path into segments and store in the list
            int current = 0;
            int index2 = 0;
            index = (pathlen > 0 && path[0] == '/') ? 1 : 0;
            while ((index2 = path.IndexOf('/', index + 1)) != -1)
            {
                seglist[current++] = path.Substring(index, index2 - index);
                index = index2 + 1;
            }

            // if current==size, then the last character was a slash
            // and there are no more segments
            if (current < size)
            {
                seglist[current] = path.Substring(index);
            }

            // determine which segments get included in the normalized path
            for (int i = 0; i < size; i++)
            {
                include[i] = true;
                if (seglist[i].Equals(".."))
                { //$NON-NLS-1$
                    int remove = i - 1;
                    // search back to find a segment to remove, if possible
                    while (remove > -1 && !include[remove])
                    {
                        remove--;
                    }
                    // if we find a segment to remove, remove it and the ".."
                    // segment
                    if (remove > -1 && !seglist[remove].Equals(".."))
                    { //$NON-NLS-1$
                        include[remove] = false;
                        include[i] = false;
                    }
                }
                else if (seglist[i].Equals("."))
                { //$NON-NLS-1$
                    include[i] = false;
                }
            }

            // put the path back together
            StringBuilder newpath = new StringBuilder();
            if (path.StartsWith("/"))
            { //$NON-NLS-1$
                newpath.Append('/');
            }

            for (int i = 0; i < seglist.Length; i++)
            {
                if (include[i])
                {
                    newpath.Append(seglist[i]);
                    newpath.Append('/');
                }
            }

            // if we used at least one segment and the path previously ended with
            // a slash and the last segment is still used, then delete the extra
            // trailing '/'
            if (!path.EndsWith("/") && seglist.Length > 0 //$NON-NLS-1$
                && include[seglist.Length - 1])
            {
                newpath.Remove(newpath.Length - 1, 1);
            }

            String result = newpath.ToString();

            // check for a ':' in the first segment if one exists,
            // prepend "./" to normalize
            index = result.IndexOf(':');
            index2 = result.IndexOf('/');
            if (index != -1 && (index < index2 || index2 == -1))
            {
                newpath.Insert(0, "./"); //$NON-NLS-1$
                result = newpath.ToString();
            }
            return result;
        }

        /**
        * @return True if the Uri is absolute.
        */
        public bool isAbsolute() 
        {
            return scheme != null;
        }

        /**
        * @return The scheme part of the uri, or null if none was specified.
        */
        public String getScheme() 
        {
            return scheme;
        }

        /**
        * @return The authority part of the uri, or null if none was specified.
        */
        public String getAuthority()
        {
            return authority;
        }

        /**
        * @return The path part of the uri, or null if none was specified.
        */
        public String getPath() 
        {
            return path;
        }

        /**
        * @return The query part of the uri, or null if none was specified.
        */
        public String getQuery() 
        {
            return query;
        }

        /**
       * @return The query part of the uri, separated into component parts.
       */
        public Dictionary<String, List<String>> getQueryParameters()
        {
            return queryParameters;
        }

        /**
        * @return All query parameters with the given name.
        */
        public List<String> getQueryParameters(String name) 
        {
            List<String> values = null;
            queryParameters.TryGetValue(name, out values);
            return values;
        }

        /**
       * @return The first query parameter value with the given name.
       */
        public String getQueryParameter(String name) 
        {
            List<String> values = null;
            if (!queryParameters.TryGetValue(name, out values) || values.Count == 0) 
            {
                return null;
            }
            IEnumerator<String> ivalues = values.GetEnumerator();
            ivalues.MoveNext();
            return ivalues.Current;
        }

        /**
       * @return The uri fragment.
       */
        public String getFragment()
        {
            return fragment;
        }

        public override String ToString()
        {
            if (text == null)
            {
                return "";
            }
            return text;
        }

        public override int GetHashCode() 
        {
            return text.GetHashCode();
        }

        public override bool Equals(Object obj)
        {
            if (obj == this) 
            {return true;}
            if (!(obj is Uri)) {return false;}

            return text.Equals(((Uri)obj).text);
        }
    }
}