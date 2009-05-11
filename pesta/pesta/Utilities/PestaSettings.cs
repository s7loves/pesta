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
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;

namespace Pesta.Utilities
{
    public sealed class PestaSettings : IConfigurationSectionHandler
    {
        #region static methods

        static PestaSettings() 
        {
            Object obj = ConfigurationManager.GetSection("Pesta");
        }
        #endregion

        #region public methods

        public Object Create(Object parent, object configContext, XmlNode section) 
        {
            NameValueCollection settings;

            try 
            {
                NameValueSectionHandler baseHandler = new NameValueSectionHandler();
                settings = (NameValueCollection) baseHandler.Create(parent, configContext, section);
            } 
            catch
            {
                settings = null;
            }

            if (settings != null) 
            {
                dbServiceName = ReadSetting(settings, "dbServiceName", "JsonDbOpensocialService");
                allowUnauthenticated = ReadSetting(settings, "allowUnauthenticated", "true");
                gadgetDebug = ReadSetting(settings, "gadgetDebug", "true");
                tokenMaxAge = ReadSetting(settings, "tokenMaxAge", "3600");
                gadgetCacheXmlRefreshInterval = ReadSetting(settings, "gadgetCacheXmlRefreshInterval", "300000");
                allowPlaintextToken = ReadSetting(settings, "allowPlaintextToken", "true");
                containerUrlPrefix = ReadSetting(settings, "containerUrlPrefix", "");
                tokenMasterKey = ReadSetting(settings, "tokenMasterKey", "INSECURE_DEFAULT_KEY");
                javaPath = ReadSetting(settings, "javaPath", @"c:\program files\java\jdk1.6.0_12\bin\java.exe");
            }

            return null;
        }

        #endregion

        internal static String ReadSetting(NameValueCollection settings, String key, String defaultValue)
        {
            if (settings == null || key == null)
                throw new ArgumentNullException();

            try
            {
                Object setting = settings[key];
                return (setting == null) ? defaultValue : (String)setting;
            }
            catch
            {
                return defaultValue;
            }
        }

        private static string dbServiceName;
        private static string allowUnauthenticated;
        private static string gadgetDebug;
        private static string gadgetCacheXmlRefreshInterval;
        private static string allowPlaintextToken;
        private static string tokenMaxAge;
        private static string containerUrlPrefix;
        private static string tokenMasterKey;
        private static string javaPath;

        public static String DbServiceName 
        {
            get { return dbServiceName; }
        }
        public static String AllowPlaintextToken
        {
            get { return allowPlaintextToken; }
        }
        public static String TokenMaxAge
        {
            get { return tokenMaxAge; }
        }
        public static String AllowUnauthenticated
        {
            get { return allowUnauthenticated; }
        }
        public static String GadgetDebug
        {
            get { return gadgetDebug; }
        }
        public static String GadgetCacheXmlRefreshInterval
        {
            get { return gadgetCacheXmlRefreshInterval; }
        }

        public static String ContainerUrlPrefix
        {
            get { return containerUrlPrefix; }
        }

        public static String TokenMasterKey
        {
            get { return tokenMasterKey; }
        }

        public static String JavaPath
        {
            get { return javaPath; }
        }
        
    }
}