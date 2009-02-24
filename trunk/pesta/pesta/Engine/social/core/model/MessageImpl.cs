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
using System.Text.RegularExpressions;
using Pesta.Engine.social.model;

namespace Pesta.Engine.social.core.model
{
    /// <summary>
    /// Summary description for MessageImpl
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class MessageImpl : Message
    {
        private String body;
        private String title;
        private Type type;
        private String id;
        private List<Person> recipients;

        public MessageImpl()
        {
        }

        public MessageImpl(String initBody, String initTitle, Type initType)
        {
            body = initBody;
            title = initTitle;
            type = initType;
        }

        public override String getBody()
        {
            return body;
        }

        public override void setBody(String newBody)
        {
            body = newBody;
        }

        public override String getId()
        {
            return id;
        }
        public override void setId(String newId)
        {
            id = newId;
        }

        public override String getTitle()
        {
            return title;
        }

        public override void setTitle(String newTitle)
        {
            title = newTitle;
        }

        public override Type getType()
        {
            return type;
        }

        public override void setType(Type newType)
        {
            type = newType;
        }

        public List<Person> getRecipients()
        {
            return recipients;
        }

        public void setRecipients(List<Person> newRecipients)
        {
            recipients = newRecipients;
        }

        public override String sanitizeHTML(String htmlStr)
        {
            return Regex.Replace(htmlStr, @"<(.|\n)*?>", string.Empty);
        }
    }
}