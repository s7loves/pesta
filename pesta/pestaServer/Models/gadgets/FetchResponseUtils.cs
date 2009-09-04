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
using Jayrock.Json;
using pestaServer.Models.gadgets.http;

/**
 * Handles converting HttpResponse objects to the format expected by the makeRequest javascript.
 */

namespace pestaServer.Models.gadgets
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