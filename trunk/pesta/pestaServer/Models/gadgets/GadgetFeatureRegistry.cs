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
using Pesta.Utilities;
using pestaServer.Models.gadgets;

namespace pestaServer.Models.gadgets
{
    /// <summary>
    /// Summary description for GadgetFeatureRegistry combined with jsfeatureloader
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class GadgetFeatureRegistry
    {
        private readonly Dictionary<String, GadgetFeature> features;
        private readonly Dictionary<String, GadgetFeature> core;
        // Caches the transitive dependencies to enable faster lookups.
        private readonly Dictionary<HashKey<String>, HashSet<GadgetFeature>> cache
            = new Dictionary<HashKey<String>, HashSet<GadgetFeature>>();

        private bool graphComplete;
        public static readonly GadgetFeatureRegistry Instance = new GadgetFeatureRegistry();
        private GadgetFeatureRegistry()
        {
            features = new Dictionary<String, GadgetFeature>();
            core = new Dictionary<String, GadgetFeature>();
            JsFeatureLoader loader = new JsFeatureLoader();
            loader.loadFeatures(AppDomain.CurrentDomain.BaseDirectory + "/Content/features/features.txt", this);
        }

        protected void populateDependencies(HashSet<string> needed, HashSet<GadgetFeature> deps)
        {
            foreach (string feature in needed)
            {
                GadgetFeature feat;
                if (features.TryGetValue(feature, out feat) &&
                    !deps.Contains(feat))
                {
                    populateDependencies(feat.getDependencies(), deps);
                    deps.Add(feat);
                }
            }
        }
        public ICollection<GadgetFeature> getAllFeatures()
        {
            return features.Values;
        }

        /**
        * @return All {@code GadgetFeature} objects necessary for {@code needed} in
        *     graph-dependent order.
        */
        public HashSet<GadgetFeature> getFeatures(HashKey<String> needed)
        {
            return getFeatures(needed, null);
        }

        /**
        * @param needed All features requested by the gadget.
        * @param unsupported Populated with any unsupported features.
        * @return All {@code GadgetFeature} objects necessary for {@code needed} in
        *     graph-dependent order.
        */
        public HashSet<GadgetFeature> getFeatures(HashKey<String> needed,
                                                  HashSet<String> unsupported)
        {
            graphComplete = true;
            if (needed.Count == 0)
            {
                foreach (var item in core.Keys)
                {
                    needed.Add(item);
                }
            }
            // We use the cache only for situations where all needed are available.
            // if any are missing, the result won't be cached.
            HashSet<GadgetFeature> libCache;
            if (cache.TryGetValue(needed, out libCache))
            {
                return libCache;
            }
            HashSet<GadgetFeature> ret = new HashSet<GadgetFeature>();
            populateDependencies(needed, ret);
            // Fill in anything that was optional but missing. These won't be cached.
            if (unsupported != null)
            {
                foreach (String feature in needed)
                {

                    if (!features.ContainsKey(feature))
                    {
                        unsupported.Add(feature);
                    }
                }
            }
            if (unsupported == null || unsupported.Count == 0)
            {
                cache[needed] = ret;
            }
            return ret;
        }

        public void register(GadgetFeature feature)
        {
            if (graphComplete)
            {
                throw new Exception("register should never be " +
                                    "invoked after calling getLibraries");
            }
            if (isCore(feature))
            {
                core[feature.getName()] = feature;
                foreach (var feat in features.Values)
                {
                    feat.addDependency(feature.getName());
                }
            }
            else
            {
                feature.addDependencies(core.Keys);
            }
            features[feature.getName()] = feature;
        }

        private static bool isCore(GadgetFeature feature)
        {
            return feature.getName().StartsWith("core");
        }
    }
}