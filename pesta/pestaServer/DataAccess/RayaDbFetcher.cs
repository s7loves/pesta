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
using System.Configuration;
using System.Linq;
using Jayrock;
using Pesta.DataAccess;
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;
using Pesta.Utilities;

namespace pestaServer.DataAccess
{
    public class RayaDbFetcher : IDisposable
    {
        private readonly LinqRayaDataContext Db;
        private readonly string urlPrefix;

        private RayaDbFetcher()
        {
            Db = new LinqRayaDataContext(ConfigurationManager.ConnectionStrings["rayaConnectionString"].ConnectionString);
            urlPrefix = PestaSettings.ContainerUrlPrefix;
        }
        public static RayaDbFetcher Get()
        {
            return new RayaDbFetcher();
        }
        public void Dispose()
        {
            if (Db != null)
            {
                Db.Dispose();
            }
        }

        public bool CreateActivity(string personId, Activity activity, string appId) 
        {
            string title = (activity.title ?? "").Trim();
            if (string.IsNullOrEmpty(title)) 
            {
                throw new Exception("Invalid activity: empty title");
            }
            string body = (activity.body ?? "").Trim();
            var time = UnixTime.ToInt64(DateTime.UtcNow);
            var act = new activity
                          {
                              person_id = int.Parse(personId),
                              app_id = int.Parse(appId),
                              title = title,
                              body = body,
                              created = time
                          };
            Db.activities.InsertOnSubmit(act);
            Db.SubmitChanges();
            if (Db.GetChangeSet().Inserts.Count != 0)
                return false;
        
            var mediaItems = activity.mediaItems;
            if (mediaItems.Count != 0)
            {
                foreach (var mediaItem in mediaItems) 
                {
                    var actm = new activity_media_item
                                   {
                                       activity_id = act.id,
                                       media_type = mediaItem.type.ToString().ToLower(),
                                       mime_type = mediaItem.mimeType,
                                       url = mediaItem.url
                                   };
                    if (!string.IsNullOrEmpty(actm.mime_type) && 
                        !string.IsNullOrEmpty(actm.url)) 
                    {
                        Db.activity_media_items.InsertOnSubmit(actm);
                        Db.SubmitChanges();
                        if (actm.id == 0) 
                        {
                            return false;
                        }
                    } 
                    else 
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public IQueryable<activity> GetActivities(HashSet<string> ids, string appId, HashSet<String> fields) 
        {
            var activities = Db.activities
                .OrderByDescending(x => x.id)
                .Where(x => ids.AsEnumerable().Contains(x.person_id.ToString()) && (string.IsNullOrEmpty(appId)?true:x.app_id.ToString() == appId));
            
            return activities;
        }

        public bool DeleteActivities(string userId, string appId, HashSet<string> activityIds) 
        {
            var res =
                Db.activities.Where(
                x => activityIds.AsEnumerable().Contains(x.id.ToString()) && x.person_id.ToString() == userId && x.app_id.ToString() == appId);
            Db.activities.DeleteAllOnSubmit(res);
            Db.SubmitChanges();
            return (Db.GetChangeSet().Deletes.Count == 0);
        }

        public List<MediaItem> GetMediaItems(int activityId) 
        {
            var _media = new List<MediaItem>();
            var _res = Db.activity_media_items.Where(x=>x.activity_id == activityId).Select(x=> new{x.mime_type,x.media_type,x.url});
            foreach (var _re in _res)
            {
                _media.Add(new MediaItem(_re.mime_type, (MediaItem.Type)Enum.Parse(typeof(MediaItem.Type), _re.media_type, true), _re.url));
            }
            return _media;
        }

        public HashSet<int> GetFriendIds(int personId) 
        {
            HashSet<int> _ret = new HashSet<int>();
            var _res =
                Db.friends.Where(x => x.person_id == personId || x.friend_id == personId)
                            .Select(x =>  new {x.person_id, x.friend_id});

            foreach (var _re in _res)
            {
                int _id = (_re.person_id == personId) ? _re.friend_id : _re.person_id;
                _ret.Add(_id);
            }
            return _ret;
        }

        public bool SetAppData(string personId, string key, string value, string appId) 
        {
            if (string.IsNullOrEmpty(value))
            {
                // empty key kind of became to mean "delete data" (was an old orkut hack that became part of the spec spec)
                var ret = new application_setting
                              {
                                  application_id = int.Parse(appId),
                                  person_id = int.Parse(personId),
                                  name = key
                              };
                Db.application_settings.DeleteOnSubmit(ret);
                Db.SubmitChanges();
                if (Db.GetChangeSet().Deletes.Count != 0)
                    return false;
            } 
            else 
            {
                var ret = Db.application_settings
                    .Where(x => x.application_id.ToString() == appId && x.person_id.ToString() == personId && x.name == key).SingleOrDefault();
                if (ret == null)
                {
                    ret = new application_setting
                              {
                                  application_id = int.Parse(appId),
                                  person_id = int.Parse(personId),
                                  name = key,
                                  value = value
                              };
                    Db.application_settings.InsertOnSubmit(ret);
                    Db.SubmitChanges();
                    if (Db.GetChangeSet().Inserts.Count != 0)
                        return false;
                }
                else
                {
                    ret.value = value;
                    Db.SubmitChanges();
                    if (Db.GetChangeSet().Updates.Count != 0)
                        return false;
                }
            }
            return true;
        }

        public bool DeleteAppData(string personId, HashSet<string> key, string appId) 
        {
            var ret = Db.application_settings.Where(
                x => x.application_id.ToString() == appId && x.person_id.ToString() == personId && (key.Count==0?true:key.Contains(x.name)));
            Db.application_settings.DeleteAllOnSubmit(ret);
            Db.SubmitChanges();
            return Db.GetChangeSet().Deletes.Count == 0;
        }

        public DataCollection GetAppData(HashSet<String> ids, HashSet<String> keys, String appId) 
        {
            var data = new Dictionary<string, Dictionary<string, string>>();
            var res = Db.application_settings
                .Where(x => (!String.IsNullOrEmpty(appId)?x.application_id.ToString() == appId : true) && ids.AsEnumerable().Contains(x.person_id.ToString()) && (keys.Count == 0 ? true : keys.AsEnumerable().Contains(x.name)))
                .Select(x => new { x.person_id, x.name, x.value });
            
            foreach (var re in res)
            {
                if (!data.ContainsKey(re.person_id.ToString()))
                {
                    data.Add(re.person_id.ToString(), new Dictionary<string, string>());
                }
                data[re.person_id.ToString()].Add(re.name, re.value);
            }
            return new DataCollection(data);
        }

        public Dictionary<string,Person> GetPeople(HashSet<String> ids, HashSet<String> fields, CollectionOptions options)
        {
            var result = new Dictionary<string, Person>();
            var persons = Db.persons.Where(x => ids.AsEnumerable().Contains(x.id.ToString()));

            // TODO filter first then fill dictionary

            foreach (var p in persons)
            {
                int personId = p.id;
                var name = new Name();
                var person = new Person();
                
                name.givenName = p.first_name;
                name.familyName = p.last_name;
                name.formatted = p.first_name + " " + p.last_name;
                person.displayName = name.formatted;
                person.name = name;
                person.id = personId.ToString();
                if (fields.Contains("about_me") || fields.Contains("@all"))
                {
                    person.aboutMe = p.about_me;
                }
                if (fields.Contains("age") || fields.Contains("@all"))
                {
                    person.age = p.age;
                }
                if (fields.Contains("children") || fields.Contains("@all"))
                {
                    person.children = p.children;
                }
                if (fields.Contains("date_of_birth") || fields.Contains("@all"))
                {
                    if (p.date_of_birth.HasValue)
                    {
                        person.birthday = UnixTime.ToDateTime(p.date_of_birth.Value);
                    }
                }
                if (fields.Contains("ethnicity") || fields.Contains("@all"))
                {
                    person.ethnicity = p.ethnicity;
                }
                if (fields.Contains("fashion") || fields.Contains("@all"))
                {
                    person.fashion = p.fashion;
                }
                if (fields.Contains("happiest_when") || fields.Contains("@all"))
                {
                    person.happiestWhen = p.happiest_when;
                }
                if (fields.Contains("humor") || fields.Contains("@all"))
                {
                    person.humor = p.humor;
                }
                if (fields.Contains("job_interests") || fields.Contains("@all"))
                {
                    person.jobInterests = p.job_interests;
                }
                if (fields.Contains("living_arrangement") || fields.Contains("@all"))
                {
                    person.livingArrangement = p.living_arrangement;
                }
                if (fields.Contains("looking_for") || fields.Contains("@all"))
                {
                    person._lookingFor = p.looking_for;
                }
                if (fields.Contains("nickname") || fields.Contains("@all"))
                {
                    person.nickname = p.nickname;
                }
                if (fields.Contains("pets") || fields.Contains("@all"))
                {
                    person.pets = p.pets;
                }
                if (fields.Contains("political_views") || fields.Contains("@all"))
                {
                    person.politicalViews = p.political_views;
                }
                if (fields.Contains("profile_song") || fields.Contains("@all"))
                {
                    if (!string.IsNullOrEmpty(p.profile_song))
                    {
                        person.profileSong = new Url(p.profile_song, "", "");
                    }
                }
                if (fields.Contains("profileUrl") || fields.Contains("@all"))
                {
                    person.profileUrl = urlPrefix + "/profile/" + personId;
                }
                if (fields.Contains("profile_video") || fields.Contains("@all"))
                {
                    if (!string.IsNullOrEmpty(p.profile_video))
                    {
                        person.profileVideo = new Url(p.profile_video, "", "");
                    }
                }
                if (fields.Contains("relationship_status") || fields.Contains("@all"))
                {
                    person.relationshipStatus = p.relationship_status;
                }
                if (fields.Contains("religion") || fields.Contains("@all"))
                {
                    person.religion = p.religion;
                }
                if (fields.Contains("romance") || fields.Contains("@all"))
                {
                    person.romance = p.romance;
                }
                if (fields.Contains("scared_of") || fields.Contains("@all"))
                {
                    person.scaredOf = p.scared_of;
                }
                if (fields.Contains("sexual_orientation") || fields.Contains("@all"))
                {
                    person.sexualOrientation = p.sexual_orientation;
                }
                if (fields.Contains("status") || fields.Contains("@all"))
                {
                    person.status = p.status;
                }
                if (fields.Contains("thumbnailUrl") || fields.Contains("@all"))
                {
                    person.thumbnailUrl = !string.IsNullOrEmpty(p.thumbnail_url) ? urlPrefix + p.thumbnail_url : "";
                    if (!string.IsNullOrEmpty(p.thumbnail_url))
                    {
                        person.photos = new List<ListField>
                                              {
                                                  new Url(urlPrefix + p.thumbnail_url, "thumbnail", "thumbnail")
                                              };
                    }
                }
                if (fields.Contains("time_zone") || fields.Contains("@all"))
                {
                    person.utcOffset = p.time_zone; // force "-00:00" utc-offset format
                }
                if (fields.Contains("drinker") || fields.Contains("@all"))
                {
                    if (!String.IsNullOrEmpty(p.drinker))
                    {
                        person.drinker = (Drinker)Enum.Parse(typeof(Drinker), p.drinker);
                    }
                }
                if (fields.Contains("gender") || fields.Contains("@all"))
                {
                    if (!String.IsNullOrEmpty(p.gender))
                    {
                        person.gender = (Person.Gender)Enum.Parse(typeof(Person.Gender), p.gender, true);
                    }
                }
                if (fields.Contains("smoker") || fields.Contains("@all"))
                {
                    if (!String.IsNullOrEmpty(p.smoker))
                    {
                        person.smoker = (Smoker)Enum.Parse(typeof(Smoker), p.smoker); 
                    }
                }
                if (fields.Contains("activities") || fields.Contains("@all"))
                {
                    var activities = Db.person_activities.Where(a => a.person_id == personId).Select(a => a.activity);
                    person.activities = activities.ToList();
                }

                if (fields.Contains("addresses") || fields.Contains("@all"))
                {
                    var person_addresses = Db.addresses.
                        Join(Db.person_addresses, a => a.id, b => b.address_id, (a, b) => new { a, b }).
                        Where(x => x.b.person_id == personId).
                        Select(x => x.a);
                    List<Address> _addresses = new List<Address>();
                    foreach (address _row in person_addresses)
                    {
                        if (String.IsNullOrEmpty(_row.unstructured_address))
                        {
                            _row.unstructured_address = (_row.street_address + " " + _row.region + " " + _row.country).Trim();
                        }
                        var _addres = new Address(_row.unstructured_address);
                        _addres.country = _row.country;
                        _addres.latitude = _row.latitude;
                        _addres.longitude = _row.longitude;
                        _addres.locality = _row.locality;
                        _addres.postalCode = _row.postal_code;
                        _addres.region = _row.region;
                        _addres.streetAddress = _row.street_address;
                        _addres.type = _row.address_type;
                        //FIXME quick and dirty hack to demo PC
                        _addres.primary = true;
                        _addresses.Add(_addres);
                    }
                    person.addresses = _addresses;
                }

                if (fields.Contains("bodyType") || fields.Contains("@all"))
                {
                    var _row = Db.person_body_types.Where(x => x.person_id == personId).SingleOrDefault();
                    if (_row != null)
                    {
                        BodyType _bodyType = new BodyType();
                        _bodyType.build = _row.build;
                        _bodyType.eyeColor = _row.eye_color;
                        _bodyType.hairColor = _row.hair_color;
                        if (_row.height.HasValue)
                            _bodyType.height = float.Parse(_row.height.Value.ToString());
                        if (_row.weight.HasValue)
                            _bodyType.weight = float.Parse(_row.weight.Value.ToString());
                        person.bodyType = _bodyType;
                    }
                }

                if (fields.Contains("books") || fields.Contains("@all"))
                {
                    var books = Db.person_books.Where(x => x.person_id == personId).Select(x => x.book);
                    person.books = books.ToList();
                }

                if (fields.Contains("cars") || fields.Contains("@all"))
                {
                    var _cars = Db.person_cars.Where(x => x.person_id == personId).Select(x => x.car);
                    person.cars = _cars.ToList();
                }

                if (fields.Contains("currentLocation") || fields.Contains("@all"))
                {
                    var _row = Db.addresses.
                            Join(Db.person_current_locations, a => a.id, b => b.address_id, (a, b) => new { a, b }).
                            Where(x => x.b.person_id == personId).Select(x => x.a).SingleOrDefault();
                    if (_row != null)
                    {
                        if (string.IsNullOrEmpty(_row.unstructured_address))
                        {
                            _row.unstructured_address = (_row.street_address + " " + _row.region + " " + _row.country).Trim();
                        }
                        var _addres = new Address(_row.unstructured_address);
                        _addres.country = _row.country;
                        _addres.latitude = _row.latitude;
                        _addres.longitude = _row.longitude;
                        _addres.locality = _row.locality;
                        _addres.postalCode = _row.postal_code;
                        _addres.region = _row.region;
                        _addres.streetAddress = _row.street_address;
                        _addres.type = _row.address_type;
                        person.currentLocation = _addres;
                    }
                }

                if (fields.Contains("emails") || fields.Contains("@all"))
                {
                    var _emails = Db.person_emails.Where(x => x.person_id == personId);
                    List<ListField> _emailList = new List<ListField>();
                    foreach (person_email _email in _emails)
                    {
                        _emailList.Add(new ListField(_email.email_type, _email.address)); // TODO: better email canonicalization; remove dups
                    }
                    person.emails = _emailList;
                }

                if (fields.Contains("food") || fields.Contains("@all"))
                {
                    var _foods = Db.person_foods.Where(x => x.person_id == personId).Select(x => x.food);
                    person.food = _foods.ToList();
                }

                if (fields.Contains("heroes") || fields.Contains("@all"))
                {
                    var _strings = Db.person_heroes.Where(x => x.person_id == personId).Select(x => x.hero);
                    person.heroes = _strings.ToList();
                }

                if (fields.Contains("interests") || fields.Contains("@all"))
                {
                    var _strings = Db.person_interests.Where(x => x.person_id == personId).Select(x => x.interest);
                    person.interests = _strings.ToList();
                }
                List<Organization> _organizations = new List<Organization>();
                bool _fetchedOrg = false;
                if (fields.Contains("jobs") || fields.Contains("@all"))
                {
                    var _org = Db.organizations.
                        Join(Db.person_jobs, a => a.id, b => b.organization_id, (a, b) => new { a, b }).
                        Where(x => x.b.person_id == personId).
                        Select(x => x.a);
                    foreach (var _row in _org)
                    {
                        var _organization = new Organization();
                        _organization.description = _row.description;
                        if (_row.end_date.HasValue)
                            _organization.endDate = UnixTime.ToDateTime(_row.end_date.Value);
                        _organization.field = _row.field;
                        _organization.name = _row.name;
                        _organization.salary = _row.salary;
                        if (_row.start_date.HasValue)
                            _organization.startDate = UnixTime.ToDateTime(_row.start_date.Value);
                        _organization.subField = _row.sub_field;
                        _organization.title = _row.title;
                        _organization.webpage = _row.webpage;
                        _organization.type = "job";
                        if (_row.address_id.HasValue)
                        {
                            int addressid = _row.address_id.Value;
                            var _res3 = Db.addresses.Where(x => x.id == addressid).Single();
                            if (string.IsNullOrEmpty(_res3.unstructured_address))
                            {
                                _res3.unstructured_address = (_res3.street_address + " " + _res3.region + " " + _res3.country).Trim();
                            }
                            var _addres = new Address(_res3.unstructured_address);
                            _addres.country = _res3.country;
                            _addres.latitude = _res3.latitude;
                            _addres.longitude = _res3.longitude;
                            _addres.locality = _res3.locality;
                            _addres.postalCode = _res3.postal_code;
                            _addres.region = _res3.region;
                            _addres.streetAddress = _res3.street_address;
                            _addres.type = _res3.address_type;
                            _organization.address = _addres;
                        }
                        _organizations.Add(_organization);
                    }
                    _fetchedOrg = true;
                }

                if (fields.Contains("schools") || fields.Contains("@all"))
                {
                    var _res2 = Db.organizations.
                        Join(Db.person_schools, a => a.id, b => b.organization_id, (a, b) => new { a, b }).
                        Where(x => x.b.person_id == personId).
                        Select(x => x.a);
                    foreach (var _row in _res2)
                    {
                        var _organization = new Organization();
                        _organization.description = _row.description;
                        if (_row.end_date.HasValue)
                            _organization.endDate = UnixTime.ToDateTime(_row.end_date.Value);
                        _organization.field = _row.field;
                        _organization.name = _row.name;
                        _organization.salary = _row.salary;
                        if (_row.start_date.HasValue)
                            _organization.startDate = UnixTime.ToDateTime(_row.start_date.Value);
                        _organization.subField = _row.sub_field;
                        _organization.title = _row.title;
                        _organization.webpage = _row.webpage;
                        _organization.type = "school";
                        if (_row.address_id.HasValue)
                        {
                            int addressid = _row.address_id.Value;
                            var _res3 = Db.addresses.Where(x => x.id == addressid).Single();
                            if (string.IsNullOrEmpty(_res3.unstructured_address))
                            {
                                _res3.unstructured_address = (_res3.street_address + " " + _res3.region + " " + _res3.country).Trim();
                            }
                            var _addres = new Address(_res3.unstructured_address);
                            _addres.country = _res3.country;
                            _addres.latitude = _res3.latitude;
                            _addres.longitude = _res3.longitude;
                            _addres.locality = _res3.locality;
                            _addres.postalCode = _res3.postal_code;
                            _addres.region = _res3.region;
                            _addres.streetAddress = _res3.street_address;
                            _addres.type = _res3.address_type;
                            _organization.address = _addres;
                        }
                        _organizations.Add(_organization);
                    }
                    _fetchedOrg = true;
                }
                if (_fetchedOrg)
                {
                    person.organizations = _organizations;
                }
                //TODO languagesSpoken, currently missing the languages / countries tables so can"t do this yet

                if (fields.Contains("movies") || fields.Contains("@all"))
                {
                    var _strings = Db.person_movies.Where(x => x.person_id == personId).Select(x => x.movie);
                    person.movies = _strings.ToList();
                }
                if (fields.Contains("music") || fields.Contains("@all"))
                {
                    var _strings = Db.person_musics.Where(x => x.person_id == personId).Select(x => x.music);
                    person.music = _strings.ToList();
                }
                if (fields.Contains("phoneNumbers") || fields.Contains("@all"))
                {
                    List<ListField> numList = new List<ListField>();
                    var _numbers = Db.person_phone_numbers.Where(x => x.person_id == personId);
                    foreach (var _number in _numbers)
                    {
                        numList.Add(new ListField(_number.number_type, _number.number));
                    }
                    person.phoneNumbers = numList;
                }
                /*
                if (_fields.Contains("ims") || _fields.Contains("@all")) 
                {
                    var _ims = array();
                    _res2 = mysqli_query(this._db, "select value, value_type from person_ims where person_id = " + _person_id);
                    while (list(_value, _type) = @mysqli_fetch_row(_res2)) 
                    {
                    _ims[] = new Im(_value, _type);
                    }
                    _person.Ims = _ims;
                }
                if (_fields.Contains("accounts") || _fields.Contains("@all")) {
                _accounts = array();
                _res2 = mysqli_query(this._db, "select domain, userid, username from person_accounts where person_id = " + _person_id);
                while (list(_domain, _userid, _username) = @mysqli_fetch_row(_res2)) {
                _accounts[] = new Account(_domain, _userid, _username);
                }
                _person.Accounts = _accounts;
                }*/
                if (fields.Contains("quotes") || fields.Contains("@all"))
                {
                    var _strings = Db.person_quotes.Where(x => x.person_id == personId).Select(x => x.quote);
                    person.quotes = _strings.ToList();
                }
                if (fields.Contains("sports") || fields.Contains("@all"))
                {
                    var _strings = Db.person_sports.Where(x => x.person_id == personId).Select(x => x.sport);
                    person.sports = _strings.ToList();
                }
                if (fields.Contains("tags") || fields.Contains("@all"))
                {
                    var _strings = Db.person_tags.Where(x => x.person_id == personId).Select(x => x.tag);
                    person.tags = _strings.ToList();
                }

                if (fields.Contains("turnOns") || fields.Contains("@all"))
                {
                    var _strings = Db.person_turn_ons.Where(x => x.person_id == personId).Select(x => x.turn_on);
                    person.turnOns = _strings.ToList();
                }
                if (fields.Contains("turnOffs") || fields.Contains("@all"))
                {
                    var _strings = Db.person_turn_offs.Where(x => x.person_id == personId).Select(x => x.turn_off);
                    person.turnOffs = _strings.ToList();
                }
                
                if (fields.Contains("urls") || fields.Contains("@all"))
                {
                    var _strings = Db.person_urls.Where(x => x.person_id == personId).Select(x => x.url);
                    List<ListField> urllist = new List<ListField>();
                    foreach (string s in _strings)
                    {
                        var url = new Url(s, null, null);
                        urllist.Add(url);
                    }
                    //urllist.Add(new Url(urlPrefix + "/profile/" + personId, null, "profile"));
                    person.urls = urllist;
                }
                 
                result.Add(personId.ToString(), person);
            } // foreach

            return result;  
        }
    }
}
