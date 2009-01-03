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
    /// Summary description for NameImpl
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class NameImpl : Name
    {
        private String additionalName;
        private String familyName;
        private String givenName;
        private String honorificPrefix;
        private String honorificSuffix;
        private String formatted;

        public NameImpl()
        {
        }

        public NameImpl(String formatted)
        {
            this.formatted = formatted;
        }

        override public String getFormatted()
        {
            return formatted;
        }

        override public void setFormatted(String formatted)
        {
            this.formatted = formatted;
        }

        override public String getAdditionalName()
        {
            return additionalName;
        }

        override public void setAdditionalName(String additionalName)
        {
            this.additionalName = additionalName;
        }

        override public String getFamilyName()
        {
            return familyName;
        }

        override public void setFamilyName(String familyName)
        {
            this.familyName = familyName;
        }

        override public String getGivenName()
        {
            return givenName;
        }

        override public void setGivenName(String givenName)
        {
            this.givenName = givenName;
        }

        override public String getHonorificPrefix()
        {
            return honorificPrefix;
        }

        override public void setHonorificPrefix(String honorificPrefix)
        {
            this.honorificPrefix = honorificPrefix;
        }

        override public String getHonorificSuffix()
        {
            return honorificSuffix;
        }

        override public void setHonorificSuffix(String honorificSuffix)
        {
            this.honorificSuffix = honorificSuffix;
        }
    }
}