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

namespace pestaServer.Models.gadgets.servlet
{
    /// <summary>
    /// Contains RPC-specific exceptions.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    [Serializable]
    public class RpcException : Exception
    {
        private readonly GadgetContext context;

        public GadgetContext Context
        {
            get
            {
                return context;
            }
        }

        public RpcException()
            : this(null)
        {
            context = null;
        }
        public RpcException(String message)
            : base(message)
        {
            context = null;
        }

        public RpcException(String message, Exception cause)
            : base(message, cause)
        {
            this.context = null;
        }

        public RpcException(GadgetContext context, Exception cause)
            : base(cause.Message, cause)
        {
            this.context = context;
        }

        public RpcException(GadgetContext context, String message)
            : base(message)
        {
            this.context = context;
        }

        public RpcException(GadgetContext context, String message, Exception cause)
            : base(message, cause)
        {
            this.context = context;
        }
    }
}