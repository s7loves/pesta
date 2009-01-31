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
using System.Web;
using System.Collections.Generic;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using System.IO;
using Pesta.Engine.auth;
using Pesta.Engine.social;
using Pesta.Engine.social.core.util;
using Pesta.Engine.social.model;
using Pesta.Engine.social.service;
using Pesta.Engine.social.spi;
using Pesta.Utilities;

namespace Pesta.DataAccess
{
    /// <summary>
    /// Summary description for JsonDbOpensocialService
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class JsonDbOpensocialService : PersonService, ActivityService, AppDataService
    {
        public readonly static string dbLocation = HttpContext.Current.Server.MapPath(HttpContext.Current.Request.ApplicationPath) + PestaSettings.JsonDb;
        public readonly static JsonDbOpensocialService Instance = new JsonDbOpensocialService();
        protected JsonDbOpensocialService()
        {
            using (StreamReader reader = new StreamReader(dbLocation))
            {
                String content = reader.ReadToEnd();
                this.db = JsonConvert.Import(content) as JsonObject;
            }
            this.converter = new BeanJsonConverter();
        }
        private class NAME_COMPARATOR : IComparer<Person>
        {
            public int Compare(Person person, Person person1)
            {
                String name = person.getName().getFormatted();
                String name1 = person1.getName().getFormatted();
                return name.CompareTo(name1);
            }
        }

        /**
        * The DB
        */
        private JsonObject db;

        /**
        * The JSON<->Bean converter
        */
        private readonly BeanConverter converter;

        /**
        * db["activities"] -> Array<Person>
        */
        private static readonly String PEOPLE_TABLE = "people";

        /**
        * db["people"] -> Dictionary<Person.Id, Array<Activity>>
        */
        private static readonly String ACTIVITIES_TABLE = "activities";

        /**
        * db["data"] -> Dictionary<Person.Id, Dictionary<String, String>>
        */
        private static readonly String DATA_TABLE = "data";

        /**
        * db["friendLinks"] -> Dictionary<Person.Id, Array<Person.Id>>
        */
        private static readonly String FRIEND_LINK_TABLE = "friendLinks";

        public JsonObject getDb()
        {
            return db;
        }

        public void setDb(JsonObject db)
        {
            this.db = db;
        }

        public RestfulCollection<Activity> getActivities(HashSet<UserId> userIds, GroupId groupId,
                                                         String appId, CollectionOptions options, HashSet<String> fields, ISecurityToken token)
        {
            List<Activity> result = new List<Activity>();
            try
            {
                HashSet<String> idSet = getIdSet(userIds, groupId, token);
                foreach (String id in idSet)
                {
                    if (db.getJSONObject(ACTIVITIES_TABLE).Contains(id))
                    {
                        JsonArray activities = db.getJSONObject(ACTIVITIES_TABLE)[id] as JsonArray;
                        for (int i = 0; i < activities.Length; i++)
                        {
                            JsonObject activity = activities[i] as JsonObject;
                            if (appId == null || !activity.Contains(Activity.Field.APP_ID.Value))
                            {
                                result.Add(convertToActivity(activity, fields));
                            }
                            else if (activity[Activity.Field.APP_ID.Value].Equals(appId))
                            {
                                result.Add(convertToActivity(activity, fields));
                            }
                        }
                    }
                }
                return new RestfulCollection<Activity>(result);
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR, je.Message, je);
            }
        }

