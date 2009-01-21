using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Pesta.Engine.gadgets.preload
{
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