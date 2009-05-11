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
using Jayrock.Json;
using System.Collections.Generic;
using Pesta.Engine.auth;
using Pesta.Engine.social;
using Pesta.Engine.social.spi;

namespace pestaServer.Models.social.service
{
    /// <summary>
    /// Summary description for RpcRequestItem
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class RpcRequestItem : RequestItem
    {
        private JsonObject data;

        static String getService(String rpcMethod)
        {
            return rpcMethod.Substring(0, rpcMethod.IndexOf('.'));
        }

        static String getOperation(String rpcMethod)
        {
            return rpcMethod.Substring(rpcMethod.IndexOf('.') + 1);
        }

        public RpcRequestItem(JsonObject rpc, ISecurityToken token,
                              BeanConverter converter)
            : base(getService(rpc["method"].ToString()), getOperation(rpc["method"].ToString()), token, converter)
        {
            if (rpc.Contains("params"))
            {
                data = rpc["params"] as JsonObject;
            }
            else
            {
                data = new JsonObject();
            }
        }

        public override String getParameter(String paramName)
        {
            try
            {
                return data.Contains(paramName) ? data[paramName].ToString() : null;
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.BAD_REQUEST, je.Message, je);
            }
        }

        public override String getParameter(String paramName, String defaultValue)
        {
            try
            {
                return data.Contains(paramName) ? data[paramName].ToString() : defaultValue;
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.BAD_REQUEST, je.Message, je);
            }
        }

        public override HashSet<String> getListParameter(String paramName)
        {
            try
            {
                if (data.Contains(paramName))
                {
                    if (data[paramName] is JsonArray)
                    {
                        JsonArray jsonArray = (JsonArray)data[paramName];
                        HashSet<String> returnVal = new HashSet<string>();
                        for (int i = 0; i < jsonArray.Length; i++)
                        {
                            returnVal.Add(jsonArray.GetString(i,null));
                        }
                        return returnVal;
                    }
                    // Allow up-conversion of non-array to array params.
                    return new HashSet<string> { data[paramName].ToString() };
                }
                return new HashSet<string>();
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.BAD_REQUEST, je.Message, je);
            }
        }

        public override object getTypedParameter(String parameterName, Type dataTypeClass)
        {
            try
            {
                return converter.ConvertToObject(data[parameterName].ToString(), dataTypeClass);
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.BAD_REQUEST, je.Message, je);
            }
        }

        public override object getTypedParameters(Type dataTypeClass)
        {
            return converter.ConvertToObject(data.ToString(), dataTypeClass);
        }


        public override void applyUrlTemplate(String urlTemplate)
        {
            // No params in the URL
        }
    }
}