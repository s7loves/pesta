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
using System.IO;
using System.Net;
using Pesta.Engine.common;
using pestaServer.Models.gadgets.http;
using Uri=Pesta.Engine.common.uri.Uri;

namespace pestaServer.Models.gadgets
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class JsLibrary
    {
        private readonly Type type;
        public Type _Type
        {
            get
            {
                return type;
            }
        }

        /**
        * The content of the library. May be optimized through minification or
        * other compression techniques. Use debugContent to get the unmodified
        * version.
        */
        private readonly String content;
        public String Content
        {
            get
            {
                return content;
            }
        }

        /**
        * Unmodified content. May be identical to content if no optimized version of
        * the script exists.
        */
        private String debugContent;
        public String DebugContent
        {
            get
            {
                return debugContent;
            }
        }

        /**
        * The feature that this library belongs to; may be null;
        */
        private String feature;
        public String Feature
        {
            get
            {
                return feature;
            }
            set
            {
                if (feature == value)
                    return;
                feature = value;
            }
        }

        public override String ToString()
        {
            if (type == Type.URL)
            {
                return "<script src=\"" + content + "\"></script>";
            }
            return "<script><!--\n" + content + "\n--></script>";
        }

        /**
        * Indicates how to load a given resource.
        */
        public enum Type
        {
            FILE, RESOURCE, URL, INLINE
        }

        /**
        * Creates a new js library.
        *
        * @param type If FILE or RESOURCE, the content will be loaded from disk.
        *     if URL or INLINE, the content will be handled the same as html <script>
        * @param content If FILE or RESOURCE, we will also look for a file
        *     named file.opt.ext for every file.ext, and if present we will
        *     use that as the standard content and file.ext as the debug content.
        * @param feature The name of the feature that this library was created for
        *     may be null.
        * @param fetcher Used to retrieve Type.URL; if null, Type.URL will not be
        *     kept as a url reference, otherwise the file will be fetched and treated
        *     as a FILE type.
        * @return The newly created library.
        * @throws GadgetException
        */
        public static JsLibrary Create(Type type, String content, String feature, IHttpFetcher fetcher)
        {
            String optimizedContent = null;
            String debugContent;
            switch (type)
            {
                case Type.FILE:
                case Type.RESOURCE:
                    if (content.EndsWith(".js"))
                    {
                        optimizedContent = LoadData(content.Substring(0, content.Length - 3) + ".opt.js", type);
                    }
                    debugContent = LoadData(content, type);
                    if (String.IsNullOrEmpty(optimizedContent))
                    {
                        optimizedContent = debugContent;
                    }
                    break;
                case Type.URL:
                    if (fetcher == null)
                    {
                        debugContent = optimizedContent = content;
                    }
                    else
                    {
                        type = Type.FILE;
                        debugContent = optimizedContent = LoadDataFromUrl(content, fetcher);
                    }
                    break;
                default:
                    debugContent = content;
                    optimizedContent = content;
                    break;
            }
            return new JsLibrary(feature, type, optimizedContent, debugContent);
        }

        /**
        * Loads an external resource.
        * @param name
        * @param type
        * @return The contents of the file or resource named by @code name.
        */
        private static String LoadData(String name, Type type)
        {
            if (type == Type.FILE)
            {
                return LoadFile(name);
            }
            if (type == Type.RESOURCE)
            {
                return LoadResource(name);
            }
            return null;
        }

        /**
        * Retrieves js content from the given url.
        *
        * @param url
        * @param fetcher
        * @return The contents of the JS file, or null if it can't be fetched.
        * @throws GadgetException
        */
        private static String LoadDataFromUrl(String url, IHttpFetcher fetcher)
        {
            
            // set up the request and response objects
            Uri uri = Uri.parse(url);
            sRequest request = new sRequest(uri);
            sResponse response = fetcher.fetch(request);
            if (response.getHttpStatusCode() == (int)HttpStatusCode.OK)
            {
                return response.responseString;
            }
            return null;
        }

        /**
        * Loads a file
        * @param fileName
        * @return The contents of the file.
        */
        private static String LoadFile(String fileName)
        {
            // try to use MapPath to get the file, this throws an error if the file doesn't exist
            // use HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath)??
            string path2 = AppDomain.CurrentDomain.BaseDirectory + "/Content/" + fileName;
            if (!File.Exists(path2))
            {
                return "";
            }
            string html = File.ReadAllText(path2);
            
            return html;
        }

        /**
        * Loads a resource.
        * @param name
        * @return The contents of the named resource.
        */
        private static String LoadResource(String name)
        {
            try
            {
                return ResourceLoader.GetContent(name);
            }
            catch
            {
                return null;
            }
        }

        public override int GetHashCode()
        {
            return content.GetHashCode() + type.GetHashCode();
        }

        public override bool Equals(Object rhs)
        {
            if (rhs == this)
            {
                return true;
            }
            if (rhs is JsLibrary)
            {
                JsLibrary lib = (JsLibrary)rhs;
                return content.Equals(lib.content) && type.Equals(lib.type);
            }
            return false;
        }

        /**
        * @param feature
        * @param type
        * @param content
        * @param debugContent
        */
        private JsLibrary(String feature, Type type, String content, String debugContent)
        {
            this.feature = feature;
            this.type = type;
            this.content = content;
            this.debugContent = debugContent;
        }
    }
}