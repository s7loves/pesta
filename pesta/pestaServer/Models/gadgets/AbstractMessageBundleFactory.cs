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

using Pesta.Utilities;
using pestaServer.Models.gadgets.spec;
using Uri=Pesta.Engine.common.uri.Uri;


namespace pestaServer.Models.gadgets
{
    /// <summary>
    /// Summary description for AbstractMessageBundleFactory
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public abstract class AbstractMessageBundleFactory : MessageBundleFactory
    {

        public MessageBundle getBundle(GadgetSpec spec, Locale locale, bool ignoreCache)
        {
            MessageBundle parent = getParentBundle(spec, locale, ignoreCache);
            MessageBundle child = null;
            LocaleSpec localeSpec = spec.getModulePrefs().getLocale(locale);
            if (localeSpec == null)
            {
                return parent ?? MessageBundle.EMPTY;
            }
            Uri messages = localeSpec.getMessages();
            if (messages == null || messages.ToString().Length == 0)
            {
                child = localeSpec.getMessageBundle();
            }
            else
            {
                child = fetchBundle(localeSpec, ignoreCache);
            }
            return new MessageBundle(parent, child);
        }

        private MessageBundle getParentBundle(GadgetSpec spec, Locale locale, bool ignoreCache)
        {
            if (locale.getLanguage().Equals("all"))
            {
                // Top most locale already.
                return null;
            }

            if (locale.getCountry().Equals("ALL"))
            {
                return getBundle(spec, new Locale("all", "ALL"), ignoreCache);
            }

            return getBundle(spec, new Locale(locale.getLanguage(), "ALL"), ignoreCache);
        }

        /**
     * Retrieve the MessageBundle for the given LocaleSpec from the network or cache.
     */
        protected abstract MessageBundle fetchBundle(LocaleSpec locale, bool ignoreCache);


    }
}