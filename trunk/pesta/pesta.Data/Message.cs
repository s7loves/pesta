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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using pesta.Data.Model.Helpers;

namespace pesta.Data
{
    [XmlRoot(ElementName = "message", Namespace = Constants.osNameSpace)]
    [DataContract(Name = "message", Namespace = Constants.osNameSpace)]
    public class Message
    {
        [DataMember(EmitDefaultValue = false)]
        public String appUrl { get; set; }
        [DataMember(EmitDefaultValue = false)] 
        public String body { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String bodyId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> collectionIds { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String inReplyTo { get; set; }
        [DataMember(EmitDefaultValue = false)] 
        public String title { get; set; }
        [DataMember(EmitDefaultValue = false)] 
        public Type type { get; set; }
        [DataMember(EmitDefaultValue = false)] 
        public String id { get; set; }
        [DataMember(EmitDefaultValue = false)] 
        public List<string> recipients { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> replies { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String senderId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public Status status { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public DateTime timeSent { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String titleId { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public DateTime updated { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<Url> urls { get; set; }

        public enum Field
        {
            [Description("body")]
            BODY,
            [Description("title")]
            TITLE,
            [Description("type")]
            TYPE,
            [Description("id")]
            ID
        }

        public static readonly List<string> ALL_FIELDS = (from Enum e in Enum.GetValues(typeof(Field))
                                                          select e.ToDescriptionString()).ToList();

        public enum Type
        {
            EMAIL, 
            NOTIFICATION, 
            PRIVATE_MESSAGE, 
            PUBLIC_MESSAGE
        }

        public enum Status
        {
            NEW, 
            DELETED, 
            FLAGGED
        }

        public String sanitizeHTML(String htmlStr)
        {
            return Regex.Replace(htmlStr, @"<(.|\n)*?>", string.Empty);
        }


    }
}