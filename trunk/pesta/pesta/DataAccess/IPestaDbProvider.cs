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
using Pesta.Engine.social.spi;

namespace pesta.Data
{
    public interface IPestaDbProvider
    {
        bool CreateActivity(string personId, Activity activity, string appId);

        List<Activity> GetActivities(HashSet<string> ids, string appId, HashSet<String> fields,
                                     CollectionOptions options);

        List<Activity> GetActivities(HashSet<string> ids, string appId, HashSet<String> fields,
                                     HashSet<String> activityIds);

        bool DeleteActivities(string userId, string appId, HashSet<string> activityIds);
        List<MediaItem> GetMediaItems(string activityId);
        HashSet<string> GetFriendIds(string personId);
        bool SetAppData(string personId, string key, string value, string appId);
        bool DeleteAppData(string personId, HashSet<string> key, string appId);
        DataCollection GetAppData(HashSet<String> ids, HashSet<String> keys, String appId);
        Dictionary<string, Person> GetPeople(HashSet<String> ids, HashSet<String> fields, CollectionOptions options);
    }
}