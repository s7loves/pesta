using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

/**
 * Handles converting HttpResponse objects to the format expected by the makeRequest javascript.
 */
namespace Pesta
{
    public class FetchResponseUtils
    {
        /**
       * Convert a response to a JSON object.  static so it can be used by HttpPreloaders as well.
       * 
       * The returned JSON object contains the following values:
       * rc: integer response code
       * body: string response body
       * headers: object, keys are header names, values are lists of header values
       * 
       * @param response the response body
       * @param body string to use as the body of the response.
       * @return a JSONObject representation of the response body.
       */
        public static JsonObject getResponseAsJson(sResponse response, String body)
        {
            JsonObject resp = new JsonObject();
            resp.Put("rc", response.getHttpStatusCode());
            resp.Put("body", body);
            JsonObject headers = new JsonObject();
            addHeaders(headers, response, "set-cookie");
            addHeaders(headers, response, "location");
            resp.Put("headers", headers);
            // Merge in additional response data
            foreach (var entry in response.getMetadata())
            {
                resp.Put(entry.Key, entry.Value);
            }
            return resp;
        }

        private static void addHeaders(JsonObject headers, sResponse response, String headerName)
        {
            string[] values = response.getHeaders(headerName);
            if (values != null)
            {
                headers.Put(headerName.ToLower(), new JsonArray(values));
            }
        }
    }
}
