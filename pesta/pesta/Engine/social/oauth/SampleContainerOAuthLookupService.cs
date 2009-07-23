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
using System.IO;
using Pesta.Engine.auth;
using Pesta.Engine.social.core.oauth;
using Pesta.Interop.oauth;
using Pesta.Libraries.oauth;

namespace Pesta.Engine.social.oauth
{
    /// <summary>
    /// Summary description for SampleContainerOAuthLookupService
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class SampleContainerOAuthLookupService : IOAuthLookupService
    {
        // If we were a real social network this would probably be a function
        private static readonly Dictionary<String, String> sampleContainerUrlToAppIdMap = new Dictionary<string, string>
                                                                                     {
                                                                                         {"http://localhost/gadgets/files/samplecontainer/examples/SocialHelloWorld.xml","7810"},
                                                                                         {"http://localhost/gadgets/files/samplecontainer/examples/SocialActivitiesWorld.xml","8355"}
                                                                                     };

        // If we were a real social network we would probably be keeping track of this in a db somewhere
        private static readonly Dictionary<String, List<String>> sampleContainerAppInstalls = new Dictionary<string, List<string>>
                                                                                                  {
                                                                                             {"john.doe", new List<String> {"7810", "8355"}}
                                                                                         };

        // If we were a real social network we would establish shared secrets with each of our gadgets
        private static readonly Dictionary<String, String> sampleContainerSharedSecrets = new Dictionary<string, string>
                                                                                     {
                                                                                         {"7810", "SocialHelloWorldSharedSecret"},
                                                                                         {"8355", "SocialActivitiesWorldSharedSecret"}
                                                                                     };

        public bool thirdPartyHasAccessToUser(OAuthMessage message, String appUrl, String userId)
        {
            String appId = getAppId(appUrl);
            return hasValidSignature(message, appUrl, appId)
                   && userHasAppInstalled(userId, appId);
        }

        private static bool hasValidSignature(OAuthMessage message, String appUrl, String appId)
        {
            String sharedSecret = sampleContainerSharedSecrets[appId];
            if (sharedSecret == null)
            {
                return false;
            }

            OAuthServiceProvider provider = new OAuthServiceProvider(null, null, null);
            OAuthConsumer consumer = new OAuthConsumer(null, appUrl, sharedSecret, provider);
            OAuthAccessor accessor = new OAuthAccessor(consumer);

            SimpleOAuthValidator validator = new SimpleOAuthValidator();
            try
            {
                validator.validateMessage(message, accessor);
            }
            catch (OAuthException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            catch (UriFormatException)
            {
                return false;
            }

            return true;
        }

        private bool userHasAppInstalled(String userId, String appId)
        {
            List<String> appInstalls = sampleContainerAppInstalls[userId];
            if (appInstalls != null)
            {
                foreach (String appInstall in appInstalls)
                {
                    if (appInstall.Equals(appId))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public ISecurityToken getSecurityToken(String appUrl, String userId)
        {
            String domain = "samplecontainer.com";
            String container = "default";
            return new OAuthSecurityToken(userId, appUrl, getAppId(appUrl), domain, container);
        }

        private String getAppId(String appUrl)
        {
            return sampleContainerUrlToAppIdMap[appUrl];
        }
    }
}