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
using Locale = java.util.Locale;
using URI = System.Uri;

namespace Pesta
{
    /// <summary>
    /// Summary description for GadgetContext
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class GadgetContext
    {
        /**
       * @param name The parameter to get data for.
       * @return The parameter set under the given name, or null.
       */
        public virtual String getParameter(String name)
        {
            return null;
        }

        /**
         * @return The url for this gadget.
         */
        public virtual URI getUrl()
        {
            return null;
        }

        /**
         * @return The module id for this request.
         */
        public virtual int getModuleId()
        {
            return 0;
        }

        /**
         * @return The locale for this request.
         */
        public virtual Locale getLocale()
        {
            return GadgetSpec.DEFAULT_LOCALE;
        }

        /**
         * @return The rendering context for this request.
         */
        public virtual RenderingContext getRenderingContext()
        {
            return RenderingContext.GADGET;
        }

        /**
         * @return Whether or not to bypass caching behavior for the current request.
         */
        public virtual bool getIgnoreCache()
        {
            return false;
        }

        /**
         * @return The container of the current request.
         */
        public virtual String getContainer()
        {
            return ContainerConfig.DEFAULT_CONTAINER;
        }

        /**
         * @return The host for which the current request is being made.
         */
        public virtual String getHost()
        {
            return null;
        }

        /**
         * @return The IP Address for the current user.
         */
        public virtual String getUserIp()
        {
            return null;
        }


        /**
         * @return Whether or not to show debug output.
         */
        public virtual bool getDebug()
        {
            return ContainerConfig.getConfigurationValue("gadget.debug").ToLower().Equals("true");
        }

        /**
         * @return Name of view to show
         */
        public virtual String getView()
        {
            return GadgetSpec.DEFAULT_VIEW;
        }

        /**
         * @return The user prefs for the current request.
         */
        public virtual UserPrefs getUserPrefs()
        {
            return UserPrefs.EMPTY;
        }

        /**
         * @return The token associated with this request
         */
        public virtual SecurityToken getToken()
        {
            return null;
        }
    } 
}
