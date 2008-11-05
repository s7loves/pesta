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
using System.Collections.Generic;
using System.Text;


namespace Pesta
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class GadgetFeature
    {
        private readonly String name;
        private readonly Dictionary<RenderingContext, Dictionary<String, List<JsLibrary>>> libraries;
        private readonly HashSet<String> dependencies;

        /**
         * @return The name of this feature.
         */
        public String getName()
        {
            return name;
        }

        /**
         * @return All dependencies of this feature.
         */
        public HashSet<String> getDependencies()
        {
            return dependencies;
        }

        /**
         * Adds a new dependency to the graph.
         */
        public void addDependency(String dependency)
        {
            dependencies.Add(dependency);
        }

        /**
         * Adds multiple new dependencies to the graph.
         */
        public void addDependencies(ICollection<String> dependencies)
        {
            foreach (var item in dependencies)
            {
                this.dependencies.Add(item);
            }
        }

        /**
         * Provides javavscript libraries needed to satisfy the requirements for this
         * feature.
         *
         * @param context The context in which the gadget is being used.
         * @param container The container to get libraries for.
         * @return The collection of libraries needed for the provided context.
         */
        public List<JsLibrary> getJsLibraries(RenderingContext context, String container)
        {
            List<JsLibrary> libs = null;

            if (context == null)
            {
                // For this special case we return all JS libraries in a single list.
                // This is usually only used for debugging or at startup, so it's ok
                // that we're creating new objects every time.
                libs = new List<JsLibrary>();
                foreach (var ctx in libraries.Values)
                {
                    foreach (var lib in ctx.Values)
                    {
                        libs.AddRange(lib);
                    }
                }
            }
            else
            {
                Dictionary<String, List<JsLibrary>> contextLibs = null;
                if (libraries.TryGetValue(context, out contextLibs))
                {
                    libs = contextLibs[container];
                    if (libs == null)
                    {
                        // Try default.
                        libs = contextLibs[ContainerConfig.DEFAULT_CONTAINER];
                    }
                }
            }

            if (libs == null)
            {
                return new List<JsLibrary>();
            }
            return libs;
        }

        /**
        * Simplified ctor that registers a set of libraries for all contexts and
        * the default container. Used for testing.
        */
        GadgetFeature(String name, List<JsLibrary> libraries, HashSet<String> dependencies)
        {
            this.name = name;
            this.libraries = new Dictionary<RenderingContext, Dictionary<string, List<JsLibrary>>>();
            foreach (var context in RenderingContext.GetValues())
            {
                Dictionary<String, List<JsLibrary>> container = new Dictionary<string, List<JsLibrary>>();
                container.Add(ContainerConfig.DEFAULT_CONTAINER, libraries);
                this.libraries.Add(context, container);
            }
            this.dependencies = dependencies;
        }
        public GadgetFeature(String name,
          Dictionary<RenderingContext, Dictionary<String, List<JsLibrary>>> libraries,
          HashSet<String> dependencies)
        {
            this.name = name;
            this.libraries = libraries;
            this.dependencies = dependencies;
        }
    } 
}
