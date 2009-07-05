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
using System.Linq;
using System.Net;
using System.Collections.Generic;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using Pesta.Engine.auth;
using Pesta.Engine.common;
using Pesta.Engine.protocol;
using Pesta.Engine.protocol.conversion;
using Pesta.Engine.social;
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;
using pestaServer.Models.social.service;
using Activity=Pesta.Engine.social.model.Activity;

/// <summary>
    /// Summary description for JsonDbOpensocialService
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class JsonDbOpensocialService : IPersonService, IActivityService, IAppDataService, IMessagesService
    {
        public readonly static JsonDbOpensocialService Instance = new JsonDbOpensocialService();

        private JsonDbOpensocialService()
        {
            String content = ResourceLoader.GetContent(SampleContainerHandler.Jsondb);
            db = JsonConvert.Import(content) as JsonObject;
            converter = new BeanJsonConverter();
        }
        private class NameComparator : IComparer<Person>
        {
            public int Compare(Person person, Person person1)
            {
                String name = person.name.formatted;
                String name1 = person1.name.formatted;
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
    public static readonly String PEOPLE_TABLE = "people";

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

        /**
        * db["messages"] -> Map<Person.Id, Array<Message>>
        */
        private static readonly String MESSAGE_TABLE = "messages";

        /**
        * db["passwords"] -> Map<Person.Id, String>
        */
        private static readonly String PASSWORDS_TABLE = "passwords";

        public JsonObject getDb()
        {
            return db;
        }

        public void SetDb(JsonObject _db)
        {
            this.db = _db;
        }

        public RestfulCollection<Activity> getActivities(HashSet<UserId> userIds, GroupId groupId,
                                                         String appId, CollectionOptions options, HashSet<String> fields, ISecurityToken token)
        {
            List<Activity> result = new List<Activity>();
            try
            {
                HashSet<String> idSet = GetIdSet(userIds, groupId, token);
                foreach (String id in idSet)
                {
                    if (db.getJSONObject(ACTIVITIES_TABLE).Contains(id))
                    {
                        JsonArray activities = db.getJSONObject(ACTIVITIES_TABLE)[id] as JsonArray;
                        if (activities != null)
                        {
                            for (int i = 0; i < activities.Length; i++)
                            {
                                JsonObject activity = activities[i] as JsonObject;
                                if (appId == null || !activity.Contains(Activity.Field.APP_ID.Value))
                                {
                                    result.Add(ConvertToActivity(activity, fields));
                                }
                                else if (activity[Activity.Field.APP_ID.Value].Equals(appId))
                                {
                                    result.Add(ConvertToActivity(activity, fields));
                                }
                            }
                        }
                    }
                }
                return new RestfulCollection<Activity>(result);
            }
            catch (Exception je)
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR, je.Message);
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
                    if (activities != null)
                    {
                        for (int i = 0; i < activities.Length; i++)
                        {
                            JsonObject activity = activities[i] as JsonObject;
                            if (activity[Activity.Field.USER_ID.Value].Equals(user)
                                && activityIds.Contains(activity[Activity.Field.ID.Value] as string))
                            {
                                result.Add(ConvertToActivity(activity, fields));
                            }
                        }
                    }
                }
                return new RestfulCollection<Activity>(result);
            }
            catch (Exception je)
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR, je.Message);
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
                        if (activity != null && 
                            activity[Activity.Field.USER_ID.Value] as string == user && 
                            activity[Activity.Field.ID.Value] as string == activityId)
                        {
                            return ConvertToActivity(activity, fields);
                        }
                    }
                }
                throw new ProtocolException(ResponseError.BAD_REQUEST, "Activity not found");
            }
            catch (Exception je)
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR, je.Message);
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
                            if (activity != null && !activityIds.Contains(activity[Activity.Field.ID.Value] as string))
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
            catch (Exception je)
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR, je.Message);
            }
        }

        public void createActivity(UserId userId, GroupId groupId, String appId,
                                   HashSet<String> fields, Activity activity, ISecurityToken token)
        {
            // Are fields really needed here?
            try
            {
                JsonObject jsonObject = ConvertFromActivity(activity, fields);
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
            catch (Exception je)
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR, je.Message);
            }
        }

        public override RestfulCollection<Person> getPeople(HashSet<UserId> userIds, GroupId groupId,
                                                            CollectionOptions options, HashSet<String> fields, ISecurityToken token)
        {
            List<Person> result = new List<Person>();
            try
            {
                JsonArray people = db[PEOPLE_TABLE] as JsonArray;

                HashSet<String> idSet = GetIdSet(userIds, groupId, token);

                if (people != null)
                {
                    for (int i = 0; i < people.Length; i++)
                    {
                        JsonObject person = people[i] as JsonObject;
                        if (person != null && !idSet.Contains(person[Person.Field.ID.Value] as string))
                        {
                            continue;
                        }
                        // Add group support later
                        result.Add(ConvertToPerson(person, fields));
                    }
                }
                // We can pretend that by default the people are in top friends order
                if (options.getSortBy().Equals(Person.Field.NAME.Value))
                {
                    result.Sort(new NameComparator());
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
            catch (Exception je)
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR, je.Message);
            }
        }

        public override Person getPerson(UserId id, HashSet<String> fields, ISecurityToken token)
        {
            try
            {
                JsonArray people = db[PEOPLE_TABLE] as JsonArray;

                if (people != null)
                {
                    for (int i = 0; i < people.Length; i++)
                    {
                        JsonObject person = people[i] as JsonObject;
                        if (id != null && person != null && person[Person.Field.ID.Value] as string == id.getUserId(token))
                        {
                            return ConvertToPerson(person, fields);
                        }
                    }
                }
                throw new ProtocolException(ResponseError.BAD_REQUEST, "Person not found");
            }
            catch (Exception je)
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR, je.Message);
            }
        }

        public DataCollection getPersonData(HashSet<UserId> userIds, GroupId groupId,
                                            String appId, HashSet<String> fields, ISecurityToken token)
        {
            try
            {
                Dictionary<String, Dictionary<String, String>> idToData = new Dictionary<string, Dictionary<string, string>>();
                HashSet<String> idSet = GetIdSet(userIds, groupId, token);
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
            catch (Exception je)
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR, je.Message);
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
            catch (Exception je)
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR, je.Message);
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
            catch (Exception je)
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR, je.Message);
            }
        }

        /**
        * Get the set of user id's from a user and group
        */
        private HashSet<String> GetIdSet(UserId user, GroupId group, ISecurityToken token)
        {
            String userId = user.getUserId(token);

            if (group == null)
            {
                return new HashSet<string> { userId };
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
                        if (friends != null)
                        {
                            for (int i = 0; i < friends.Length; i++)
                            {
                                returnVal.Add(friends[i] as string);
                            }
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
        private HashSet<String> GetIdSet(HashSet<UserId> users, GroupId group, ISecurityToken token)
        {
            HashSet<String> ids = new HashSet<string>();
            foreach (UserId user in users)
            {
                ids.UnionWith(GetIdSet(user, group, token));
            }
            return ids;
        }

        private Activity ConvertToActivity(JsonObject inobject, HashSet<String> fields)
        {
            if (fields.Count != 0)
            {
                string[] datafields = new string[fields.Count];
                fields.CopyTo(datafields);
                inobject = new JsonObject(inobject, datafields);
            }
            return converter.ConvertToObject<Activity>(inobject.ToString());
        }

        private JsonObject ConvertFromActivity(Activity activity, HashSet<String> fields)
        {
            // TODO Not using fields yet
            return JsonConvert.Import(converter.ConvertToString(activity)) as JsonObject;
        }

        private Person ConvertToPerson(JsonObject inobject, HashSet<String> fields)
        {
            if (fields.Count != 0)
            {
                // Create a copy with just the specified fields
                string[] datafields = new string[fields.Count];
                fields.CopyTo(datafields);
                inobject = new JsonObject(inobject, datafields);
            }
            return converter.ConvertToObject<Person>(inobject.ToString());
        }

        public RestfulCollection<MessageCollection> getMessageCollections(UserId userId, IEnumerable<string> fields, CollectionOptions options, ISecurityToken token)
        {
            try 
            {
                List<MessageCollection> result = new List<MessageCollection>();
                JsonObject messageCollections = db.getJSONObject(MESSAGE_TABLE).getJSONObject(
                userId.getUserId(token));
                foreach(String msgCollId in messageCollections.Names)
                {
                    JsonObject msgColl = messageCollections.getJSONObject(msgCollId);
                    msgColl.Put("id", msgCollId);
                    JsonArray messages = (JsonArray)msgColl["messages"];
                    int numMessages = (messages == null) ? 0 : messages.Length;
                    msgColl.Put("total", numMessages.ToString());
                    msgColl.Put("unread", numMessages.ToString());

                    result.Add(filterFields<MessageCollection>(msgColl, fields));
                }
                return new RestfulCollection<MessageCollection>(result);
            } 
            catch (JsonException je) 
            {
                throw new ProtocolException((int)HttpStatusCode.InternalServerError, je.Message, je);
            }
        }

        public MessageCollection createMessageCollection(UserId userId, MessageCollection msgCollection, ISecurityToken token)
        {
            throw new ProtocolException((int)HttpStatusCode.NotImplemented,
                                "this functionality is not yet available");
        }

        public void modifyMessageCollection(UserId userId, MessageCollection msgCollection, ISecurityToken token)
        {
            throw new ProtocolException((int)HttpStatusCode.NotImplemented,
                            "this functionality is not yet available");
        }

        public void deleteMessageCollection(UserId userId, string msgCollId, ISecurityToken token)
        {
            throw new ProtocolException((int)HttpStatusCode.NotImplemented,
                                "this functionality is not yet available");
        }

        public RestfulCollection<Message> getMessages(UserId userId, string msgCollId, IEnumerable<string> fields, IEnumerable<string> msgIds, CollectionOptions options, ISecurityToken token)
        {
            try 
            {
                List<Message> result = new List<Message>();
                JsonArray messages = db.getJSONObject(MESSAGE_TABLE)
                                        .getJSONObject(userId.getUserId(token))
                                        .getJSONObject(msgCollId)["messages"] as JsonArray;

                // TODO: special case @all

                if (messages == null) 
                {
                    throw new ProtocolException((int)HttpStatusCode.BadRequest, "message collection"
                                        + msgCollId + " not found");
                }

                // TODO: filter and sort outbox.
                for (int i = 0; i < messages.Length; i++) 
                {
                    JsonObject msg = messages.GetObject(i);
                    result.Add(filterFields<Message>(msg, fields));
                }
                return new RestfulCollection<Message>(result);
            } 
            catch (JsonException je) 
            {
                throw new ProtocolException((int)HttpStatusCode.InternalServerError, je.Message, je);
            }
        }

        public void createMessage(UserId userId, string appId, string msgCollId, Message message, ISecurityToken token)
        {
            foreach(var recipient in message.recipients) 
            {
                try 
                {
                    JsonArray outbox = (JsonArray)db.getJSONObject(MESSAGE_TABLE)[recipient];
                    if (outbox == null) 
                    {
                        outbox = new JsonArray();
                        db.getJSONObject(MESSAGE_TABLE).Put(recipient, outbox);
                    }

                    outbox.Put(message);
                } 
                catch (JsonException je) 
                {
                    throw new ProtocolException((int)HttpStatusCode.InternalServerError, je.Message, je);
                }
            }
        }

        public void deleteMessages(UserId userId, string msgCollId, HashSet<string> ids, ISecurityToken token)
        {
            throw new ProtocolException((int)HttpStatusCode.NotImplemented,
                            "this functionality is not yet available");
        }

        public void modifyMessage(UserId userId, string msgCollId, string messageId, Message message, ISecurityToken token)
        {
            throw new ProtocolException((int)HttpStatusCode.NotImplemented,
                            "this functionality is not yet available");
        }

        private  T filterFields<T>(JsonObject obj, IEnumerable<String> fields)
        {
            if (fields.Count() != 0) 
            {
                // Create a copy with just the specified fields
                obj = new JsonObject(obj, fields.ToArray());
            }
            return converter.ConvertToObject<T>(obj.ToString());
        }
    }
