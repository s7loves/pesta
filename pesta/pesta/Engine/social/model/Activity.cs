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
using Pesta.Engine.social.core.model;
using Pesta.Utilities;

namespace Pesta.Engine.social.model
{
    /// <summary>
    /// Summary description for Activity
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    [ImplementedBy(typeof(ActivityImpl))]
    public abstract class Activity
    {
        public class Field : EnumBaseType<Field>
        {
            /// <summary>
            /// Initializes a new instance of the Field class.
            /// </summary>
            public Field()
            {
            }
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

            /**
            * The json field that the instance represents.
            */
            private readonly String jsonString;

            /**
            * create a field base on the a json element.
            *
            * @param jsonString the name of the element
            */
            private Field(String jsonString)
            {
                this.jsonString = jsonString;
            }

            /**
            * emit the field as a json element.
            *
            * @return the field name
            */
            public override String ToString()
            {
                return jsonString;
            }
        }

        /**
        * Get a string specifying the application that this activity is associated with. Container
        * support for this field is REQUIRED.
        *
        * @return A string specifying the application that this activity is associated with
        */
        public abstract String getAppId();

        /**
        * Set a string specifying the application that this activity is associated with. Container
        * support for this field is REQUIRED.
        *
        * @param appId A string specifying the application that this activity is associated with
        */
        public abstract void setAppId(String appId);

        /**
        * Get a string specifying an optional expanded version of an activity. Container support for this
        * field is OPTIONAL.
        *
        * @return a string specifying an optional expanded version of an activity.
        */
        public abstract String getBody();

        /**
        * Set a string specifying an optional expanded version of an activity. Container support for this
        * field is OPTIONAL.
        *
        * Bodies may only have the following HTML tags:&lt;b&gt; &lt;i&gt;, &lt;a&gt;, &lt;span&gt;. The
        * container may ignore this formatting when rendering the activity.
        *
        *
        * @param body a string specifying an optional expanded version of an activity.
        */
        public abstract void setBody(String body);

        /**
        * Get a string specifying the body template message ID in the gadget spec. Container support for
        * this field is OPTIONAL.
        *
        * Bodies may only have the following HTML tags: &lt;b&gt; &lt;i&gt;, &lt;a&gt;, &lt;span&gt;. The
        * container may ignore this formatting when rendering the activity.
        *
        * @return a string specifying the body template message ID in the gadget spec.
        */
        public abstract String getBodyId();

        /**
        * Set a string specifying the body template message ID in the gadget spec. Container support for
        * this field is OPTIONAL.
        *
        * @param bodyId a string specifying the body template message ID in the gadget spec.
        */
        public abstract void setBodyId(String bodyId);

        /**
        * Get an optional string ID generated by the posting application. Container support for this
        * field is OPTIONAL.
        *
        * @return An optional string ID generated by the posting application.
        */
        public abstract String getExternalId();

        /**
        * Set an optional string ID generated by the posting application. Container support for this
        * field is OPTIONAL.
        *
        * @param externalId An optional string ID generated by the posting application.
        */
        public abstract void setExternalId(String externalId);

        /**
        * Get a string ID that is permanently associated with this activity. Container support for this
        * field is OPTIONAL.
        *
        * @return a string ID that is permanently associated with this activity.
        */
        public abstract String getId();

        /**
        * Set a string ID that is permanently associated with this activity. Container support for this
        * field is OPTIONAL.
        *
        * @param id a string ID that is permanently associated with this activity.
        */
        public abstract void setId(String id);

        /**
        * Get the last updated date of the Activity, additional to the Opensocial specification for the
        * REST-API. Container support for this field is OPTIONAL.
        *
        * @return the last updated date
        */
        public abstract DateTime? getUpdated();

        /**
        * . Set the last updated date of the Activity, additional to the Opensocial specification for the
        * REST-API. Container support for this field is OPTIONAL.
        *
        * @param updated the last updated date
        */
        public abstract void setUpdated(DateTime? updated);

        /**
        * Get any photos, videos, or images that should be associated with the activity.
        *
        * Container support for this field is OPTIONAL.
        *
        * @return A List of {@link MediaItem} containing any photos, videos, or images that should be
        *         associated with the activity.
        */
        public abstract List<MediaItem> getMediaItems();

        /**
        * Set any photos, videos, or images that should be associated with the activity. Container
        * support for this field is OPTIONAL.
        *
        * Higher priority ones are higher in the list.
        *
        * @param mediaItems a list of {@link MediaItem} to be associated with the activity
        */
        public abstract void setMediaItems(List<MediaItem> mediaItems);

        /**
        * Get the time at which this activity took place in milliseconds since the epoch. Container
        * support for this field is OPTIONAL.
        *
        * Higher priority ones are higher in the list.
        *
        * @return The time at which this activity took place in milliseconds since the epoch
        */
        public abstract long? getPostedTime();

        /**
        * Set the time at which this activity took place in milliseconds since the epoch Container
        * support for this field is OPTIONAL.
        *
        * This value can not be set by the end user.
        *
        * @param postedTime the time at which this activity took place in milliseconds since the epoch
        */
        public abstract void setPostedTime(long? postedTime);

        /**
        * Get the priority, a number between 0 and 1 representing the relative priority of this activity
        * in relation to other activities from the same source. Container support for this field is
        * OPTIONAL.
        *
        * @return a number between 0 and 1 representing the relative priority of this activity in
        *         relation to other activities from the same source
        */
        public abstract float? getPriority();

