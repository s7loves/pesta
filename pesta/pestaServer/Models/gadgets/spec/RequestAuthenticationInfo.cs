﻿#region License, Terms and Conditions
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
using Uri=Pesta.Engine.common.uri.Uri;

namespace pestaServer.Models.gadgets.spec
{
    /// <summary>
    /// Summary description for RequestAuthenticationInfo
    /// </summary>
    /// <remarks>
    /// <para>
    
    /// </para>
    /// </remarks>
    public interface RequestAuthenticationInfo
    {
        /**
       * @return The type of authentication to use.
       */
        AuthType getAuthType();

        /**
         * @return The destination URI for making authenticated requests to.
         */
        Uri getHref();

        /**
         * @return True if owner signing is needed.
         */
        bool isSignOwner();


        /**
         * @return True if viewer signing is needed.
         */
        bool isSignViewer();

        /**
         * @return A map of all relevant auth-related attributes.
         */
        Dictionary<String, String> getAttributes();
    }
}