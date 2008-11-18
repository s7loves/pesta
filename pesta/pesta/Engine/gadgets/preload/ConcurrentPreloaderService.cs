using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Pesta
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

        public Preloads preload(GadgetContext context, GadgetSpec gadget)
        {
            ConcurrentPreloads preloads = new ConcurrentPreloads();
            foreach(Preloader preloader in preloaders) 
            {
                Dictionary<String, Preloader.preloadProcessor> tasks = preloader.createPreloadTasks(context, gadget);
                foreach(var entry in tasks)
                {
                    preloads.add(entry.Key, entry.Value.BeginInvoke(null,null));
                }
            }
            return preloads;
        }
    }
}
