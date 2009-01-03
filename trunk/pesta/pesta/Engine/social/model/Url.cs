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
using Pesta.Interop;

namespace Pesta.Engine.social.model
{
    /// <summary>
    /// Summary description for Url
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    [ImplementedBy(typeof(UrlImpl))]
    public interface Url : ListField
    {
        /**
        * Get the text associated with the link.
        * @return the link text
        */
        String getLinkText();

        /**
        * Set the Link text associated with the link.
        * @param linkText the link text
        */
        void setLinkText(String linkText);
    } 
}
