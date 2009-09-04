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
using System.Diagnostics;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using Pesta.Engine.common.xml;
using pestaServer.Models.common;
using pestaServer.Models.gadgets.http;

namespace pestaServer.Models.gadgets
{
    public class JsFeatureLoader
    {
        public static char FILE_SEPARATOR = ',';

        private readonly IHttpFetcher fetcher;

        public JsFeatureLoader()
        {
            fetcher = BasicHttpFetcher.Instance;
        }
        public void LoadFeatures(string path, GadgetFeatureRegistry registry)
        {
            // read features.txt
            List<string> resources = new List<string>();
            List<ParsedFeature> features = new List<ParsedFeature>();
            string[] lines = File.ReadAllLines(path);
            foreach (var entry in lines)
            {
                string line = entry.Trim();
                if (!line.StartsWith("#") && line.Length > 0)
                {
                    resources.Add(line);
                }
            }
            
            foreach (string item in resources)
            {
                string content = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/Content/" + item);
                string loc = item.Substring(0, item.LastIndexOf('/') + 1);
                ParsedFeature feature = Parse(content, loc, true);
                if (feature != null)
                {
                    features.Add(feature);
                }
                
            }
            foreach (ParsedFeature item in features)
            {
                GadgetFeature gadgetFeature = new GadgetFeature(item.Name, item.libraries, item.deps);
                registry.Register(gadgetFeature);
            }
        }

        private ParsedFeature Parse(String xml, String path, bool isResource)
        {
            XmlElement doc = XmlUtil.Parse(xml);

            ParsedFeature feature = new ParsedFeature {BasePath = path, IsResource = isResource};

            XmlNodeList nameNode = doc.GetElementsByTagName("name");
            if (nameNode.Count != 1)
            {
                throw new Exception("No name provided");
            }
            feature.Name = nameNode.Item(0).InnerText;

            XmlNodeList gadgets = doc.GetElementsByTagName("gadget");
            foreach (XmlElement gadget in gadgets)
            {
                ProcessContext(feature, gadget, RenderingContext.GADGET);
            }

            XmlNodeList containers = doc.GetElementsByTagName("container");
            foreach (XmlElement container in containers)
            {
                ProcessContext(feature, container, RenderingContext.CONTAINER);
            }
            XmlNodeList dependencies = doc.GetElementsByTagName("dependency");
            foreach (XmlElement dependency in dependencies)
            {
                feature.deps.Add(dependency.InnerText);
            }

            return feature;
        }

        private void ProcessContext(ParsedFeature feature, XmlElement context, RenderingContext renderingContext)
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
                        Debug.Assert(true); // should never reach here
                        content = content.Substring(6);
                        type = JsLibrary.Type.RESOURCE;
                    }
                    else
                    {
                        content = feature.BasePath + content;
                        type = JsLibrary.Type.FILE;
                    }
                }
                JsLibrary library = JsLibrary.Create(type, content, feature.Name, inlineOk ? fetcher : null);
                foreach (String cont in container.Split(','))
                {
                    feature.AddLibrary(renderingContext, cont.Trim(), library);
                }
            }
        }
    }

    public class ParsedFeature
    {
        public String Name;
        public String BasePath;
        public bool IsResource;
        internal readonly Dictionary<RenderingContext, Dictionary<String, List<JsLibrary>>> libraries;
        internal readonly HashSet<String> deps;

        public ParsedFeature()
        {
            IsResource = false;
            BasePath = "";
            Name = "";
            libraries = new Dictionary<RenderingContext, Dictionary<String, List<JsLibrary>>>();
            deps = new HashSet<string>();
        }

        public void AddLibrary(RenderingContext ctx, String cont, JsLibrary library)
        {
            Dictionary<String, List<JsLibrary>> ctxLibs;
            if (!libraries.TryGetValue(ctx, out ctxLibs))
            {
                ctxLibs = new Dictionary<String, List<JsLibrary>>();
                libraries.Add(ctx, ctxLibs);
            }
            List<JsLibrary> containerLibs;
            if (!ctxLibs.TryGetValue(cont, out containerLibs))
            {
                containerLibs = new List<JsLibrary>();
                ctxLibs.Add(cont, containerLibs);
            }
            containerLibs.Add(library);
        }
    }
}