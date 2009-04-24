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
using Pesta.Engine.social.model;

namespace Pesta.Engine.social.core.model
{
    /// <summary>
    /// Summary description for MediaItemImpl
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class MediaItemImpl : MediaItem
    {
        private String _mimeType;
        private Type _type;
        private String _url;

        public MediaItemImpl()
        {
        }

        public MediaItemImpl(String mimeType, Type type, String url)
        {
            this._mimeType = mimeType;
            this._type = type;
            this._url = url;
        }

        public override String getMimeType()
        {
            return _mimeType;
        }

        public override void setMimeType(String mimeType)
        {
            this._mimeType = mimeType;
        }

        public override Type getType()
        {
            return _type;
        }

        public override void setType(Type type)
        {
            this._type = type;
        }

        public override String getUrl()
        {
            return _url;
        }

        public override void setUrl(String url)
        {
            this._url = url;
        }
    }
}