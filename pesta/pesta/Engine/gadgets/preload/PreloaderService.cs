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

using Pesta.Engine.gadgets.spec;

namespace Pesta.Engine.gadgets.preload
{
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    public abstract class PreloaderService
    {
        /**
         * Phases for preloading.
         */
        public enum PreloadPhase
        {
            /**
             * Preloaded content that will be POSTed to a proxied render.
             */
            PROXY_FETCH,

            /**
             * Preloaded content that will be delivered to the final render
             */
            HTML_RENDER
        }

        /**
       * Begin all preload operations.
       *
       * @param context The request that needs preloading.
       * @param gadget The gadget that the operations will be performed for.
       * @return The preloads for the gadget.
       *
       * TODO: This should probably have a read only input. If we can
       */
        public abstract IPreloads preload(GadgetContext context, GadgetSpec gadget, PreloadPhase phase);
    }
}