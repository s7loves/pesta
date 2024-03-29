﻿#region License, Terms and Conditions
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
using Jayrock.Json;
using System.Runtime.Remoting.Messaging;
using pestaServer.Models.gadgets.process;
using pestaServer.Models.gadgets.spec;

namespace pestaServer.Models.gadgets.servlet
{
    /// <summary>
    /// Summary description for JsonRpcHandler
    /// </summary>
    /// <remarks>
    /// <para>
    
    /// </para>
    /// </remarks>
    public class JsonRpcHandler
    {
        private static Processor Processor;
        private static DefaultUrlGenerator UrlGenerator;
        private delegate JsonObject PreloadProcessor(GadgetContext context);
        public readonly static JsonRpcHandler Instance = new JsonRpcHandler();

        private JsonRpcHandler()
        {
            Processor = Processor.Instance;
            UrlGenerator = DefaultUrlGenerator.Instance;
        }
        /**
         * Processes a JSON request.
         *
         * @param request Original JSON request
         * @return The JSON response.
         */
        public JsonObject Process(JsonObject request)
        {
            JsonObject response = new JsonObject();

            JsonObject requestContext = request.getJSONObject("context");
            JsonArray requestedGadgets = request["gadgets"] as JsonArray;

            // Process all JSON first so that we don't wind up with hanging threads if
            // a JsonException is thrown.
            List<IAsyncResult> gadgets = new List<IAsyncResult>(requestedGadgets.Length);
            for (int i = 0, j = requestedGadgets.Length; i < j; ++i)
            {
                var context = new JsonRpcGadgetContext(requestContext, (JsonObject)requestedGadgets[i]);
                PreloadProcessor proc = new PreloadProcessor(CallJob);
                IAsyncResult result = proc.BeginInvoke(context, null, null);
                gadgets.Add(result);
            }

            foreach (var entry in gadgets)
            {
                try
                {
                    AsyncResult result = (AsyncResult)entry;
                    PreloadProcessor proc = (PreloadProcessor)result.AsyncDelegate;
                    JsonObject gadget = proc.EndInvoke(result);
                    response.Accumulate("gadgets", gadget);
                }
                catch (JsonException e)
                {
                    throw new RpcException("Unable to write JSON", e);
                }
                catch (Exception ee)
                {
                    if (!(ee is RpcException))
                    {
                        throw new RpcException("Processing interrupted", ee);
                    }
                    RpcException e = (RpcException)ee;
                    // Just one gadget failed; mark it as such.
                    try
                    {
                        GadgetContext context = e.Context;
                        
                        JsonObject errorObj = new JsonObject();
                        errorObj.Put("url", context.getUrl())
                            .Put("moduleId", context.getModuleId());
                        errorObj.Accumulate("errors", e.Message);
                        response.Accumulate("gadgets", errorObj);
                    }
                    catch (JsonException je)
                    {
                        throw new RpcException("Unable to write JSON", je);
                    }
                }
            }
            return response;
        }

        private JsonObject CallJob(GadgetContext context)
        {
            try
            {
                JsonObject gadgetJson = new JsonObject();
                Gadget gadget = Processor.Process(context);

                GadgetSpec spec = gadget.getSpec();
                ModulePrefs prefs = spec.getModulePrefs();

                // TODO: modularize response fields based on requested items.
                JsonObject views = new JsonObject();
                foreach (View view in spec.getViews().Values)
                {
                    views.Put(view.getName(), new JsonObject()
                                                  // .Put("content", view.getContent())
                                                  .Put("type", view.getType().ToString().ToLower())
                                                  .Put("quirks", view.getQuirks())
                                                  .Put("preferredHeight", view.getPreferredHeight())
                                                  .Put("preferredWidth", view.getPreferredWidth()));
                }

                // Features.
                List<String> feats = new List<String>();
                foreach (var entry in prefs.getFeatures())
                {
                    feats.Add(entry.Key);
                }
                string[] features = new string[feats.Count];
                feats.CopyTo(features, 0);

                // Links
                JsonObject links = new JsonObject();
                foreach (LinkSpec link in prefs.getLinks().Values)
                {
                    links.Put(link.getRel(), link.getHref());
                }

                JsonObject userPrefs = new JsonObject();

                // User pref specs
                foreach (UserPref pref in spec.getUserPrefs())
                {
                    JsonObject up = new JsonObject()
                        .Put("displayName", pref.getDisplayName())
                        .Put("type", pref.getDataType().ToString().ToLower())
                        .Put("default", pref.getDefaultValue())
                        .Put("enumValues", pref.getEnumValues())
                        .Put("orderedEnumValues", getOrderedEnums(pref));
                    userPrefs.Put(pref.getName(), up);
                }

                // TODO: This should probably just copy all data from
                // ModulePrefs.getAttributes(), but names have to be converted to
                // camel case.
                gadgetJson.Put("iframeUrl", UrlGenerator.getIframeUrl(gadget))
                    .Put("url", context.getUrl().ToString())
                    .Put("moduleId", context.getModuleId())
                    .Put("title", prefs.getTitle())
                    .Put("titleUrl", prefs.getTitleUrl().ToString())
                    .Put("views", views)
                    .Put("features", features)
                    .Put("userPrefs", userPrefs)
                    .Put("links", links)

                    // extended meta data
                    .Put("directoryTitle", prefs.getDirectoryTitle())
                    .Put("description", prefs.getDescription())
                    .Put("thumbnail", prefs.getThumbnail().ToString())
                    .Put("screenshot", prefs.getScreenshot().ToString())
                    .Put("author", prefs.getAuthor())
                    .Put("authorEmail", prefs.getAuthorEmail())
                    .Put("authorAffiliation", prefs.getAuthorAffiliation())
                    .Put("authorLocation", prefs.getAuthorLocation())
                    .Put("authorPhoto", prefs.getAuthorPhoto())
                    .Put("authorAboutme", prefs.getAuthorAboutme())
                    .Put("authorQuote", prefs.getAuthorQuote())
                    .Put("authorLink", prefs.getAuthorLink())
                    .Put("categories", prefs.getCategories())
                    .Put("screenshot", prefs.getScreenshot().ToString())
                    .Put("height", prefs.getHeight())
                    .Put("width", prefs.getWidth())
                    .Put("showStats", prefs.getShowStats())
                    .Put("showInDirectory", prefs.getShowInDirectory())
                    .Put("singleton", prefs.getSingleton())
                    .Put("scaling", prefs.getScaling())
                    .Put("scrolling", prefs.getScrolling());
                return gadgetJson;
            }
            catch (ProcessingException e)
            {
                throw new RpcException(context, e);
                //throw e;
            }
            catch (JsonException e)
            {
                // Shouldn't be possible
                throw new RpcException(context, e);
            }
        }

        private List<JsonObject> getOrderedEnums(UserPref pref) 
        {
            List<UserPref.EnumValuePair> orderedEnums = pref.getOrderedEnumValues();
            List<JsonObject> jsonEnums = new List<JsonObject>();
            foreach(UserPref.EnumValuePair evp in orderedEnums) 
            {
                JsonObject curEnum = new JsonObject();
                curEnum.Put("value", evp.getValue());
                curEnum.Put("displayValue", evp.getDisplayValue());
                jsonEnums.Add(curEnum);
            }
            return jsonEnums;
        }
    }
}