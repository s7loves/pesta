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
using Pesta.Engine.gadgets.spec;

namespace Pesta.Engine.gadgets.preload
{
    public class ConcurrentPreloaderService : PreloaderService
    {
        private readonly List<Preloader> preloaders = new List<Preloader>();
        /// <summary>
        /// Initializes a new instance of the ConcurrentPreloaderService class.
        /// </summary>
        /// <param name="preloaders"></param>
        public ConcurrentPreloaderService()
        {
            this.preloaders.Add(new HttpPreloader());
        }

        public override Preloads preload(GadgetContext context, GadgetSpec gadget, PreloadPhase phase)
        {
            if (preloaders.Count == 0)
            {
                return null;
            }

            var tasks = new List<Preloader.preloadProcessor>();
            foreach(Preloader preloader in preloaders) 
            {
                ICollection<Preloader.preloadProcessor> taskCollection = preloader.createPreloadTasks(context, gadget, phase);
                tasks.AddRange(taskCollection); 
            }
            ConcurrentPreloads preloads = new ConcurrentPreloads();
            foreach (var task in tasks)
            {
                preloads.add(task.BeginInvoke(null, null));
            }
            return preloads;
        }
    }
}