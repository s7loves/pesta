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
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Text;
using System.Security.Cryptography;
using org.apache.commons.codec.digest;
using org.apache.shindig.common.util;

namespace Pesta
{
    /// <summary>
    /// Locked domain implementation based on sha1.
    /// The generated domain takes the form:
    /// base32(sha1(gadget url)).
    /// Other domain locking schemes are possible as well.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class HashLockedDomainService : LockedDomainService
    {
        private ContainerConfig config = ContainerConfig.Instance;
        private String embedHost;
        private bool enabled;
        private java.util.Collection suffixes;
        public static String LOCKED_DOMAIN_REQUIRED_KEY = "gadgets.lockedDomainRequired";
        public static String LOCKED_DOMAIN_SUFFIX_KEY = "gadgets.lockedDomainSuffix";
        private GadgetReader gadgetReader = new GadgetReader();

        public static readonly HashLockedDomainService Instance = new HashLockedDomainService("127.0.0.1:9090", false);
        /**
        * Create a LockedDomainService
        * @param config per-container configuration
        * @param embedHost host name to use for embedded content
        * @param enabled whether this service should do anything at all.
        */
        protected HashLockedDomainService(String embedHost, bool enabled)
        {
            this.embedHost = embedHost;
            this.enabled = enabled;
            suffixes = new java.util.HashSet();
            java.util.Collection containers = config.getContainers();
            if (enabled)
            {
                for (java.util.Iterator iter = containers.iterator(); iter.hasNext(); )
                {
                    String container = iter.next() as String;
                    String suffix = config.get(container, LOCKED_DOMAIN_SUFFIX_KEY);
                    suffixes.add(suffix);
                }
            }
        }

        public bool isEnabled()
        {
            return enabled;
        }

        public String getEmbedHost()
        {
            return embedHost;
        }

        public bool embedCanRender(String host)
        {
            return (!enabled || host.EndsWith(embedHost));
        }

        public bool gadgetCanRender(String host, Gadget gadget, String container)
        {
            if (!enabled)
            {
                return true;
            }
            // Gadgets can opt-in to locked domains, or they can be enabled globally
            // for a particular container
            if (gadgetReader.gadgetWantsLockedDomain(gadget) || containerWantsLockedDomain(container))
            {
                String neededHost = getLockedDomainForGadget(gadgetReader.getGadgetUrl(gadget), container);
                return (neededHost.Equals(host));
            }
            // Make sure gadgets that don't ask for locked domain aren't allowed
            // to render on one.
            return !gadgetUsingLockedDomain(host, gadget);
        }

        public String getLockedDomainForGadget(String gadget, String container)
        {
            String suffix = config.get(container, LOCKED_DOMAIN_SUFFIX_KEY);
            if (suffix == null)
            {
                throw new Exception(
                    "Cannot redirect to locked domain if it is not configured");
            }

            byte[] sha1 = DigestUtils.sha(gadget);
            String hash = Encoding.Unicode.GetString(Base32.encodeBase32(sha1));
            return hash + suffix;
        }

        private bool containerWantsLockedDomain(String container)
        {
            String required = config.get(container, LOCKED_DOMAIN_REQUIRED_KEY);
            return ("true".Equals(required));
        }

        private bool gadgetUsingLockedDomain(String host, Gadget gadget)
        {
            for (java.util.Iterator iter = suffixes.iterator(); iter.hasNext(); )
            {
                string suffix = iter.next() as string;
                if (host.EndsWith(suffix))
                {
                    return true;
                }
            }
            return false;
        }

        class GadgetReader
        {
            public bool gadgetWantsLockedDomain(Gadget gadget)
            {
                Dictionary<string, Feature> prefs = gadget.Spec.getModulePrefs().getFeatures();
                return prefs.ContainsKey("locked-domain");
            }

            public String getGadgetUrl(Gadget gadget)
            {
                return gadget.Context.getUrl().toString();
            }
        }
    } 
}

