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
using Pesta.Engine.social.model;

namespace Pesta.Engine.social.core.model
{
    /// <summary>
    /// Summary description for ActivityImpl
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class ActivityImpl : Activity
    {
        private String appId;
        private String body;
        private String bodyId;
        private String externalId;
        private String id;
        private DateTime? updated;
        private List<MediaItem> mediaItems;
        private long? postedTime;
        private float? priority;
        private String streamFaviconUrl;
        private String streamSourceUrl;
        private String streamTitle;
        private String streamUrl;
        private Dictionary<String, String> templateParams;
        private String title;
        private String titleId;
        private String url;
        private String userId;

        public ActivityImpl()
        {
        }

        public ActivityImpl(String id, String userId)
        {
            this.id = id;
            this.userId = userId;
        }

        public override String getAppId()
        {
            return appId;
        }

        public override void setAppId(String appId)
        {
            this.appId = appId;
        }

        public override String getBody()
        {
            return body;
        }

        public override void setBody(String body)
        {
            this.body = body;
        }

        public override String getBodyId()
        {
            return bodyId;
        }

        public override void setBodyId(String bodyId)
        {
            this.bodyId = bodyId;
        }

        public override String getExternalId()
        {
            return externalId;
        }

        public override void setExternalId(String externalId)
        {
            this.externalId = externalId;
        }

        public override String getId()
        {
            return id;
        }

        public override void setId(String id)
        {
            this.id = id;
        }

        public override DateTime? getUpdated()
        {
            if (updated == null)
            {
                return null;
            }
            return updated;
        }

        public override void setUpdated(DateTime? updated)
        {
            if (updated == null)
            {
                this.updated = null;
            }
            else
            {
                this.updated = updated;
            }
        }

        public override List<MediaItem> getMediaItems()
        {
            return mediaItems;
        }

        public override void setMediaItems(List<MediaItem> mediaItems)
        {
            this.mediaItems = mediaItems;
        }

        public override long? getPostedTime()
        {
            return postedTime;
        }

        public override void setPostedTime(long? postedTime)
        {
            this.postedTime = postedTime;
        }

        public override float? getPriority()
        {
            return priority;
        }

        public override void setPriority(float? priority)
        {
            this.priority = priority;
        }

        public override String getStreamFaviconUrl()
        {
            return streamFaviconUrl;
        }

        public override void setStreamFaviconUrl(String streamFaviconUrl)
        {
            this.streamFaviconUrl = streamFaviconUrl;
        }

        public override String getStreamSourceUrl()
        {
            return streamSourceUrl;
        }

        public override void setStreamSourceUrl(String streamSourceUrl)
        {
            this.streamSourceUrl = streamSourceUrl;
        }

        public override String getStreamTitle()
        {
            return streamTitle;
        }

        public override void setStreamTitle(String streamTitle)
        {
            this.streamTitle = streamTitle;
        }

        public override String getStreamUrl()
        {
            return streamUrl;
        }

        public override void setStreamUrl(String streamUrl)
        {
            this.streamUrl = streamUrl;
        }

        public override Dictionary<String, String> getTemplateParams()
        {
            return templateParams;
        }

        public override void setTemplateParams(Dictionary<String, String> templateParams)
        {
            this.templateParams = templateParams;
        }

        public override String getTitle()
        {
            return title;
        }

        public override void setTitle(String title)
        {
            this.title = title;
        }

        public override String getTitleId()
        {
            return titleId;
        }

        public override void setTitleId(String titleId)
        {
            this.titleId = titleId;
        }

        public override String getUrl()
        {
            return url;
        }

        public override void setUrl(String url)
        {
            this.url = url;
        }

        public override String getUserId()
        {
            return userId;
        }

        public override void setUserId(String userId)
        {
            this.userId = userId;
        }
    }
}