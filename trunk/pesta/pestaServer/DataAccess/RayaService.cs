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
using Pesta.Engine.auth;
using Pesta.Engine.protocol;
using Pesta.Engine.social;
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;
using pestaServer.DataAccess;

    public class RayaService : IPersonService, IActivityService, IAppDataService, IMessagesService
    {
        public readonly static RayaService Instance = new RayaService();
        protected RayaService()
        {
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

        private static HashSet<String> GetIdSet(UserId user, GroupId group, ISecurityToken token)
        {
            String userId = user.getUserId(token);

            if (group == null)
            {
                return new HashSet<string> { userId };
            }

            HashSet<string> returnVal = new HashSet<string>();
            switch (group.getType())
            {
                case GroupId.Type.all:
                case GroupId.Type.friends:
                case GroupId.Type.groupId:
                    var friendIds = RayaDbFetcher.Get().GetFriendIds(int.Parse(userId)).ToList();
                    for (int i = 0; i < friendIds.Count; i++)
                    {
                        returnVal.Add(friendIds[i].ToString());
                    }
                    break;
                case GroupId.Type.self:
                    returnVal.Add(userId);
                    break;
            }
            return returnVal;
        }

        private static HashSet<String> GetIdSet(IEnumerable<UserId> users, GroupId group, ISecurityToken token)
        {
            HashSet<String> ids = new HashSet<string>();
            foreach (UserId user in users)
            {
                ids.UnionWith(GetIdSet(user, group, token));
            }
            return ids;
        }

        override public RestfulCollection<Person> getPeople(HashSet<UserId> userId, GroupId groupId,
                                                            CollectionOptions options, HashSet<String> fields, ISecurityToken token)
        {
            int first = options.getFirst();
            int max = options.getMax();
            HashSet<String> ids = GetIdSet(userId, groupId, token);
            var allPeople = RayaDbFetcher.Get().GetPeople(ids, fields, options);
            var totalSize = allPeople.Count;
            var result = new List<Person>();
            if (first < totalSize)
            {
                foreach (var id in ids)
                {
                    if (!allPeople.ContainsKey(id))
                        continue;

                    Person person = allPeople[id];
                    if (!token.isAnonymous() && id == token.getViewerId())
                    {
                        person.isViewer = true;
                    }
                    if (!token.isAnonymous() && id == token.getOwnerId())
                    {
                        person.isOwner = true;
                    }
                    result.Add(person);
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
                result = result.GetRange(first,
                                         Math.Min(max,
                                                  totalSize - first > 0
                                                      ? totalSize - first
                                                      : 1));
            }
            
            return new RestfulCollection<Person>(result, options.getFirst(), totalSize);
        }


        override public Person getPerson(UserId userId, HashSet<string> fields, ISecurityToken token)
        {
            try
            {
                var groupId = new GroupId(GroupId.Type.self, "all");
                var person = getPeople(new HashSet<UserId> {userId}, groupId, new CollectionOptions(), fields,
                                             token);
                if (person.entry.Count == 1)
                    return (Person)person.entry[0];

                throw new ProtocolException(ResponseError.BAD_REQUEST, "Person not found");
            }
            catch (Exception je)
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR, je.Message);
            }
        }

        public DataCollection getPersonData(HashSet<UserId> userId, GroupId groupId,
                                            String appId, HashSet<String> fields, ISecurityToken token)
        {
            var ids = GetIdSet(userId, groupId, token);
            var data = RayaDbFetcher.Get().GetAppData(ids, fields, appId);
            
            return data;
        }


        public void deletePersonData(UserId uid, GroupId groupId,
                                     String appId, HashSet<String> fields, ISecurityToken token)
        {
            var ids = GetIdSet(uid, groupId, token);
            if (ids.Count < 1)
            {
                throw new ProtocolException(ResponseError.BAD_REQUEST, "No _userId specified");
            }
            if (ids.Count > 1) 
            {
                throw new ProtocolException(ResponseError.BAD_REQUEST, "Multiple _userIds not supported");
            }
            IEnumerator<string> iuserid = ids.GetEnumerator();
            iuserid.MoveNext();
            string userId = iuserid.Current;
            if (!RayaDbFetcher.Get().DeleteAppData(userId, fields, appId))
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR, "Internal server error");
            }
            
        }
        
        public void updatePersonData(UserId userId, GroupId groupId,
                                     String appId, HashSet<String> fields, Dictionary<String, String> values, ISecurityToken token)
        {
            if (userId.getUserId(token) == null) 
            {
                throw new ProtocolException(ResponseError.NOT_FOUND, "Unknown person id.");
            }
            switch (groupId.getType()) 
            {
                case GroupId.Type.self:
                    foreach (var key in fields) 
                    {
                        var value = values[key];
                        if (! RayaDbFetcher.Get().SetAppData(userId.getUserId(token), key, value, token.getAppId())) 
                        {
                            throw new ProtocolException(ResponseError.INTERNAL_ERROR, "Internal server error");
                        }
                    }
                    break;
                default:
                    throw new ProtocolException(ResponseError.NOT_IMPLEMENTED, "We don't support updating data in batches yet");
            }
        }

        public RestfulCollection<Activity> getActivities(HashSet<UserId> userIds,
                GroupId groupId, String appId, CollectionOptions options, HashSet<String> fields, ISecurityToken token)
        {
            var ids = GetIdSet(userIds, groupId, token);
            var activities = RayaDbFetcher.Get().GetActivities(ids, appId, fields);
            int totalCount = activities.Count();
            int first = options.getFirst();
            int max = options.getMax();
            if (first != 0)
            {
                activities = activities.Skip(first);
            }
            if (max != 0)
            {
                activities = activities.Take(max);
            }
            List<Activity> actList = new List<Activity>();
            foreach (var row in activities)
            {
                var act = new Activity(row.id.ToString(), row.person_id.ToString());
                act.streamTitle = "activities";
                act.title = row.title;
                act.body = row.body;
                act.postedTime = row.created;
                act.mediaItems = RayaDbFetcher.Get().GetMediaItems(row.id);
                actList.Add(act);
            }

            return new RestfulCollection<Activity>(actList, options.getFirst(), totalCount);
        }
        
        public RestfulCollection<Activity> getActivities(UserId userId, GroupId groupId,
                                                         String appId, HashSet<String> fields, HashSet<String> activityIds, ISecurityToken token)
        {
            var ids = GetIdSet(userId, groupId, token);
            var activities = RayaDbFetcher.Get().GetActivities(ids, appId, fields);
            List<Activity> actList = new List<Activity>();
            if (activityIds != null)
            {
                foreach (var row in activities)
                {
                    if (activityIds.Contains(row.id.ToString()))
                    {
                        var act = new Activity(row.id.ToString(), row.person_id.ToString());
                        act.streamTitle = "activities";
                        act.title = row.title;
                        act.body = row.body;
                        act.postedTime = row.created;
                        act.mediaItems = RayaDbFetcher.Get().GetMediaItems(row.id);
                        actList.Add(act);
                    }
                }
            }

            if (actList.Count != 0)
            {
                return new RestfulCollection<Activity>(actList);
            }
            throw new ProtocolException(ResponseError.NOT_FOUND, "Activity not found");
        }

        public Activity getActivity(UserId userId, GroupId groupId, String appId, CollectionOptions options,
                                    HashSet<String> fields, String activityId, ISecurityToken token)
        {
            var activities = getActivities(new HashSet<UserId> { userId }, groupId, appId, options, fields, token);
            var acts = activities.entry;
            foreach (Activity activity in acts)
            {
                if (activity.id == activityId)
                {
                    return activity;
                }
            }
            throw new ProtocolException(ResponseError.NOT_FOUND, "Activity not found");
        }

        
        public void deleteActivities(UserId userId, GroupId groupId, String appId,
                                     HashSet<String> activityIds, ISecurityToken token)
        {
            var ids = GetIdSet(userId, groupId, token);
            if (ids.Count < 1 || ids.Count > 1)
            {
                throw new ProtocolException(ResponseError.BAD_REQUEST, "Invalid user id or count");
            }
            IEnumerator<string> iuserid = ids.GetEnumerator();
            iuserid.MoveNext();
            if (!RayaDbFetcher.Get().DeleteActivities(iuserid.Current, appId, activityIds)) 
            {
                throw new ProtocolException(ResponseError.NOT_IMPLEMENTED, "Invalid activity id(s)");
            }
        }

        public void createActivity(UserId userId, GroupId groupId, String appId,
                                   HashSet<String> fields, Activity activity, ISecurityToken token)
        {
            try 
            {
                if (token.getOwnerId() != token.getViewerId()) 
                {
                    throw new ProtocolException(ResponseError.UNAUTHORIZED, "unauthorized: Create activity permission denied.");
                }
                RayaDbFetcher.Get().CreateActivity(userId.getUserId(token), activity, token.getAppId());
            } 
            catch (Exception e) 
            {
                throw new ProtocolException(ResponseError.INTERNAL_ERROR,"Invalid create activity request: " + e.Message);
            }
        }

    public RestfulCollection<MessageCollection> getMessageCollections(UserId userId, IEnumerable<string> fields, CollectionOptions options, ISecurityToken token)
    {
        throw new NotImplementedException();
    }

    public MessageCollection createMessageCollection(UserId userId, MessageCollection msgCollection, ISecurityToken token)
    {
        throw new NotImplementedException();
    }

    public void modifyMessageCollection(UserId userId, MessageCollection msgCollection, ISecurityToken token)
    {
        throw new NotImplementedException();
    }

    public void deleteMessageCollection(UserId userId, string msgCollId, ISecurityToken token)
    {
        throw new NotImplementedException();
    }

    public RestfulCollection<Message> getMessages(UserId userId, string msgCollId, IEnumerable<string> fields, IEnumerable<string> msgIds, CollectionOptions options, ISecurityToken token)
    {
        throw new NotImplementedException();
    }

    public void createMessage(UserId userId, string appId, string msgCollId, Message message, ISecurityToken token)
    {
        throw new NotImplementedException();
    }

    public void deleteMessages(UserId userId, string msgCollId, HashSet<string> ids, ISecurityToken token)
    {
        throw new NotImplementedException();
    }

    public void modifyMessage(UserId userId, string msgCollId, string messageId, Message message, ISecurityToken token)
    {
        throw new NotImplementedException();
    }
    }
