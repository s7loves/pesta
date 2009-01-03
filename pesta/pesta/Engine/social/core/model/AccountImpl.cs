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
    /// Summary description for AccountImpl
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class AccountImpl : Account
    {
        String domain;
        String userId;
        String username;

        public AccountImpl() { }

        public AccountImpl(String domain, String userId, String username)
        {
            this.domain = domain;
            this.userId = userId;
            this.username = username;
        }

        public override String getDomain()
        {
            return domain;
        }

        public override void setDomain(String domain)
        {
            this.domain = domain;
        }

        public override String getUserId()
        {
            return userId;
        }

        public override void setUserId(String userId)
        {
            this.userId = userId;
        }

        public override String getUsername()
        {
            return username;
        }

        public override void setUsername(String username)
        {
            this.username = username;
        }
    }
}