using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pesta
{
    public interface PreloaderService
    {
        /**
       * Begin all preload operations.
       *
       * @param context The request that needs preloading.
       * @param gadget The gadget that the operations will be performed for.
       * @return The preloads for the gadget.
       *
       * TODO: This should probably have a read only input. If we can
       */
        Preloads preload(GadgetContext context, GadgetSpec gadget);
    }
}
