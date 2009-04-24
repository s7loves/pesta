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
    /// Summary description for Message
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    [ImplementedBy(typeof(MessageImpl))]
    public abstract class Message
    {
        /**
       * An enumeration of field names in a message.
       */
        public class Field : EnumBaseType<Field>
        {
            /// <summary>
            /// Initializes a new instance of the Field class.
            /// </summary>
            public Field(int key, string value)
                : base(key, value)
            {

            }
            public static readonly Field BODY = new Field(1, "body");
            public static readonly Field TITLE = new Field(2, "title");
            public static readonly Field TYPE = new Field(3, "type");
            public static readonly Field ID = new Field(4, "id");

            /**
            * the name of the field.
            */
            private readonly String jsonString;

            /**
            * Create a field based on a name.
            * @param jsonString the name of the field
            */
            protected Field(String jsonString)
            {
                this.jsonString = jsonString;
            }


            public override String ToString()
            {
                return jsonString;
            }
        }

        /**
         * The type of a message
         */
        public class Type : EnumBaseType<Type>
        {
            public Type(int key, string value)
                : base(key, value)
            {

            }
            public static readonly Type EMAIL = new Type(1, "EMAIL");
            public static readonly Type NOTIFICATION = new Type(2, "NOTIFICATION");
            public static readonly Type PRIVATE_MESSAGE = new Type(3, "PRIVATE_MESSAGE");
            public static readonly Type PUBLIC_MESSAGE = new Type(4, "PUBLIC_MESSAGE");

            /**
            * The type of message.
            */
            private readonly String jsonString;

            /**
            * Create a message type based on a string token.
            * @param jsonString the type of message
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
        * Gets the main text of the message.
        * @return the main text of the message
        */
        public abstract String getBody();

        /**
        * Sets the main text of the message.
        * HTML attributes are allowed and are sanitized by the container
        * @param newBody the main text of the message
        */
        public abstract void setBody(String newBody);


        public abstract String getId();
        public abstract void setId(String newId);

        /**
        * Gets the title of the message.
        * @return the title of the message
        */
        public abstract String getTitle();

        /**
        * Sets the title of the message.
        * HTML attributes are allowed and are sanitized by the container.
        * @param newTitle the title of the message
        */
        public abstract void setTitle(String newTitle);

        /**
        * Gets the type of the message, as specified by opensocial.Message.Type.
        * @return the type of message (enum Message.Type)
        */
        public abstract Type getType();

        /**
        * Sets the type of the message, as specified by opensocial.Message.Type
        * @param newType the type of message (enum Message.Type)
        */
        public abstract void setType(Type newType);

        /**
        * TODO implement either a standard 'sanitizing' facility or
        * define an interface that can be set on this class so
        * others can plug in their own.
        * @param htmlStr String to be sanitized.
        * @return the sanitized HTML String
        */
        public abstract String sanitizeHTML(String htmlStr);
    } 
}
