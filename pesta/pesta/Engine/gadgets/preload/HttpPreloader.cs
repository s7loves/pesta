using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;
using System.Runtime.Remoting.Messaging;

namespace Pesta
{
    public class HttpPreloader : Preloader
    {
        private readonly ContentFetcherFactory fetcher;
        

        public HttpPreloader()
        {
            this.fetcher = ContentFetcherFactory.Instance;
        }

        public override Dictionary<String, preloadProcessor> createPreloadTasks(GadgetContext context,
                                                                                    GadgetSpec gadget) 
        {
            Dictionary<String, preloadProcessor> preloads = new Dictionary<String, preloadProcessor>();

            foreach(Preload preload in gadget.getModulePrefs().getPreloads()) 
            {
                HashSet<String> preloadViews = preload.getViews();
                if (preloadViews.Count == 0 || preloadViews.Contains(context.getView())) 
                {
                    PreloadTask task = new PreloadTask(context, preload);
                    preloadProcessor process = new preloadProcessor(task.call);
                    preloads.Add(preload.getHref().ToString(), process);
                }
            }

            return preloads;
        }

        private class PreloadTask 
        {
            private readonly GadgetContext context;
            private readonly Preload preload;

            public PreloadTask(GadgetContext context, Preload preload) 
            {
                this.context = context;
                this.preload = preload;
            }

            public PreloadedData call()
            {
                // TODO: This should be extracted into a common helper that takes any
                // org.apache.shindig.gadgets.spec.RequestAuthenticationInfo.
                sRequest request = new sRequest(preload.getHref())
                                        .setSecurityToken(context.getToken())
                                        .setOAuthArguments(new OAuthArguments(preload))
                                        .setAuthType(preload.getAuthType())
                                        .setContainer(context.getContainer())
                                        .setGadget(Uri.fromJavaUri(context.getUrl()));
                return new HttpPreloadData(ContentFetcherFactory.Instance.fetch(request));
            }
        }

        /**
        * Implements PreloadData by returning a Map that matches the output format used by makeRequest.
        */
        private struct HttpPreloadData : PreloadedData 
        {
            private readonly JsonObject data;

            public HttpPreloadData(sResponse response) 
            {
                JsonObject data = null;
                try 
                {
                    data = FetchResponseUtils.getResponseAsJson(response, response.responseString);
                } 
                catch (JsonException e) 
                {
                    data = new JsonObject();
                }
                this.data = data;
            }

            public Object toJson() 
            {
                return data;
            }
        }
    }
}
