using System;
using System.Collections.Generic;
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
        * Convert a java.net.URI to a Uri.
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
           * @return a java.net.URI equal to this Uri.
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
        public Uri resolve(Uri other) 
        {
            // TODO: We can probably make this more efficient by implementing resolve ourselves.
            if (other == null) 
            {
                return null;
            }
            
            URI result = null;
            URI.TryCreate(this.toJavaUri(), other.toJavaUri(), out result);
            return fromJavaUri(result);
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