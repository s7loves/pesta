using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * Contains preloaded data and methods for manipulating it.
 */
namespace Pesta
{
    public interface PreloadedData
    {
        /**
         * Serialize the preloaded data into json.
         *
         * @return A JSON object suitable for passing to org.json.JSONObject.put(String, Object).
         */
        Object toJson();
    }
}
