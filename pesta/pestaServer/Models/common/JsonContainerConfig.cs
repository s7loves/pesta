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
using System.IO;
using System.Web;
using Jayrock.Json;
using Jayrock.Json.Conversion;

namespace pestaServer.Models.common
{
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    class JsonContainerConfig : ContainerConfig
    {
        
        public static readonly char FILE_SEPARATOR = ',';
        public static readonly String PARENT_KEY = "parent";
        // TODO: Rename this to simply "container", gadgets.container is unnecessary.
        public static readonly String CONTAINER_KEY = "gadgets.container";

        private readonly Dictionary<String, JsonObject> config;

        /**
       * Creates a new, empty configuration.
       * @param containers
       * @throws ContainerConfigException
       */
        public static readonly JsonContainerConfig Instance = new JsonContainerConfig();

        protected JsonContainerConfig()
        {
            config = new Dictionary<String, JsonObject>();
            foreach (var container in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "/config/", "*.js"))
            {
                loadContainers(container);
            }
        }

        public override ICollection<String> getContainers()
        {
            return config.Keys;
        }

        public override Object getJson(String container, String parameter) 
        {
            JsonObject data = config[container];
            if (data == null)
            {
                return null;
            }
            if (parameter == null)
            {
                return data;
            }

            try
            {
                foreach (String param in parameter.Split('/'))
                {
                    Object next = data[param];
                    if (next is JsonObject)
                    {
                        data = (JsonObject)next;
                    }
                    else
                    {
                        return next;
                    }
                }
                return data;
            }
            catch (JsonException e)
            {
                return null;
            }
        }

        public override String get(String container, String parameter) 
        {
            Object data = getJson(container, parameter);
            return data == null ? null : data.ToString();
        }

        public override JsonObject getJsonObject(String container, String parameter)
        {
            Object data = getJson(container, parameter);
            if (data is JsonObject)
            {
                return (JsonObject)data;
            }
            return null;
        }

        public override JsonArray getJsonArray(String container, String parameter) 
        {
            Object data = getJson(container, parameter);
            if (data is JsonArray)
            {
                return (JsonArray)data;
            }
            return null;
        }

        /**
       * Loads containers from the specified resource. Follows the same rules
       * as {@code JsFeatureLoader.loadFeatures} for locating resources.
       *
       * @param path
       * @throws ContainerConfigException
       */
        private void loadContainers(string path)
        {
            string json;
            using (StreamReader sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }
            JsonObject contents = (JsonObject)JsonConvert.Import(json);
            JsonArray containers = (JsonArray)contents[CONTAINER_KEY];
            for (int i = 0; i < containers.Length; i++)
            {
                // Copy the default object and produce a new one.
                String container = containers.GetString(i);
                config.Add(container, contents);
            }
        }
    }
}