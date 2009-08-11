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
using System.Linq;
using Microsoft.Samples.ServiceHosting.StorageClient;
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;

namespace pestaServer.DataAccess
{
    public class RayaAzureContext : TableStorageDataServiceContext
    {
        public IQueryable<Person> Persons
        {
            get { return this.CreateQuery<Person>("Persons"); }
        }
        public IQueryable<Activity> Activitys
        {
            get { return this.CreateQuery<Activity>("Activitys"); }
        }
        public IQueryable<MediaItem> MediaItems
        {
            get { return this.CreateQuery<MediaItem>("MediaItems"); }
        }
    }

    public class RayaAzureDbFetcher : IPestaDbProvider
    {


        #region Implementation of IPestaDbProvider

        public bool CreateActivity(string personId, Activity activity, string appId)
        {
            throw new NotImplementedException();
        }

        public List<Activity> GetActivities(HashSet<string> ids, string appId, HashSet<string> fields, CollectionOptions options)
        {
            throw new NotImplementedException();
        }

        public List<Activity> GetActivities(HashSet<string> ids, string appId, HashSet<string> fields, HashSet<string> activityIds)
        {
            throw new NotImplementedException();
        }

        public bool DeleteActivities(string userId, string appId, HashSet<string> activityIds)
        {
            throw new NotImplementedException();
        }

        public List<MediaItem> GetMediaItems(int activityId)
        {
            throw new NotImplementedException();
        }

        public HashSet<int> GetFriendIds(int personId)
        {
            throw new NotImplementedException();
        }

        public bool SetAppData(string personId, string key, string value, string appId)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAppData(string personId, HashSet<string> key, string appId)
        {
            throw new NotImplementedException();
        }

        public DataCollection GetAppData(HashSet<string> ids, HashSet<string> keys, string appId)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, Person> GetPeople(HashSet<string> ids, HashSet<string> fields, CollectionOptions options)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
