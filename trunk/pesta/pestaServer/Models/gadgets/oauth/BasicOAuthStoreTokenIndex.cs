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

namespace pestaServer.Models.gadgets.oauth
{
    /// <summary>
    /// Summary description for BasicOAuthStoreTokenIndex
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class BasicOAuthStoreTokenIndex
    {
        private String userId;
        private String gadgetUri;
        private long moduleId;
        private String tokenName;
        private String serviceName;

        public String getUserId()
        {
            return userId;
        }
        public void setUserId(String userid)
        {
            userId = userid;
        }
        public String getGadgetUri()
        {
            return gadgetUri;
        }
        public void setGadgetUri(String uri)
        {
            gadgetUri = uri;
        }
        public long getModuleId()
        {
            return moduleId;
        }
        public void setModuleId(long id)
        {
            moduleId = id;
        }
        public String getTokenName()
        {
            return tokenName;
        }
        public void setTokenName(String name)
        {
            tokenName = name;
        }
        public String getServiceName()
        {
            return serviceName;
        }
        public void setServiceName(String name)
        {
            serviceName = name;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result =
                prime * result + ((gadgetUri == null) ? 0 : gadgetUri.GetHashCode());
            result = prime * result + (int)(moduleId ^ (moduleId >> 32));
            result =
                prime * result + ((serviceName == null) ? 0 : serviceName.GetHashCode());
            result =
                prime * result + ((tokenName == null) ? 0 : tokenName.GetHashCode());
            result = prime * result + ((userId == null) ? 0 : userId.GetHashCode());
            return result;
        }

        public override bool Equals(Object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            BasicOAuthStoreTokenIndex other = (BasicOAuthStoreTokenIndex)obj;
            if (gadgetUri == null)
            {
                if (other.gadgetUri != null)
                    return false;
            }
            else if (!gadgetUri.Equals(other.gadgetUri))
                return false;
            if (moduleId != other.moduleId)
                return false;
            if (serviceName == null)
            {
                if (other.serviceName != null)
                    return false;
            }
            else if (!serviceName.Equals(other.serviceName))
                return false;
            if (tokenName == null)
            {
                if (other.tokenName != null)
                    return false;
            }
            else if (!tokenName.Equals(other.tokenName))
                return false;
            if (userId == null)
            {
                if (other.userId != null)
                    return false;
            }
            else if (!userId.Equals(other.userId))
                return false;
            return true;
        }
    }
}