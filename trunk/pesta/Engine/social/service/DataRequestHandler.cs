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


namespace Pesta
{
    /// <summary>
    /// Summary description for DataRequestHandler
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public abstract class DataRequestHandler
    {
        private static readonly List<String> GET_SYNONYMS = new List<string>(new string[] { "get" });
        private static readonly List<String> CREATE_SYNONYMS = new List<string>(new string[] { "put", "create" });
        private static readonly List<String> UPDATE_SYNONYMS = new List<string>(new string[] { "post", "update" });
        private static readonly List<String> DELETE_SYNONYMS = new List<string>(new string[] { "delete" });

        protected abstract object handleDelete(RequestItem request);
        protected abstract object handlePut(RequestItem request);
        protected abstract object handlePost(RequestItem request);
        protected abstract object handleGet(RequestItem request);

        public delegate object handleItemDelegate(RequestItem request);

        public IAsyncResult handleItem(RequestItem request)
        {
            if (request.getOperation() == null)
            {
                throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED,
                            "Unserviced operation");
            }
            String operation = request.getOperation().ToLower();
            handleItemDelegate handler;
            if (GET_SYNONYMS.Contains(operation))
            {
                handler = new handleItemDelegate(handleGet);
            }
            else if (UPDATE_SYNONYMS.Contains(operation))
            {
                handler = new handleItemDelegate(handlePost);
            }
            else if (CREATE_SYNONYMS.Contains(operation))
            {
                handler = new handleItemDelegate(handlePut);
            }
            else if (DELETE_SYNONYMS.Contains(operation))
            {
                handler = new handleItemDelegate(handleDelete);
            }
            else
            {
                throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED,
                    "Unserviced operation " + operation);
            }
            return handler.BeginInvoke(request, null, null);
        }

        /**
        * Utility class for common API call preconditions
        */
        public abstract class Preconditions<T>
        {
            public static void requireNotEmpty(IList<T> coll, String message)
            {
                if (coll.Count == 0)
                {
                    throw new SocialSpiException(ResponseError.BAD_REQUEST, message);
                }
            }

            public static void requireEmpty(IList<T> list, String message)
            {
                if (list.Count != 0)
                {
                    throw new SocialSpiException(ResponseError.BAD_REQUEST, message);
                }
            }

            public static void requireSingular(IList<T> coll, String message)
            {
                if (coll.Count != 1)
                {
                    throw new SocialSpiException(ResponseError.BAD_REQUEST, message);
                }
            }

            public static void requirePlural(IList<T> coll, String message)
            {
                if (coll.Count <= 1)
                {
                    throw new SocialSpiException(ResponseError.BAD_REQUEST, message);
                }
            }
        }
    } 
}
