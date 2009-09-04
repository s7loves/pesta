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
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace pesta.Data
{
    [XmlRoot(Namespace = Constants.osNameSpace)]
    [DataContract(Name = "MediaItem", Namespace = Constants.osNameSpace)]
    public class MediaItem
    {
        public MediaItem()
        {
        }

        public MediaItem(String mimeType, Type type, String url)
        {
            this.mimeType = mimeType;
            this.type = type;
            this.url = url;
        }

        [DataMember(EmitDefaultValue = false)] 
        public String mimeType { get; set; }
        
        public Type type { get; set; }
        [DataMember(Name = "type")]
        private string _type 
        { 
            get { return type.ToString().ToLower(); }
            set { type = (Type)Enum.Parse(typeof(Type), value, true); } 
        }

        [DataMember(EmitDefaultValue = false)] 
        public String url { get; set; }

        public String thumbnailUrl { get; set; }

        [DataContract]
        public enum Type
        {
            audio,
            image,
            video
        }
    }
}