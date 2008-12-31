﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace Pesta.DataAccess
{
    public class PartuzaService : PersonService, ActivityService, AppDataService, MessagesService
    {
        public readonly static PartuzaService Instance = new PartuzaService();
        protected PartuzaService()
        {
            this.converter = new BeanJsonConverter();
        }
        private readonly BeanConverter converter;

        private class NAME_COMPARATOR : IComparer<Person>
        {
            public int Compare(Person person, Person person1)
            {
                String name = person.getName().getFormatted();
                String name1 = person1.getName().getFormatted();
                return name.CompareTo(name1);
            }
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

        private HashSet<String> getIdSet(UserId user, GroupId group, SecurityToken token)
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
                    var friendIds = PartuzaDbFetcher.get().getFriendIds(int.Parse(userId)).ToArray();
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

        private HashSet<String> getIdSet(HashSet<UserId> users, GroupId group, SecurityToken _token)
        {
            HashSet<String> ids = new HashSet<string>();
            foreach (UserId user in users)
            {
                ids.UnionWith(getIdSet(user, group, _token));
            }
            return ids;
        }

        override public RestfulCollection<Person> getPeople(HashSet<UserId> _userId, GroupId _groupId,
            CollectionOptions _options, HashSet<String> _fields, SecurityToken _token)
        {
            HashSet<String> _ids = this.getIdSet(_userId, _groupId, _token);
            var _allPeople = PartuzaDbFetcher.get().getPeople(_ids, _fields, _options);
            var _totalSize = _allPeople.Count;
            var result = new List<Person>();
            foreach (var _id in _ids) 
            {
                if (!_allPeople.ContainsKey(_id)) 
                    continue;
                
                Person _person = _allPeople[_id];
                if (!_token.isAnonymous() && _id == _token.getViewerId()) 
                {
                    _person.setIsViewer(true);
                }
                if (! _token.isAnonymous() && _id == _token.getOwnerId()) 
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

            result = result.GetRange(_options.getFirst(), Math.Min(_options.getMax(), _totalSize - _options.getFirst() > 0 ? _totalSize - _options.getFirst() : 0));
            return new RestfulCollection<Person>(result, _options.getFirst(), _totalSize);
        }


        override public Person getPerson(UserId _userId, HashSet<String> _fields, SecurityToken _token)
        {
            try
            {
                var _groupId = new GroupId(GroupId.Type.self, "all");
                var _person = this.getPeople(new HashSet<UserId> {_userId}, _groupId, new CollectionOptions(), _fields,
                                             _token);
                if (_person.getEntry().Count == 1)
                    return _person.getEntry()[0];

                throw new SocialSpiException(ResponseError.BAD_REQUEST, "Person not found");
            }
            catch (Exception je)
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR, je.Message, je);
            }
        }

        public DataCollection getPersonData(HashSet<UserId> _userId, GroupId _groupId,
            String _appId, HashSet<String> _fields, SecurityToken _token)
        {
            var _ids = this.getIdSet(_userId, _groupId, _token);
            var _data = PartuzaDbFetcher.get().getAppData(_ids, _fields, _appId);
            if (_data.getEntry().Count == 0) 
            {
                // if the data array is empty, the key was not found, raise a not found error
                throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED, "Unknown person app data key(s): " + string.Join(",",_fields.ToArray()));
            }
            return _data;
        }


        public void deletePersonData(UserId _userId, GroupId _groupId,
            String _appId, HashSet<String> _fields, SecurityToken _token)
        {
            var _ids = this.getIdSet(_userId, _groupId, _token);
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
            _appId = _token.getAppId();
            foreach (var _key in _fields)
            {
                if (!PartuzaDbFetcher.get().deleteAppData(userId, _key, _appId)) 
                {
                    throw new SocialSpiException(ResponseError.INTERNAL_ERROR, "Internal server error");
                }
            }
        }
        
        public void updatePersonData(UserId _userId, GroupId _groupId,
            String _appId, HashSet<String> _fields, Dictionary<String, String> _values, SecurityToken _token)
        {
            if (_userId.getUserId(_token) == null) 
            {
                throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED, "Unknown person id.");
            }
            switch (_groupId.getType()) 
            {
                case GroupId.Type.self:
                    foreach (var _key in _fields) 
                    {
                        var _value = _values[_key] ?? null;
                        if (! PartuzaDbFetcher.get().setAppData(_userId.getUserId(_token), _key, _value, _token.getAppId())) 
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
            GroupId _groupId, String _appId, HashSet<String> _fields, SecurityToken _token)
        {
            var _ids = this.getIdSet(_userIds, _groupId, _token);
            var _activities = PartuzaDbFetcher.get().getActivities(_ids, _appId, _fields);
            if (_activities.Count != 0)
            {
                return new RestfulCollection<Activity>(_activities);
            }

            throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED, "Invalid activity");
        }
        
        public RestfulCollection<Activity> getActivities(UserId _userId, GroupId _groupId,
            String _appId, HashSet<String> _fields, HashSet<String> _activityIds, SecurityToken _token)
        {
            throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED, "We don't support retrieving activities by activity IDs yet");
        }
        
        public Activity getActivity(UserId _userId, GroupId _groupId, String _appId,
            HashSet<String> _fields, String _activityId, SecurityToken _token)
        {
            var _activities = this.getActivities(new HashSet<UserId>{ _userId }, _groupId, _appId, _fields, _token);
            var acts = _activities.getEntry();
            foreach (var _activity in acts)
            {
                if (_activity.getId() == _activityId)
                {
                    return _activity;
                }
            }
            throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED, "Activity not found");
        }

        
        public void deleteActivities(UserId _userId, GroupId _groupId, String _appId,
            HashSet<String> _activityIds, SecurityToken _token)
        {
            var _ids = this.getIdSet(_userId, _groupId, _token);
            if (_ids.Count < 1 || _ids.Count > 1)
            {
                throw new SocialSpiException(ResponseError.BAD_REQUEST, "Invalid user id or count");
            }
            IEnumerator<string> iuserid = _ids.GetEnumerator();
            iuserid.MoveNext();
            if (!PartuzaDbFetcher.get().deleteActivities(iuserid.Current, _appId, _activityIds)) 
            {
                throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED, "Invalid activity id(s)");
            }
        }

        
        public void createActivity(UserId _userId, GroupId _groupId, String _appId,
            HashSet<String> _fields, Activity _activity, SecurityToken _token)
        {
            try 
            {
                if (_token.getOwnerId() != _token.getViewerId()) 
                {
                    throw new SocialSpiException(ResponseError.UNAUTHORIZED, "unauthorized: Create activity permission denied.");
                }
                PartuzaDbFetcher.get().createActivity(_userId.getUserId(_token), _activity, _token.getAppId());
            } 
            catch (Exception _e) 
            {
                throw new SocialSpiException(ResponseError.INTERNAL_ERROR,"Invalid create activity request: " + _e.Message);
            }
        }

        
    
    }
}