        public RestfulCollection<Activity> getActivities(UserId userId, GroupId groupId, String appId,
                                                         HashSet<String> fields, HashSet<String> activityIds, ISecurityToken token)
        {
            List<Activity> result = new List<Activity>();
            try
            {
                String user = userId.getUserId(token);
                if (db.getJSONObject(ACTIVITIES_TABLE).Contains(user))
                {
                    JsonArray activities = db.getJSONObject(ACTIVITIES_TABLE)[user] as JsonArray;
                    for (int i = 0; i < activities.Length; i++)
                    {
                        JsonObject activity = activities[i] as JsonObject;
                        if (activity[Activity.Field.USER_ID.Value].Equals(user)
                            && activityIds.Contains(activity[Activity.Field.ID.Value] as string))
                        {
                            result.Add(convertToActivity(activity, fields));
                        }
                    }
                }
                return new RestfulCollection<Activity>(result);
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR, je.Message, je);
            }
        }

        public Activity getActivity(UserId userId,
                                    GroupId groupId, String appId, CollectionOptions options, HashSet<String> fields, String activityId, ISecurityToken token)
        {
            try
            {
                String user = userId.getUserId(token);
                if (db.getJSONObject(ACTIVITIES_TABLE).Contains(user))
                {
                    JsonArray activities = db.getJSONObject(ACTIVITIES_TABLE)[user] as JsonArray;
                    for (int i = 0; i < activities.Length; i++)
                    {
                        JsonObject activity = activities[i] as JsonObject;
                        if (activity[Activity.Field.USER_ID.Value] as string == user
                            && activity[Activity.Field.ID.Value] as string == activityId)
                        {
                            return convertToActivity(activity, fields);
                        }
                    }
                }
                throw new SocialSpiException(ResponseError.BAD_REQUEST, "Activity not found");
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR, je.Message, je);
            }
        }

        public void deleteActivities(UserId userId, GroupId groupId, String appId,
                                     HashSet<String> activityIds, ISecurityToken token)
        {
            try
            {
                String user = userId.getUserId(token);
                if (db.getJSONObject(ACTIVITIES_TABLE).Contains(user))
                {
                    JsonArray activities = db.getJSONObject(ACTIVITIES_TABLE)[user] as JsonArray;
                    if (activities != null)
                    {
                        JsonArray newList = new JsonArray();
                        for (int i = 0; i < activities.Length; i++)
                        {
                            JsonObject activity = activities[i] as JsonObject;
                            if (!activityIds.Contains(activity[Activity.Field.ID.Value] as string))
                            {
                                newList.Put(activity);
                            }
                        }
                        db.getJSONObject(ACTIVITIES_TABLE).Put(user, newList);
                        // TODO. This seems very odd that we return no useful response in this case
                        // There is no way to represent not-found
                        // if (found) { ??
                        //}
                    }
                }
                // What is the appropriate response here??
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR, je.Message, je);
            }
        }

        public void createActivity(UserId userId, GroupId groupId, String appId,
                                   HashSet<String> fields, Activity activity, ISecurityToken token)
        {
            // Are fields really needed here?
            try
            {
                JsonObject jsonObject = convertFromActivity(activity, fields);
                if (!jsonObject.Contains(Activity.Field.ID.Value))
                {
                    jsonObject.Put(Activity.Field.ID.Value, DateTime.UtcNow.Ticks);
                }
                JsonArray jsonArray = db.getJSONObject(ACTIVITIES_TABLE)[userId.getUserId(token)] as JsonArray;
                if (jsonArray == null)
                {
                    jsonArray = new JsonArray();
                    db.getJSONObject(ACTIVITIES_TABLE).Put(userId.getUserId(token), jsonArray);
                }
                jsonArray.Put(jsonObject);
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR, je.Message, je);
            }
        }

        public override RestfulCollection<Person> getPeople(HashSet<UserId> userIds, GroupId groupId,
                                                            CollectionOptions options, HashSet<String> fields, ISecurityToken token)
        {
            List<Person> result = new List<Person>();
            try
            {
                JsonArray people = db[PEOPLE_TABLE] as JsonArray;

                HashSet<String> idSet = getIdSet(userIds, groupId, token);

                for (int i = 0; i < people.Length; i++)
                {
                    JsonObject person = people[i] as JsonObject;
                    if (!idSet.Contains(person[Person.Field.ID.Value] as string))
                    {
                        continue;
                    }
                    // Add group support later
                    result.Add(convertToPerson(person, fields));
                }

                // We can pretend that by default the people are in top friends order
                if (options.getSortBy().Equals(Person.Field.NAME.Value))
                {
                    result.Sort(new NAME_COMPARATOR());
                }

                if (options.getSortOrder().Equals(SortOrder.descending))
                {
                    result.Reverse();
                }

                // TODO: The samplecontainer doesn't really have the concept of HAS_APP so
                // we can't support any filters yet. We should fix this.

                int totalSize = result.Count;
                int last = options.getFirst() + options.getMax();
                result = result.GetRange(options.getFirst(), Math.Min(last, totalSize) - options.getFirst());

                return new RestfulCollection<Person>(result, options.getFirst(), totalSize);
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR, je.Message, je);
            }
        }

        public override Person getPerson(UserId id, HashSet<String> fields, ISecurityToken token)
        {
            try
            {
                JsonArray people = db[PEOPLE_TABLE] as JsonArray;

                for (int i = 0; i < people.Length; i++)
                {
                    JsonObject person = people[i] as JsonObject;
                    if (id != null && person[Person.Field.ID.Value] as string == id.getUserId(token))
                    {
                        return convertToPerson(person, fields);
                    }
                }
                throw new SocialSpiException(ResponseError.BAD_REQUEST, "Person not found");
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR, je.Message, je);
            }
        }

        public DataCollection getPersonData(HashSet<UserId> userIds, GroupId groupId,
                                            String appId, HashSet<String> fields, ISecurityToken token)
        {
            try
            {
                Dictionary<String, Dictionary<String, String>> idToData = new Dictionary<string, Dictionary<string, string>>();
                HashSet<String> idSet = getIdSet(userIds, groupId, token);
                foreach (String id in idSet)
                {
                    JsonObject personData;
                    if (!db.getJSONObject(DATA_TABLE).Contains(id))
                    {
                        personData = new JsonObject();
                    }
                    else
                    {
                        if (fields.Count != 0)
                        {
                            string[] datafields = new string[fields.Count];
                            fields.CopyTo(datafields);
                            personData = new JsonObject(db.getJSONObject(DATA_TABLE).getJSONObject(id), datafields);
                        }
                        else
                        {
                            personData = db.getJSONObject(DATA_TABLE).getJSONObject(id);
                        }
                    }

                    // TODO: We can use the converter here to do this for us
                    Dictionary<String, String> data = new Dictionary<string, string>();
                    foreach (string key in personData.GetNamesArray())
                    {
                        data.Add(key, personData[key] as string);
                    }
                    idToData.Add(id, data);
                }
                return new DataCollection(idToData);
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR, je.Message, je);
            }
        }

        public void deletePersonData(UserId userId, GroupId groupId, String appId, HashSet<String> fields, ISecurityToken token)
        {
            try
            {
                String user = userId.getUserId(token);
                if (!db.getJSONObject(DATA_TABLE).Contains(user))
                {
                    return;
                }
                JsonObject newPersonData = new JsonObject();
                JsonObject oldPersonData = db.getJSONObject(DATA_TABLE).getJSONObject(user);
                foreach (String key in oldPersonData.GetNamesArray())
                {
                    if (!fields.Contains(key))
                    {
                        newPersonData.Put(key, oldPersonData[key]);
                    }
                }
                db.getJSONObject(DATA_TABLE).Put(user, newPersonData);
                return;
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR, je.Message, je);
            }
        }

        public void updatePersonData(UserId userId, GroupId groupId, String appId,
                                     HashSet<String> fields, Dictionary<String, String> values, ISecurityToken token)
        {
            // TODO: this seems redundant. No need to pass both fields and a map of field->value
            // TODO: According to rest, yes there is. If a field is in the param list but not in the map
            // that means it is a delete
            try
            {
                JsonObject personData = db.getJSONObject(DATA_TABLE).getJSONObject(userId.getUserId(token));
                if (personData == null)
                {
                    personData = new JsonObject();
                    db.getJSONObject(DATA_TABLE).Put(userId.getUserId(token), personData);
                }

                foreach (var entry in values)
                {
                    personData.Put(entry.Key, entry.Value);
                }
                return;
            }
            catch (JsonException je)
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR, je.Message, je);
            }
        }

        /**
        * Get the set of user id's from a user and group
        */
        private HashSet<String> getIdSet(UserId user, GroupId group, ISecurityToken token)
        {
            String userId = user.getUserId(token);

            if (group == null)
            {
                return new HashSet<string>() { userId };
            }

            HashSet<String> returnVal = new HashSet<string>();
            switch (group.getType())
            {
                case GroupId.Type.all:
                case GroupId.Type.friends:
                case GroupId.Type.groupId:
                    if (db[FRIEND_LINK_TABLE].ToString().Contains(userId))
                    {
                        JsonArray friends = ((JsonObject)db[FRIEND_LINK_TABLE])[userId] as JsonArray;
                        for (int i = 0; i < friends.Length; i++)
                        {
                            returnVal.Add(friends[i] as string);
                        }
                    }
                    break;
                case GroupId.Type.self:
                    returnVal.Add(userId);
                    break;
            }
            return returnVal;
        }

        /**
        * Get the set of user id's for a set of users and a group
        */
        private HashSet<String> getIdSet(HashSet<UserId> users, GroupId group, ISecurityToken token)
        {
            HashSet<String> ids = new HashSet<string>();
            foreach (UserId user in users)
            {
                ids.UnionWith(getIdSet(user, group, token));
            }
            return ids;
        }

        private Activity convertToActivity(JsonObject inobject, HashSet<String> fields)
        {
            if (fields.Count != 0)
            {
                string[] datafields = new string[fields.Count];
                fields.CopyTo(datafields);
                inobject = new JsonObject(inobject, datafields);
            }
            return converter.convertToObject(inobject.ToString(), typeof(Activity)) as Activity;
        }

        private JsonObject convertFromActivity(Activity activity, HashSet<String> fields)
        {
            // TODO Not using fields yet
            return JsonConvert.Import(converter.convertToString(activity)) as JsonObject;
        }

        private Person convertToPerson(JsonObject inobject, HashSet<String> fields)
        {
            if (fields.Count != 0)
            {
                // Create a copy with just the specified fields
                string[] datafields = new string[fields.Count];
                fields.CopyTo(datafields);
                inobject = new JsonObject(inobject, datafields);
            }
            return converter.convertToObject(inobject.ToString(), typeof(Person)) as Person;
        }
    }
}