using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Pesta.Engine.common.uri
{
    public class UriBuilder
    {
        private static readonly Regex QUERY_PATTERN = new Regex("([^&=]+)=([^&=]*)", RegexOptions.Compiled);
        private String scheme;
        private String authority;
        private String path;
        private String query;
        private String fragment;
        private readonly Dictionary<String, List<String>> queryParameters;

        /**
        * Construct a new builder from an existing uri.
        */
        public UriBuilder(Uri uri)
        {
            scheme = uri.getScheme();
            authority = uri.getAuthority();
            path = uri.getPath();
            query = uri.getQuery();
            fragment = uri.getFragment();


            queryParameters = new Dictionary<string,List<string>>();
        }

        /**
        * Create an empty builder.
        */
        public UriBuilder() 
        {
            queryParameters = new Dictionary<string,List<string>>();
        }

        /**
        * Construct a builder by parsing a string.
        */
        public static UriBuilder parse(String text)
        {
            return new UriBuilder(Uri.parse(text));
        }

        /**
        * Convert the builder to a Uri.
        */
        public Uri toUri() 
        {
            return new Uri(this);
        }

        /**
        * @return The scheme part of the uri, or null if none was specified.
        */
        public String getScheme() 
        {
            return scheme;
        }

        public UriBuilder setScheme(String _scheme)
        {
            scheme = _scheme;
            return this;
        }

        /**
        * @return The authority part of the uri, or null if none was specified.
        */
        public String getAuthority() 
        {
            return authority;
        }

        public UriBuilder setAuthority(String _authority) 
        {
            authority = _authority;
            return this;
        }

        /**
        * @return The path part of the uri, or null if none was specified.
        */
        public String getPath() 
        {
            return path;
        }

        /**
        * Sets the path component of the Uri.
        */
        public UriBuilder setPath(String _path) 
        {
            path = _path;
            return this;
        }

        /**
        * @return The query part of the uri, or null if none was specified.
        */
        public String getQuery()
        {
            if (query == null)
            {
                query = joinParameters(queryParameters);
            }
            return query;
        }

        /**
        * Assigns the specified query string as the query portion of the uri, automatically decoding
        * parameters to populate the parameter map for calls to getParameter.
        */
        public UriBuilder setQuery(String _query)
        {
            queryParameters.Clear();
            foreach (var item in splitParameters(_query))
            {
                queryParameters.Add(item.Key,item.Value);
            }
            query = _query;
            return this;
        }

        public UriBuilder addQueryParameter(String name, String value) 
        {
            query = null;
            List<String> parameters;
            if (!queryParameters.TryGetValue(name, out parameters))
            {
                parameters = new List<string>();
                queryParameters.Add(name, parameters);
            }
            parameters.Add(value);
            return this;
        }

        public UriBuilder addQueryParameters(Dictionary<String, String> parameters)
        {
            query = null;
            foreach(var entry in parameters)
            {
                addQueryParameter(entry.Key, entry.Value);
            }
            return this;
        }

        /**
        * Force overwrites a given query parameter with the given value.
        */
        public UriBuilder putQueryParameter(String name, List<String> values)
        {
            queryParameters.Add(name, values);
            return this;
        }

        /**
        * @return The queryParameters part of the uri, separated into component parts.
        */
        public Dictionary<String, List<String>> getQueryParameters()
        {
            return queryParameters;
        }

        /**
        * @return All queryParameters parameters with the given name.
        */
        public List<String> getQueryParameters(String name) 
        {
            List<String> values;
            queryParameters.TryGetValue(name, out values);
            return values;
        }

        /**
        * @return The first queryParameters parameter value with the given name.
        */
        public String getQueryParameter(String name)
        {
            List<String> values;
            if (!queryParameters.TryGetValue(name, out values)) 
            {
                return null;
            }
            if (values.Count == 0)
            {
                return null;
            }
            IEnumerator<String> ivalues = values.GetEnumerator();
            ivalues.MoveNext();
            return ivalues.Current;
        }

        /**
        * @return The queryParameters fragment.
        */
        public String getFragment()
        {
            return fragment;
        }

        public UriBuilder setFragment(String _fragment)
        {
            fragment = _fragment;
            return this;
        }

        /**
        * Utility method for joining key / value pair parameters into a url-encoded string.
        */
        static String joinParameters(ICollection<KeyValuePair<string, List<string>>> query)
        {
            if (query.Count == 0) 
            {
                return null;
            }
            StringBuilder buf = new StringBuilder();
            bool firstDone = false;
            foreach(var entry in query) 
            {
                String name = HttpUtility.UrlEncode(entry.Key);
                foreach(String value in entry.Value) 
                {
                    if (firstDone) 
                    {
                        buf.Append('&');
                    }
                    firstDone = true;

                    buf.Append(name)
                        .Append('=')
                        .Append(HttpUtility.UrlEncode(value));
                }
            }
            return buf.ToString();
        }

        /**
        * Utility method for splitting a parameter string into key / value pairs.
        */
        static Dictionary<String, List<String>> splitParameters(String query) 
        {
            if (String.IsNullOrEmpty(query)) 
            {
                return new Dictionary<string,List<string>>();
            }
            Dictionary<String, List<String>> parameters = new Dictionary<string,List<string>>();
            MatchCollection paramMatcher = QUERY_PATTERN.Matches(query);
            foreach (Match item in paramMatcher)
            {
                String name = HttpUtility.UrlDecode(item.Groups[1].Value);
                String value = HttpUtility.UrlDecode(item.Groups[2].Value);
                List<String> values;
                if (!parameters.TryGetValue(name, out values)) 
                {
                    values = new List<string>();
                    parameters.Add(name, values);
                }
                values.Add(value);
            }
            return parameters;
        }

        public override String ToString()
        {
            return toUri().ToString();
        }

        public override int GetHashCode() 
        {
            return toUri().GetHashCode();
        }

        
        public override bool Equals(Object obj) 
        {
            if (obj == this) 
            {
                return true;
            }
            if (!(obj is UriBuilder)) 
            {
                return false;
            }

            return ToString().Equals(obj.ToString());
        }
    }
}