        /**
        * Set the priority, a number between 0 and 1 representing the relative priority of this activity
        * in relation to other activities from the same source. Container support for this field is
        * OPTIONAL.
        *
        * @param priority a number between 0 and 1 representing the relative priority of this activity in
        *                relation to other activities from the same source.
        */
        public abstract void setPriority(float? priority);

        /**
        * Get a string specifying the URL for the stream's favicon. Container support for this field is
        * OPTIONAL.
        *
        * @return a string specifying the URL for the stream's favicon.
        */
        public abstract String getStreamFaviconUrl();

        /**
        * Set a string specifying the URL for the stream's favicon. Container support for this field is
        * OPTIONAL.
        *
        * @param streamFaviconUrl a string specifying the URL for the stream's favicon.
        */
        public abstract void setStreamFaviconUrl(String streamFaviconUrl);

        /**
        * Get a string specifying the stream's source URL. Container support for this field is OPTIONAL.
        *
        * @return a string specifying the stream's source URL.
        */
        public abstract String getStreamSourceUrl();

        /**
        * Set a string specifying the stream's source URL. Container support for this field is OPTIONAL.
        *
        * @param streamSourceUrl a string specifying the stream's source URL.
        */
        public abstract void setStreamSourceUrl(String streamSourceUrl);

        /**
        * Get a string specifing the title of the stream. Container support for this field is OPTIONAL.
        *
        * @return a string specifing the title of the stream.
        */
        public abstract String getStreamTitle();

        /**
        * Set a string specifing the title of the stream. Container support for this field is OPTIONAL.
        *
        * @param streamTitle a string specifing the title of the stream.
        */
        public abstract void setStreamTitle(String streamTitle);

        /**
        * Get a string specifying the stream's URL. Container support for this field is OPTIONAL.
        *
        * @return a string specifying the stream's URL.
        */
        public abstract String getStreamUrl();

        /**
        * Set a string specifying the stream's URL. Container support for this field is OPTIONAL.
        *
        * @param streamUrl a string specifying the stream's URL.
        */
        public abstract void setStreamUrl(String streamUrl);

        /**
        * Get a map of custom key/value pairs associated with this activity. Container support for this
        * field is OPTIONAL.
        *
        * @return a map of custom key/value pairs associated with this activity.
        */
        public abstract Dictionary<String, String> getTemplateParams();

        /**
        * Set a map of custom key/value pairs associated with this activity. The data has type
        * {@link Map<String, Object>}. The object may be either a String or an {@link Person}. When
        * passing in a person with key PersonKey, can use the following replacement variables in the
        * template:
        * <ul>
        * <li>PersonKey.DisplayName - Display name for the person</li>
        * <li>PersonKey.ProfileUrl. URL of the person's profile</li>
        * <li>PersonKey.Id - The ID of the person</li>
        * <li>PersonKey - Container may replace with DisplayName, but may also optionally link to the
        * user.</li>
        * </ul>
        * Container support for this field is OPTIONAL.
        *
        * @param templateParams a map of custom key/value pairs associated with this activity.
        */
        public abstract void setTemplateParams(Dictionary<String, String> templateParams);

        /**
        * Get a string specifying the primary text of an activity. Container support for this field is
        * REQUIRED.
        *
        * Titles may only have the following HTML tags: &lt;b&gt; &lt;i&gt;, &lt;a&gt;, &lt;span&gt;. The
        * container may ignore this formatting when rendering the activity.
        *
        * @return astring specifying the primary text of an activity.
        */
        public abstract String getTitle();

        /**
        * Set a string specifying the primary text of an activity. Container support for this field is
        * REQUIRED.
        *
        * Titles may only have the following HTML tags: &lt;b&gt; &lt;i&gt;, &lt;a&gt;, &lt;span&gt;. The
        * container may ignore this formatting when rendering the activity.
        *
        * @param title a string specifying the primary text of an activity.
        */
        public abstract void setTitle(String title);

        /**
        * Get a string specifying the title template message ID in the gadget spec. Container support for
        * this field is REQUIRED.
        *
        * The title is the primary text of an activity. Titles may only have the following HTML tags:
        * <&lt;b&gt; &lt;i&gt;, &lt;a&gt;, &lt;span&gt;. The container may ignore this formatting when
        * rendering the activity.
        *
        * @return a string specifying the title template message ID in the gadget spec.
        */
        public abstract String getTitleId();

        /**
        * Set a string specifying the title template message ID in the gadget spec. Container support for
        * this field is REQUIRED.
        *
        * The title is the primary text of an activity. Titles may only have the following HTML tags:
        * <&lt;b&gt; &lt;i&gt;, &lt;a&gt;, &lt;span&gt;. The container may ignore this formatting when
        * rendering the activity.
        *
        * @param titleId a string specifying the title template message ID in the gadget spec.
        */
        public abstract void setTitleId(String titleId);

        /**
        * Get a string specifying the URL that represents this activity. Container support for this field
        * is OPTIONAL.
        *
        * @return a string specifying the URL that represents this activity.
        */
        public abstract String getUrl();

        /**
        * Set a string specifying the URL that represents this activity. Container support for this field
        * is OPTIONAL.
        *
        * @param url a string specifying the URL that represents this activity.
        */
        public abstract void setUrl(String url);

        /**
        * Get a string ID of the user who this activity is for. Container support for this field is
        * OPTIONAL.
        *
        * @return a string ID of the user who this activity is for.
        */
        public abstract String getUserId();

        /**
        * Get a string ID of the user who this activity is for. Container support for this field is
        * OPTIONAL.
        *
        * @param userId a string ID of the user who this activity is for.
        */
        public abstract void setUserId(String userId);
    } 
}
