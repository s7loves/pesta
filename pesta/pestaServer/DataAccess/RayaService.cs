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
using Pesta.Engine.social;
using Pesta.Engine.social.core.model;
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;
using pestaServer.DataAccess;

///  Apache Software License 2.0 2008 Partuza! ported to Pesta by Sean Lin M.T. (my6solutions.com)
    public class RayaService : IPersonService, IActivityService, IAppDataService, IMessagesService
    {
        public readonly static RayaService Instance = new RayaService();
        protected RayaService()
        {
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

        private static HashSet<String> getIdSet(UserId user, GroupId group, ISecurityToken token)
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
                    var friendIds = RayaDbFetcher.Get().getFriendIds(int.Parse(userId)).ToArray();
                    for (int i = 0; i < friendIds.Length; i++)
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

        private static HashSet<String> getIdSet(IEnumerable<UserId> users, GroupId group, ISecurityToken _token)
        {
            HashSet<String> ids = new HashSet<string>();
            foreach (UserId user in users)
            {
                ids.UnionWith(getIdSet(user, group, _token));
            }
            return ids;
        }

        override public RestfulCollection<Person> getPeople(HashSet<UserId> _userId, GroupId _groupId,
                                                            CollectionOptions _options, HashSet<String> _fields, ISecurityToken _token)
        {
            int first = _options.getFirst();
            int max = _options.getMax();
            HashSet<String> _ids = getIdSet(_userId, _groupId, _token);
            var allPeople = RayaDbFetcher.Get().getPeople(_ids, _fields, _options);
            var totalSize = allPeople.Count;
            var result = new List<Person>();
            if (first < totalSize)
            {
                foreach (var _id in _ids)
                {
                    if (!allPeople.ContainsKey(_id))
                        continue;

                    Person _person = allPeople[_id];
                    if (!_token.isAnonymous() && _id == _token.getViewerId())
                    {
                        _person.setIsViewer(true);
                    }
                    if (!_token.isAnonymous() && _id == _token.getOwnerId())
                    {
                        _person.setIsOwner(true);
                    }
                    result.Add(_person);
                }

                // We can pretend that by default the people are in top friends order
                if (_options.getSortBy().Equals(Person.Field.NAME.Value))
                {
                    result.Sort(new NAME_COMPARATOR());
                }

                if (_options.getSortOrder().Equals(SortOrder.descending))
                {
                    result.Reverse();
                }
                result = result.GetRange(first,
                                         Math.Min(max,
                                                  totalSize - first > 0
                                                      ? totalSize - first
                                                      : 1));
            }
            
            return new RestfulCollection<Person>(result, _options.getFirst(), totalSize);
        }


        override public Person getPerson(UserId _userId, HashSet<String> _fields, ISecurityToken _token)
        {
            try
            {
                var _groupId = new GroupId(GroupId.Type.self, "all");
                var _person = getPeople(new HashSet<UserId> {_userId}, _groupId, new CollectionOptions(), _fields,
                                             _token);
                if (_person.getEntry().Count == 1)
                    return (Person)_person.getEntry()[0];

                throw new SocialSpiException(ResponseError.BAD_REQUEST, "Person not found");
            }
            catch (Exception je)
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR, je.Message, je);
            }
        }

        public DataCollection getPersonData(HashSet<UserId> _userId, GroupId _groupId,
                                            String _appId, HashSet<String> _fields, ISecurityToken _token)
        {
            var _ids = getIdSet(_userId, _groupId, _token);
            var _data = RayaDbFetcher.Get().getAppData(_ids, _fields, _appId);
            
            return _data;
        }


        public void deletePersonData(UserId _userId, GroupId _groupId,
                                     String _appId, HashSet<String> _fields, ISecurityToken _token)
        {
            var _ids = getIdSet(_userId, _groupId, _token);
            if (_ids.Count < 1)
            {
                throw new SocialSpiException(ResponseError.BAD_REQUEST, "No _userId specified");
            }
            if (_ids.Count > 1) 
            {
                throw new SocialSpiException(ResponseError.BAD_REQUEST, "Multiple _userIds not supported");
            }
            IEnumerator<string> iuserid = _ids.GetEnumerator();
            iuserid.MoveNext();
            string userId = iuserid.Current;
            foreach (var _key in _fields)
            {
                if (!RayaDbFetcher.Get().deleteAppData(userId, _key, _appId)) 
                {
                    throw new SocialSpiException(ResponseError.INTERNAL_ERROR, "Internal server error");
                }
            }
        }
        
        public void updatePersonData(UserId _userId, GroupId _groupId,
                                     String _appId, HashSet<String> _fields, Dictionary<String, String> _values, ISecurityToken _token)
        {
            if (_userId.getUserId(_token) == null) 
            {
                throw new SocialSpiException(ResponseError.NOT_FOUND, "Unknown person id.");
            }
            switch (_groupId.getType()) 
            {
                case GroupId.Type.self:
                    foreach (var _key in _fields) 
                    {
                        var _value = _values[_key];
                        if (! RayaDbFetcher.Get().setAppData(_userId.getUserId(_token), _key, _value, _token.getAppId())) 
                        {
                            throw new SocialSpiException(ResponseError.INTERNAL_ERROR, "Internal server error");
                        }
                    }
                    break;
                default:
                    throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED, "We don't support updating data in batches yet");
            }
        }

        public RestfulCollection<Activity> getActivities(HashSet<UserId> _userIds,
                                                         GroupId _groupId, String _appId, CollectionOptions options, HashSet<String> _fields, ISecurityToken _token)
        {
            var _ids = getIdSet(_userIds, _groupId, _token);
            var activities = RayaDbFetcher.Get().getActivities(_ids, _appId, _fields);
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
            List<Activity> Activities = new List<Activity>();
            foreach (var row in activities)
            {
                var _activity = new ActivityImpl(row.id.ToString(), row.person_id.ToString());
                _activity.setStreamTitle("activities");
                _activity.setTitle(row.title);
                _activity.setBody(row.body);
                _activity.setPostedTime(row.created);
                _activity.setMediaItems(RayaDbFetcher.Get().getMediaItems(row.id));
                Activities.Add(_activity);
            }

            return new RestfulCollection<Activity>(Activities, options.getFirst(), totalCount);
        }
        
        public RestfulCollection<Activity> getActivities(UserId _userId, GroupId _groupId,
                                                         String _appId, HashSet<String> _fields, HashSet<String> activityIds, ISecurityToken _token)
        {
            var _ids = getIdSet(_userId, _groupId, _token);
            var _activities = RayaDbFetcher.Get().getActivities(_ids, _appId, _fields);
            List<Activity> Activities = new List<Activity>();
            if (activityIds != null)
            {
                foreach (var row in _activities)
                {
                    if (activityIds.Contains(row.id.ToString()))
                    {
                        var _activity = new ActivityImpl(row.id.ToString(), row.person_id.ToString());
                        _activity.setStreamTitle("activities");
                        _activity.setTitle(row.title);
                        _activity.setBody(row.body);
                        _activity.setPostedTime(row.created);
                        _activity.setMediaItems(RayaDbFetcher.Get().getMediaItems(row.id));
                        Activities.Add(_activity);
                    }
                }
            }

            if (Activities.Count != 0)
            {
                return new RestfulCollection<Activity>(Activities);
            }
            throw new SocialSpiException(ResponseError.NOT_FOUND, "Activity not found");
        }

        public Activity getActivity(UserId _userId, GroupId _groupId, String _appId, CollectionOptions options,
                                    HashSet<String> _fields, String _activityId, ISecurityToken _token)
        {
            var _activities = getActivities(new HashSet<UserId> { _userId }, _groupId, _appId, options, _fields, _token);
            var acts = _activities.getEntry();
            foreach (Activity _activity in acts)
            {
                if (_activity.getId() == _activityId)
                {
                    return _activity;
                }
            }
            throw new SocialSpiException(ResponseError.NOT_FOUND, "Activity not found");
        }

        
        public void deleteActivities(UserId _userId, GroupId _groupId, String _appId,
                                     HashSet<String> _activityIds, ISecurityToken _token)
        {
            var _ids = getIdSet(_userId, _groupId, _token);
            if (_ids.Count < 1 || _ids.Count > 1)
            {
                throw new SocialSpiException(ResponseError.BAD_REQUEST, "Invalid user id or count");
            }
            IEnumerator<string> iuserid = _ids.GetEnumerator();
            iuserid.MoveNext();
            if (!RayaDbFetcher.Get().deleteActivities(iuserid.Current, _appId, _activityIds)) 
            {
                throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED, "Invalid activity id(s)");
            }
        }

        public void createActivity(UserId _userId, GroupId _groupId, String _appId,
                                   HashSet<String> _fields, Activity _activity, ISecurityToken _token)
        {
            try 
            {
                if (_token.getOwnerId() != _token.getViewerId()) 
                {
                    throw new SocialSpiException(ResponseError.UNAUTHORIZED, "unauthorized: Create activity permission denied.");
                }
                RayaDbFetcher.Get().createActivity(_userId.getUserId(_token), _activity, _token.getAppId());
            } 
            catch (Exception _e) 
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR,"Invalid create activity request: " + _e.Message);
            }
        }

        
    
    }
