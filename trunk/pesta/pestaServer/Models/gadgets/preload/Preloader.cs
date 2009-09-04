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
using pestaServer.Models.gadgets.spec;

namespace pestaServer.Models.gadgets.preload
{
    
    public abstract class Preloader
    {
        public delegate PreloadedData preloadProcessor();
        /**
   * Create new preload tasks for the provided gadget.
   *
   * @param context The request that needs preloading.
   * @param gadget The gadget that the operations will be performed for.
   * @return Preloading tasks that will be executed by
   *  {@link PreloaderService#preload(GadgetContext, GadgetSpec)}.
   */
        public abstract List<preloadProcessor> createPreloadTasks(GadgetContext context, GadgetSpec gadget, PreloaderService.PreloadPhase phase);

    }
}