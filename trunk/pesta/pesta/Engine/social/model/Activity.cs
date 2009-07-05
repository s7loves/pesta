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
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Pesta.Engine.protocol.conversion;
using Pesta.Utilities;

namespace Pesta.Engine.social.model
{
    [XmlRoot(ElementName = "activity", Namespace = BeanConverter.osNameSpace)]
    [DataContract(Name = "activity", Namespace = BeanConverter.osNameSpace)]
    public class Activity
    {
        public Activity()
        {
        }

        public Activity(String id, String userId) : this()
        {
            this.id = id;
            this.userId = userId;
        }

        [DataMember(EmitDefaultValue = false)] public String appId { get; set; }
        [DataMember(EmitDefaultValue = false)] public String body { get; set; }
        [DataMember(EmitDefaultValue = false)] public String bodyId { get; set; }
        [DataMember(EmitDefaultValue = false)] public String externalId { get; set; }
        [DataMember(EmitDefaultValue = false)] public String id { get; set; }

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
        public Dictionary<String, String> templateParams { get; set; }
        [DataMember(EmitDefaultValue = false)] public String title { get; set; }
        [DataMember(EmitDefaultValue = false)] public String titleId { get; set; }
        [DataMember(EmitDefaultValue = false)] public String url { get; set; }
        [DataMember(EmitDefaultValue = false)] public String userId { get; set; }

        public class Field : EnumBaseType<Field>
        {
            /// <summary>
            /// Initializes a new instance of the Field class.
            /// </summary>
            public Field(int key, string value)
                : base(key, value)
            {

            }
            /** the json field for appId. */
            public static readonly Field APP_ID = new Field(1, "appId");
            /** the json field for body. */
            public static readonly Field BODY = new Field(2, "body");
            /** the json field for bodyId. */
            public static readonly Field BODY_ID = new Field(3, "bodyId");
            /** the json field for externalId. */
            public static readonly Field EXTERNAL_ID = new Field(4, "externalId");
            /** the json field for id. */
            public static readonly Field ID = new Field(5, "id");
            /** the json field for updated. */
            public static readonly Field LAST_UPDATED = new Field(6, "updated"); /* Needed to support the RESTful api */
            /** the json field for mediaItems. */
            public static readonly Field MEDIA_ITEMS = new Field(7, "mediaItems");
            /** the json field for postedTime. */
            public static readonly Field POSTED_TIME = new Field(8, "postedTime");
            /** the json field for priority. */
            public static readonly Field PRIORITY = new Field(9, "priority");
            /** the json field for streamFaviconUrl. */
            public static readonly Field STREAM_FAVICON_URL = new Field(10, "streamFaviconUrl");
            /** the json field for streamSourceUrl. */
            public static readonly Field STREAM_SOURCE_URL = new Field(11, "streamSourceUrl");
            /** the json field for streamTitle. */
            public static readonly Field STREAM_TITLE = new Field(12, "streamTitle");
            /** the json field for streamUrl. */
            public static readonly Field STREAM_URL = new Field(13, "streamUrl");
            /** the json field for templateParams. */
            public static readonly Field TEMPLATE_PARAMS = new Field(14, "templateParams");
            /** the json field for title. */
            public static readonly Field TITLE = new Field(15, "title");
            /** the json field for titleId. */
            public static readonly Field TITLE_ID = new Field(16, "titleId");
            /** the json field for url. */
            public static readonly Field URL = new Field(17, "url");
            /** the json field for userId. */
            public static readonly Field USER_ID = new Field(18, "userId");

            public static Field GetByValue(string value)
            {
                return GetBaseByValue(value);
            }

            public override String ToString()
            {
                return Value;
            }
        }
    } 
}
