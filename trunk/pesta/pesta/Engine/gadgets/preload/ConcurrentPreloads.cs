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
using System.Runtime.Remoting.Messaging;

namespace Pesta.Engine.gadgets.preload
{
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    public class ConcurrentPreloads : IPreloads
    {
        private readonly List<IAsyncResult> tasks;

        public ConcurrentPreloads()
        {
            tasks = new List<IAsyncResult>();
        }

        /**
        * Add an active preloading process.
        *
        * @param key The key that this preload will be stored under.
        * @param futureData A future that will return the preloaded data.
        */
        public ConcurrentPreloads add(IAsyncResult futureData) 
        {
            tasks.Add(futureData);
            return this;
        }

        public ICollection<PreloadedData> getData() 
        {
            var collect = new List<PreloadedData>();
            foreach (var task in tasks)
            {
                collect.Add(getPreloadedData((AsyncResult)task));
            }
            return collect;
        }
        

        private static PreloadedData getPreloadedData(AsyncResult future) 
        {
            try
            {
                Preloader.preloadProcessor processor = (Preloader.preloadProcessor)future.AsyncDelegate;
                return processor.EndInvoke(future);
            }
            catch (Exception e)
            {
                return new FailedPreload(e.InnerException);
            }
        }
        /** PreloadData implementation that reports failure */
        private class FailedPreload : PreloadedData
        {
            private readonly Exception t;

            public FailedPreload(Exception t)
            {
                this.t = t;
            }

            public Dictionary<String, Object> toJson()
            {
                if (t is PreloadException)
                {
                    throw t;
                }

                throw new PreloadException(t);
            }
        }
    }
}