using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace Pesta.Engine.gadgets.preload
{
    public class ConcurrentPreloads : Preloads
    {
        private readonly Dictionary<String, IAsyncResult> preloads;

        public ConcurrentPreloads()
        {
            preloads = new Dictionary<string, IAsyncResult>();
        }

        /**
        * Add an active preloading process.
        *
        * @param key The key that this preload will be stored under.
        * @param futureData A future that will return the preloaded data.
        */
        public ConcurrentPreloads add(String key, IAsyncResult futureData) 
        {
            preloads.Add(key, futureData);
            return this;
        }

        public ICollection<String> getKeys()
        {
            return preloads.Keys;
        }

        public PreloadedData getData(String key) 
        {
            AsyncResult future = preloads[key] as AsyncResult;

            if (future == null)
            {
                return null;
            }

            try
            {
                Preloader.preloadProcessor processor = (Preloader.preloadProcessor)future.AsyncDelegate;
                return processor.EndInvoke(future);
            }
            catch (Exception e)
            {
                // Callable threw an exception. Throw the original.
                Exception cause = e.InnerException;
                if (cause is PreloadException) 
                {
                    throw (PreloadException) cause;
                }
                throw new PreloadException(cause);
            }
        }
    }
}