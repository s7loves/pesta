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
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using Jayrock.Json;
using System.Web;
using Pesta.Utilities;

namespace Pesta.Engine.common.util
{
    /// <summary>
    /// Summary description for JsonConversionUtil
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class JsonConversionUtil
    {
        private static readonly Regex ARRAY_MATCH = new Regex("(\\w+)\\((\\d+)\\)", RegexOptions.Compiled);

        private static readonly HashKey<String> RESERVED_PARAMS = new HashKey<string>() { "method", "id", "st" };

        public static JsonObject fromRequest(HttpRequest request)
        {
            //String methodName = request.getPathInfo().replaceAll("/", "");
            JsonObject root = new JsonObject();
            NameValueCollection parameters = request.Params;
            root.Put("method", parameters["method"]);
            if (!string.IsNullOrEmpty(parameters.Get("id")))
            {
                root.Put("id", parameters["id"]);
            }
            JsonObject paramsRoot = new JsonObject();
            for (int i = 0; i < parameters.Count; i++)
            {
                if (!RESERVED_PARAMS.Contains(parameters.GetKey(i).ToLower()))
                {
                    String[] path = parameters.GetKey(i).Split('\\');
                    JsonObject holder = buildHolder(paramsRoot, path, 0);
                    holder.Put(path[path.Length - 1], convertToJsonValue(parameters.GetValues(i)[0]));
                }
            }

            if (paramsRoot.Count > 0)
            {
                root.Put("params", paramsRoot);
            }
            return root;
        }

        /**
       * Parse the steps in the path into JSON Objects.
       */
        static JsonObject buildHolder(JsonObject root, String[] steps, int currentStep)
        {
            if (currentStep > steps.Length - 2)
            {
                return root;
            }
            else
            {
                Match matcher = ARRAY_MATCH.Match(steps[currentStep]);
                if (matcher.Success)
                {
                    // Handle as array
                    String fieldName = matcher.Groups[1].Value;
                    int index = int.Parse(matcher.Groups[2].Value);
                    JsonArray newArrayStep;
                    if (root.Contains(fieldName))
                    {
                        newArrayStep = root[fieldName] as JsonArray;
                    }
                    else
                    {
                        newArrayStep = new JsonArray();
                        root.Put(fieldName, newArrayStep);
                    }
                    JsonObject newStep = new JsonObject();
                    newArrayStep[index] = newStep;
                    return buildHolder(newStep, steps, ++currentStep);
                }
                else
                {
                    JsonObject newStep;
                    if (root.Contains(steps[currentStep]))
                    {
                        newStep = root[steps[currentStep]] as JsonObject;
                    }
                    else
                    {
                        newStep = new JsonObject();
                        root.Put(steps[currentStep], newStep);
                    }
                    return buildHolder(newStep, steps, ++currentStep);
                }
            }
        }

        static Object convertToJsonValue(String value)
        {
            if (value == null)
            {
                return null;
            }
            else if (value.StartsWith("(") && value.EndsWith(")"))
            {
                // explicit form of literal array
                return new JsonArray("[" + value.Substring(1, value.Length - 1) + "]");
            }
            else
            {
                try
                {
                    // inferred parsing of literal array
                    // Attempt to parse as an array of literals
                    JsonArray parsedArray = new JsonArray("[" + value + "]");
                    if (parsedArray.Length == 1)
                    {
                        // Not an array
                        return parsedArray[0];
                    }
                    return parsedArray;
                }
                catch (JsonException)
                {
                    return value;
                }
            }
        }
    }
}