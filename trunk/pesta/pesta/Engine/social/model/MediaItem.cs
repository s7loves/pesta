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
using Pesta.Engine.social.core.model;
using Pesta.Utilities;


namespace Pesta.Engine.social.model
{
    /// <summary>
    /// Summary description for MediaItem
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    [ImplementedBy(typeof(MediaItemImpl))]
    public abstract class MediaItem
    {
        public class Field : EnumBaseType<Field>
        {
            /// <summary>
            /// Initializes a new instance of the Field class.
            /// </summary>
            public Field(int key, string value)
                : base(key, value)
            {

            }
            /** the field name for mimeType. */
            public static readonly Field MIME_TYPE = new Field(1, "mimeType");
            /** the field name for type. */
            public static readonly Field TYPE = new Field(2, "type");
            /** the field name for url. */
            public static readonly Field URL = new Field(3, "url");

            /**
            * The field name that the instance represents.
            */
            private readonly String jsonString;

            /**
            * create a field base on the an element name.
            *
            * @param jsonString the name of the element
            */
            public Field(String jsonString)
            {
                this.jsonString = jsonString;
            }

            public override String ToString()
            {
                return jsonString;
            }
        }

        /**
        * An enumeration of potential media types.
        */
        public class Type : EnumBaseType<Type>
        {
            public Type(int key, string value)
                : base(key, value)
            {
                jsonString = value;
            }

            /** the constant for audio types. */
            public static readonly Type AUDIO = new Type(1, "audio");
            /** the constant for image types. */
            public static readonly Type IMAGE = new Type(2, "image");
            /** the constant for video types. */
            public static readonly Type VIDEO = new Type(3, "video");

            /**
            * The field type.
            */
            private readonly String jsonString;

            /**
            * Construct a field type based on the name.
            *
            * @param jsonString
            */
            public Type(String jsonString)
            {
                this.jsonString = jsonString;
            }

            public override String ToString()
            {
                return jsonString;
            }
        }

        /**
        * Get the mime type for this Media item.
        *
        * @return the mime type.
        */
        public abstract String getMimeType();

        /**
        * Set the mimetype for this Media Item.
        *
        * @param mimeType the mimeType
        */
        public abstract void setMimeType(String mimeType);

        /**
        * Get the Type of this media item, either audio, image or video.
        *
        * @return the Type of this media item
        */
        public abstract Type getType();

        /**
        * Get the Type of this media item, either audio, image or video.
        *
        * @param the type of this media item
        */
        public abstract void setType(Type type);

        /**
        * Get a URL for the media item.
        *
        * @return the url of the media item
        */
        public abstract String getUrl();

        /**
        * Set a URL for the media item.
        *
        * @param url the media item URL
        */
        public abstract void setUrl(String url);
    } 
}
