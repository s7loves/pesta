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
using System.Configuration;
using Jayrock.Json;
using System.Collections.Generic;

namespace pestaServer.Models.common
{
    /// <summary>
    /// Summary description for ContainerConfig
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public abstract class ContainerConfig
    {
        public static string DEFAULT_CONTAINER = "default";
        /**
       * @return The set of all containers that are currently registered.
       */
        public abstract ICollection<String> getContainers();

        /**
       * Fetches a configuration parameter as a JSON object, array, string, or
       * number, ensuring that it can be safely passed to javascript without any
       * additional filtering.
       *
       * @param parameter The value to fetch. May be specified as an x-path like
       *     object reference such as "gadgets/features/views".
       * @return A configuration parameter as a JSON object or null if not set or
       *     can't be interpreted as JSON.
       *
       * TODO: Convert to a more generalized object.
       */
        public abstract Object getJson(String container, String parameter);

        /**
       * Attempts to fetch a parameter for the given container, or the default
       * container if the specified container is not supported.
       *
       * @return A configuration parameter as a string, or null if not set.
       */
        public abstract String get(String container, String parameter);

        /**
       * @return A configuration parameter as a JSON object or null if not set or
       *     can't be interpreted as JSON.
       */
        public abstract JsonObject getJsonObject(String container, String parameter);

        /**
       * @return A configuration parameter as a JSON object or null if not set or
       *     can't be interpreted as JSON.
       */
        public abstract JsonArray getJsonArray(String container, String parameter);

        public static string getConfigurationValue(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }
    }
}