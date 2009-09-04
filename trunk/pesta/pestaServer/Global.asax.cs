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
using System.Web.Mvc;
using System.Web.Routing;
using raya.Data.WindowsAzure;

namespace pestaServer
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
#if AZURE
        ApplicationInitAzureTables azureInit;
#endif

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon(.*)?" });

            routes.MapRoute(
               "ifr",                                              // Route name
               "gadgets/ifr/{action}/{id}",                           // URL with parameters
               new { controller = "ifr", action = "Index", id = "" }  // Parameter defaults
           );

            routes.MapRoute(
               "js",                                              // Route name
               "gadgets/js/{resourceName}",                           // URL with parameters
               new { controller = "js", action = "Index", resourceName = "" }  // Parameter defaults
           );

            routes.MapRoute(
               "concat",                                              // Route name
               "gadgets/concat/{action}/{id}",                           // URL with parameters
               new { controller = "concat", action = "Index", id = "" }  // Parameter defaults
           );

            routes.MapRoute(
               "proxy",                                              // Route name
               "gadgets/proxy/{action}/{id}",                           // URL with parameters
               new { controller = "proxy", action = "Index", id = "" }  // Parameter defaults
           );

            routes.MapRoute(
               "makeRequest",                                              // Route name
               "gadgets/makeRequest/{action}/{id}",                           // URL with parameters
               new { controller = "makeRequest", action = "Index", id = "" }  // Parameter defaults
           );

            routes.MapRoute(
               "oauthcallback",                                              // Route name
               "gadgets/oauthcallback/{action}/{id}",                           // URL with parameters
               new { controller = "oauthcallback", action = "Index", id = "" }  // Parameter defaults
           );

            routes.MapRoute(
               "metadata",                                              // Route name
               "gadgets/metadata/{action}/{id}",                           // URL with parameters
               new { controller = "metadata", action = "Index", id = "" }  // Parameter defaults
           );

            routes.MapRoute(
               "rest",                                              // Route name
               "social/rest/{id1}/{id2}/{id3}/{id4}",                           // URL with parameters
               new { controller = "rest", action = "Index", id1 = "", id2 = "", id3 = "", id4 = "" }  // Parameter defaults
           );


            routes.MapRoute(
               "rpc",                                              // Route name
               "social/rpc/{action}/{id}",                           // URL with parameters
               new { controller = "rpc", action = "Index", id = "" }  // Parameter defaults
           );

            routes.MapRoute(
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Home", action = "Index", id = "" }  // Parameter defaults
            );
            routes.MapRoute("Catch All", "{*path}", new { controller = "Error", action = "NotFound" });
        }

        protected void Application_Start()
        {
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
#if AZURE
            azureInit = ApplicationInitAzureTables.Init;
#endif
        }
    }
}