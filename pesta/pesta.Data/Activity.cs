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
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace pesta.Data
{
    [XmlRoot(ElementName = "activity", Namespace = Constants.osNameSpace)]
    [DataContract(Name = "activity", Namespace = Constants.osNameSpace)]
    public class Activity
    {
        public Activity()
        {

        }

        public Activity(String id, String userId) 
        {
            this.id = id;
            this.userId = userId;
        }

        [DataMember(EmitDefaultValue = false)] 
        public String appId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String body { get; set; }
        [DataMember(EmitDefaultValue = false)] 
        public String bodyId { get; set; }
        [DataMember(EmitDefaultValue = false)] 
        public String externalId { get; set; }
        [DataMember(EmitDefaultValue = false)] 
        public String id { get; set; }

        [DataMember(EmitDefaultValue = false)] 
        public DateTime? updated { get; set; }
        [XmlIgnore]
        public bool updatedSpecified { get { return updated.HasValue; } }

        [XmlElement]
        [DataMember(EmitDefaultValue = false)]
        public List<MediaItem> mediaItems { get; set; }

        [DataMember(EmitDefaultValue = false)] public long? postedTime { get; set; }
        [XmlIgnore]
        public bool postedTimeSpecified { get { return postedTime.HasValue; } }
        [DataMember(EmitDefaultValue = false)] public float? priority { get; set; }
        [XmlIgnore]
        public bool prioritySpecified { get { return priority.HasValue; } }
        [DataMember(EmitDefaultValue = false)] public String streamFaviconUrl { get; set; }
        [DataMember(EmitDefaultValue = false)] public String streamSourceUrl { get; set; }
        [DataMember(EmitDefaultValue = false)] public String streamTitle { get; set; }
        [DataMember(EmitDefaultValue = false)] public String streamUrl { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public Dictionary<String, Object> templateParams { get; set; }
        [DataMember(EmitDefaultValue = false)] public String title { get; set; }
        [DataMember(EmitDefaultValue = false)] public String titleId { get; set; }
        [DataMember(EmitDefaultValue = false)] public String url { get; set; }
        [DataMember(EmitDefaultValue = false)] public String userId { get; set; }

        public enum Field
        {
            [Description("appId")]
            APP_ID,
            [Description("body")]
            BODY,
            [Description("bodyId")]
            BODY_ID,
            [Description("externalId")]
            EXTERNAL_ID,
            [Description("id")]
            ID,
            [Description("updated")]
            LAST_UPDATED,
            [Description("mediaItems")]
            MEDIA_ITEMS,
            [Description("postedTime")]
            POSTED_TIME,
            [Description("priority")]
            PRIORITY,
            [Description("streamFaviconUrl")]
            STREAM_FAVICON_URL,
            [Description("streamSourceUrl")]
            STREAM_SOURCE_URL,
            [Description("streamTitle")]
            STREAM_TITLE,
            [Description("streamUrl")]
            STREAM_URL,
            [Description("templateParams")]
            TEMPLATE_PARAMS,
            [Description("title")]
            TITLE,
            [Description("titleId")]
            TITLE_ID,
            [Description("url")]
            URL,
            [Description("userId")]
            USER_ID
        }
    }
}