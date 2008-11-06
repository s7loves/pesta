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
using System.Data;
using System.Configuration;
using System.Xml;
using System.Web;
using System.Net;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace Pesta
{
    /// <summary>
    /// Summary description for JsFeatureLoader
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class JsFeatureLoader
    {
        public static char FILE_SEPARATOR = ',';

        //private WebRequest fetcher;

        public JsFeatureLoader()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public void loadFeatures(string path, GadgetFeatureRegistry registry)
        {
            // read features.txt
            List<string> resources = new List<string>();
            List<ParsedFeature> features = new List<ParsedFeature>();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream s = assembly.GetManifestResourceStream(PestaSettings.ResourcePrefixName + "." + path);
            using (StreamReader sr = new StreamReader(s))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (!line.StartsWith("#") && line.Length > 0)
                    {
                        resources.Add(line);
                    }
                }
            }
            foreach (string item in resources)
            {
                s = assembly.GetManifestResourceStream(PestaSettings.ResourcePrefixName + "." + item.Replace('/', '.'));
                using (StreamReader sr = new StreamReader(s))
                {
                    string content = sr.ReadToEnd();
                    string loc = PestaSettings.ResourcePrefixName + "." + item.Substring(0, item.LastIndexOf('/') + 1).Replace('/', '.');
                    ParsedFeature feature = Parse(content, loc, true);
                    if (feature != null)
                    {
                        features.Add(feature);
                    }
                }
            }
            foreach (ParsedFeature item in features)
            {
                GadgetFeature gadgetFeature = new GadgetFeature(item.name, item.libraries, item.deps);
                registry.register(gadgetFeature);
            }
        }

        private ParsedFeature Parse(String xml, String path, bool isResource)
        {
            XmlElement doc;
            try
            {
                doc = XmlUtil.Parse(xml);
            }
            catch (XmlException e)
            {
                throw e;
            }

            ParsedFeature feature = new ParsedFeature();

            feature.basePath = path;
            feature.isResource = isResource;

            XmlNodeList nameNode = doc.GetElementsByTagName("name");
            if (nameNode.Count != 1)
            {
                throw new Exception("No name provided");
            }
            feature.name = nameNode.Item(0).InnerText;

            XmlNodeList gadgets = doc.GetElementsByTagName("gadget");
            foreach (XmlElement gadget in gadgets)
            {
                processContext(feature, gadget, RenderingContext.GADGET);
            }

            XmlNodeList containers = doc.GetElementsByTagName("container");
            foreach (XmlElement container in containers)
            {
                processContext(feature, container, RenderingContext.CONTAINER);
            }
            XmlNodeList dependencies = doc.GetElementsByTagName("dependency");
            foreach (XmlElement dependency in dependencies)
            {
                feature.deps.Add(dependency.InnerText);
            }

            return feature;
        }

        private void processContext(ParsedFeature feature, XmlElement context, RenderingContext renderingContext)
        {
            String container = XmlUtil.getAttribute(context, "container", ContainerConfig.DEFAULT_CONTAINER);
            XmlNodeList libraries = context.GetElementsByTagName("script");
            foreach (XmlElement script in libraries)
            {
                bool inlineOk = XmlUtil.getBoolAttribute(script, "inline", true);
                String source = XmlUtil.getAttribute(script, "src");
                String content;
                JsLibrary.Type type;
                if (source == null)
                {
                    type = JsLibrary.Type.INLINE;
                    content = script.InnerText;
                }
                else
                {
                    content = source;
                    if (content.StartsWith("http://"))
                    {
                        type = JsLibrary.Type.URL;
                    }
                    else if (content.StartsWith("//"))
                    {
                        type = JsLibrary.Type.URL;
                        content = content.Substring(1);
                    }
                    else if (content.StartsWith("res://"))
                    {
                        content = content.Substring(6);
                        type = JsLibrary.Type.RESOURCE;
                    }
                    else if (feature.basePath.StartsWith(PestaSettings.ResourcePrefixName))
                    {
                        content = feature.basePath + content;
                        type = JsLibrary.Type.RESOURCE;
                    }
                    else
                    {
                        content = feature.basePath + content;
                        type = JsLibrary.Type.FILE;
                    }
                }
                JsLibrary library = JsLibrary.create(type, content, feature.name, inlineOk ? HttpContext.Current : null);
                foreach (String cont in container.Split(','))
                {
                    feature.AddLibrary(renderingContext, cont.Trim(), library);
                }
            }
        }
    }

    public class ParsedFeature
    {
        public String name;
        public String basePath;
        public bool isResource;
        internal readonly Dictionary<RenderingContext, Dictionary<String, List<JsLibrary>>> libraries;
        internal readonly HashSet<String> deps;

        public ParsedFeature()
        {
            this.isResource = false;
            this.basePath = "";
            this.name = "";
            libraries = new Dictionary<RenderingContext, Dictionary<String, List<JsLibrary>>>();
            deps = new HashSet<string>();
        }

        public void AddLibrary(RenderingContext ctx, String cont, JsLibrary library)
        {
            Dictionary<String, List<JsLibrary>> ctxLibs = null;
            if (!libraries.TryGetValue(ctx, out ctxLibs))
            {
                ctxLibs = new Dictionary<String, List<JsLibrary>>();
                libraries.Add(ctx, ctxLibs);
            }
            List<JsLibrary> containerLibs = null;
            if (!ctxLibs.TryGetValue(cont, out containerLibs))
            {
                containerLibs = new List<JsLibrary>();
                ctxLibs.Add(cont, containerLibs);
            }
            containerLibs.Add(library);
        }
    }
}