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
using Pesta.Engine.protocol;
using Pesta.Engine.social;
using Pesta.Engine.social.spi;

namespace Pesta.Utilities
{
    /// <summary>
    /// Utility class for common API call preconditions
    /// </summary>
    public class Preconditions
    {
        public static void checkArgument(bool expression, string errorMessage)
        {
            if (!expression)
            {
                throw new ArgumentException(errorMessage);
            }
        }

        public static void checkArgument(bool expression)
        {
            if (!expression)
            {
                throw new ArgumentException();
            }
        }

        public static T checkNotNull<T>(T reference)
        {
            if (reference == null)
            {
                throw new NullReferenceException();
            }
            return reference;
        }

        public static T checkNotNull<T>(T reference, Object errorMessage)
        {
            if (reference == null)
            {
                throw new NullReferenceException(errorMessage.ToString());
            }
            return reference;
        }

        public static void requireNotEmpty<T>(ICollection<T> coll, String message)
        {
            if (coll.Count == 0)
            {
                throw new ProtocolException(ResponseError.BAD_REQUEST, message);
            }
        }

        public static void requireEmpty<T>(ICollection<T> list, String message)
        {
            if (list.Count != 0)
            {
                throw new ProtocolException(ResponseError.BAD_REQUEST, message);
            }
        }

        public static void requireSingular<T>(ICollection<T> coll, String message)
        {
            if (coll.Count != 1)
            {
                throw new ProtocolException(ResponseError.BAD_REQUEST, message);
            }
        }

        public static void requirePlural<T>(ICollection<T> coll, String message)
        {
            if (coll.Count <= 1)
            {
                throw new ProtocolException(ResponseError.BAD_REQUEST, message);
            }
        }
    }
}
