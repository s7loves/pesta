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
using Pesta.Engine.auth;
using Pesta.Utilities;

namespace Pesta.Engine.social.spi
{
    /// <summary>
    /// Summary description for UserId
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class UserId
    {
        public class Type : EnumBaseType<Type>
        {
            /// <summary>
            /// Initializes a new instance of the Type class.
            /// </summary>
            public Type(int key, string value)
                : base(key, value)
            {
            }
            public readonly static Type me = new Type(1, "me");
            public readonly static Type viewer = new Type(2, "viewer");
            public readonly static Type owner = new Type(3, "owner");
            public readonly static Type userId = new Type(4, "userId");

            /** A map of JSON strings to Type objects */
            private static readonly Dictionary<String, Type> jsonTypeMap = new Dictionary<string, Type>()
                                                                               {
                                                                                   {"@me", Type.me},
                                                                                   {"@viewer", Type.viewer},
                                                                                   {"@owner", Type.owner},
                                                                                   {"@userId", Type.userId}
                                                                               };
            /** Return the Type enum value given a specific jsonType **/
            public static Type jsonValueOf(String jsonType)
            {
                Type retType = null;
                jsonTypeMap.TryGetValue(jsonType, out retType);
                return retType;
            }
        }

        private Type type;
        private String userId;

        public UserId(Type type, String userId)
        {
            this.type = type;
            this.userId = userId;
        }

        public Type getType()
        {
            return type;
        }

        public String getUserId()
        {
            return userId;
        }

        public String getUserId(ISecurityToken token)
        {
            if (type == Type.owner)
            {
                return token.getOwnerId();
            }
            else if (type == Type.viewer || type == Type.me)
            {
                return token.getViewerId();
            }
            else if (type == Type.userId)
            {
                return userId;
            }

            throw new Exception("The type field is not a valid enum: " + type);
        }

        public static UserId fromJson(String jsonId)
        {
            Type idSpecEnum = Type.jsonValueOf(jsonId);
            if (idSpecEnum != null)
            {
                return new UserId(idSpecEnum, null);
            }

            return new UserId(Type.userId, jsonId);
        }

        // These are overriden so that EasyMock doesn't throw a fit
        public override bool Equals(Object o)
        {
            if (!(o is UserId))
            {
                return false;
            }

            UserId actual = (UserId)o;
            return this.type == actual.type
                   && this.userId.Equals(actual.userId);
        }

        public override int GetHashCode()
        {
            int userHashCode = this.userId == null ? 0 : this.userId.GetHashCode();
            return this.type.GetHashCode() + userHashCode;
        }
    }
}