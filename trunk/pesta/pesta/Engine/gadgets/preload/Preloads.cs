using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pesta
{
    public interface Preloads
    {
        /**
       * @return Keys for all preloaded data.
       */
        ICollection<String> getKeys();

      /**
       * Retrieve a single preload.
       * 
       * @param key The key that the preload is stored under.
       * @return The preloaded data, or null if there is no preload under the specified key (including
       * failure to preload).
       * @throws PreloadException If there was any issue while preloading.
       */
        PreloadedData getData(String key);
    }
}
