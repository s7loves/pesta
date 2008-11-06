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
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using Jayrock.Json;
using Jayrock.Json.Conversion;

namespace Pesta
{
    /// <summary>
    /// Summary description for ContainerConfig
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class ContainerConfig
    {
        public static string DEFAULT_CONTAINER = "default";
        private static string CONTAINER_KEY = "gadgets.container";
        private java.util.Map config;

        public static readonly ContainerConfig Instance =
                new ContainerConfig(AppDomain.CurrentDomain.BaseDirectory + @"config\container.js");

        protected ContainerConfig(string path)
        {
            config = new java.util.HashMap();
            loadContainers(path);
        }
        public java.util.Collection getContainers()
        {
            return config.keySet();
        }
        public String get(String container, String parameter)
        {
            Object data = getJson(container, parameter);
            return data == null ? null : data.ToString();
        }
        public Object getJson(String container, String parameter)
        {
            JsonObject data = config.get(container) as JsonObject;
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

        public JsonObject getJsonObject(String container, String parameter)
        {
            Object data = getJson(container, parameter);
            if (data is JsonObject)
            {
                return (JsonObject)data;
            }
            return null;
        }

        public JsonArray getJsonArray(String container, String parameter)
        {
            Object data = getJson(container, parameter);
            if (data is JsonArray)
            {
                return (JsonArray)data;
            }
            return null;
        }

        private void loadContainers(string path)
        {
            string json;
            using (StreamReader sr = new StreamReader(path))
            {
                json = sr.ReadToEnd();
            }
            JsonObject contents = JsonConvert.Import(json) as JsonObject;
            JsonArray containers = contents[CONTAINER_KEY] as JsonArray;
            for (int i = 0; i < containers.Length; i++)
            {
                // Copy the default object and produce a new one.
                String container = containers.GetString(i);
                config.put(container, contents);
            }
        }
    } 
}
