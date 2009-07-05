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
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Script.Serialization;

namespace Pesta.Engine.protocol.conversion
{
    public class BeanJsonConverter : BeanConverter
    {
        public override String GetContentType()
        {
            return ContentTypes.OUTPUT_JSON_CONTENT_TYPE;
        }

        /**
         * Convert the passed in object to a string.
         *
         * @param pojo The object to convert
         * @return An object whos toString method will return json
         */
        public override String ConvertToString(Object pojo)
        {
            return ConvertToString(pojo, null);
        }

        public override String ConvertToString(Object pojo, RequestItem request)
        {
            return ConvertToJson(pojo);
        }
        /**
         * Convert the passed in object to a json object.
         *
         * @param pojo The object to convert
         * @return An object whos toString method will return json
         */
        public String ConvertToJson(Object poco)
        {
            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new JavaScriptConverter[]{new DataContractJSConverter()});
            string json = serializer.Serialize(poco);
            return json;
        }

        public override T ConvertToObject<T>(String json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof (T),new[]{typeof(JsonSurrogate.SDictionary)},int.MaxValue,true,new JsonSurrogate(),false);
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var obj = serializer.ReadObject(ms);
            return (T)obj;
        }

    }
}