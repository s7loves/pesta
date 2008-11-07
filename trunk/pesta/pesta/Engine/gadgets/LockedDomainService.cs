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

using System.Text;

namespace Pesta
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public interface LockedDomainService
    {
        /**
        * Is locked domain enabled
        * @return true is locked domain is enabled
        */
        bool isEnabled();

        /**
        * Check whether embedded content (img src, for example) can render on
        * a particular host.
        * 
        * @param host host name for rendered content
        * @return true if the content should be allowed to render
        */
        bool embedCanRender(String host);

        /**
        * Figure out where embedded content should render.
        * 
        * @return host name for safe rendering of embedded content.
        */
        String getEmbedHost();

        /**
        * Calculate the locked domain for a particular gadget on a particular
        * container.
        * 
        * @param gadget URL of the gadget
        * @param container name of the container page
        * @return the host name on which the gadget should render
        */
        String getLockedDomainForGadget(String gadget, String container);

        /**
        * Check whether a gadget should be allowed to render on a particular
        * host.
        * 
        * @param host host name for the content
        * @param gadget URL of the gadget
        * @param container container
        * @return true if the gadget can render
        */
        bool gadgetCanRender(String host, Gadget gadget, String container);
    } 
}
