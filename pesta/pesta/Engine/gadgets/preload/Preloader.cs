using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pesta
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
        public abstract Dictionary<String, preloadProcessor> createPreloadTasks(GadgetContext context, GadgetSpec gadget);

    }
}
