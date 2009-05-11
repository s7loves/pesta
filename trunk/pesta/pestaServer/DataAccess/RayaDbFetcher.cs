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
using Pesta.DataAccess;
using Pesta.Engine.social.core.model;
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;
using Pesta.Utilities;

namespace pestaServer.DataAccess
{
    ///  Apache Software License 2.0 2008 Partuza! ported to Pesta by Sean Lin M.T. (my6solutions.com)
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
            string _title = (activity.getTitle() ?? "").Trim();
            if (string.IsNullOrEmpty(_title)) 
            {
                throw new Exception("Invalid activity: empty title");
            }
            string _body = (activity.getBody() ?? "").Trim();
            var _time = UnixTime.ConvertToUnixTimestamp(DateTime.UtcNow);
            var act = new activity
                          {
                              person_id = int.Parse(personId),
                              app_id = int.Parse(appId),
                              title = _title,
                              body = _body,
                              created = (long)_time
                          };
            Db.activities.InsertOnSubmit(act);
            Db.SubmitChanges();
            if (Db.GetChangeSet().Inserts.Count != 0)
                return false;
        
            var _mediaItems = activity.getMediaItems();
            if (_mediaItems.Count != 0)
            {
                foreach (var _mediaItem in _mediaItems) 
                {
                    var actm = new activity_media_item
                                   {
                                       activity_id = act.id,
                                       media_type = _mediaItem.getType().Value,
                                       mime_type = _mediaItem.getMimeType(),
                                       url = _mediaItem.getUrl()
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
                _media.Add(new MediaItemImpl(_re.mime_type, EnumBaseType<MediaItem.Type>.GetBaseByKey(Convert.ToInt32(_re.media_type)),_re.url));
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
            var _ret = new Dictionary<string, Person>();
            var persons = Db.persons.Where(x => ids.AsEnumerable().Contains(x.id.ToString()));

            // TODO filter first then fill dictionary

            foreach (var p in persons)
            {
                int personId = p.id;
                var name = new NameImpl();
                var person = new PersonImpl();
                
                name.setGivenName(p.first_name);
                name.setFamilyName(p.last_name);
                name.setFormatted(p.first_name + " " + p.last_name);
                person.setDisplayName(name.getFormatted());
                person.setName(name);
                person.setId(personId.ToString());
                if (fields.Contains("about_me") || fields.Contains("@all"))
                {
                    person.setAboutMe(p.about_me);
                }
                if (fields.Contains("age") || fields.Contains("@all"))
                {
                    person.setAge(p.age);
                }
                if (fields.Contains("children") || fields.Contains("@all"))
                {
                    person.setChildren(p.children);
                }
                if (fields.Contains("date_of_birth") || fields.Contains("@all"))
                {
                    if (p.date_of_birth.HasValue)
                        person.setBirthday(new DateTime(p.date_of_birth.Value));
                }
                if (fields.Contains("ethnicity") || fields.Contains("@all"))
                {
                    person.setEthnicity(p.ethnicity);
                }
                if (fields.Contains("fashion") || fields.Contains("@all"))
                {
                    person.setFashion(p.fashion);
                }
                if (fields.Contains("happiest_when") || fields.Contains("@all"))
                {
                    person.setHappiestWhen(p.happiest_when);
                }
                if (fields.Contains("humor") || fields.Contains("@all"))
                {
                    person.setHumor(p.humor);
                }
                if (fields.Contains("job_interests") || fields.Contains("@all"))
                {
                    person.setJobInterests(p.job_interests);
                }
                if (fields.Contains("living_arrangement") || fields.Contains("@all"))
                {
                    person.setLivingArrangement(p.living_arrangement);
                }
                if (fields.Contains("looking_for") || fields.Contains("@all"))
                {
                    if (!string.IsNullOrEmpty(p.looking_for))
                    {
                        string[] lookingfors = p.looking_for.Split(',');
                        var lfs = new List<EnumTypes.LookingFor>();
                        foreach (var s in lookingfors)
                        {
                            lfs.Add(EnumBaseType<EnumTypes.LookingFor>.GetBaseByKey(s));
                        }
                        person.setLookingFor(lfs);
                    }
                }
                if (fields.Contains("nickname") || fields.Contains("@all"))
                {
                    person.setNickname(p.nickname);
                }
                if (fields.Contains("pets") || fields.Contains("@all"))
                {
                    person.setPets(p.pets);
                }
                if (fields.Contains("political_views") || fields.Contains("@all"))
                {
                    person.setPoliticalViews(p.political_views);
                }
                if (fields.Contains("profile_song") || fields.Contains("@all"))
                {
                    if (!string.IsNullOrEmpty(p.profile_song))
                    {
                        person.setProfileSong(new UrlImpl(p.profile_song, "", ""));
                    }
                }
                if (fields.Contains("profileUrl") || fields.Contains("@all"))
                {
                    person.setProfileUrl(urlPrefix + "/profile/" + personId);
                }
                if (fields.Contains("profile_video") || fields.Contains("@all"))
                {
                    if (!string.IsNullOrEmpty(p.profile_video))
                    {
                        person.setProfileVideo(new UrlImpl(p.profile_video, "", ""));
                    }
                }
                if (fields.Contains("relationship_status") || fields.Contains("@all"))
                {
                    person.setRelationshipStatus(p.relationship_status);
                }
                if (fields.Contains("religion") || fields.Contains("@all"))
                {
                    person.setReligion(p.religion);
                }
                if (fields.Contains("romance") || fields.Contains("@all"))
                {
                    person.setRomance(p.romance);
                }
                if (fields.Contains("scared_of") || fields.Contains("@all"))
                {
                    person.setScaredOf(p.scared_of);
                }
                if (fields.Contains("sexual_orientation") || fields.Contains("@all"))
                {
                    person.setSexualOrientation(p.sexual_orientation);
                }
                if (fields.Contains("status") || fields.Contains("@all"))
                {
                    person.setStatus(p.status);
                }
                if (fields.Contains("thumbnailUrl") || fields.Contains("@all"))
                {
                    person.setThumbnailUrl(!string.IsNullOrEmpty(p.thumbnail_url) ? urlPrefix + p.thumbnail_url : "");
                    if (!string.IsNullOrEmpty(p.thumbnail_url))
                    {
                        person.setPhotos(new List<ListField>
                                              {
                                                  new UrlImpl(urlPrefix + p.thumbnail_url, "thumbnail", "thumbnail")
                                              });
                    }
                }
                if (fields.Contains("time_zone") || fields.Contains("@all"))
                {
                    person.setUtcOffset(p.time_zone); // force "-00:00" utc-offset format
                }
                if (fields.Contains("drinker") || fields.Contains("@all"))
                {
                    if (!String.IsNullOrEmpty(p.drinker))
                    {
                        person.setDrinker(EnumBaseType<EnumTypes.Drinker>.GetBaseByKey(p.drinker));
                    }
                }
                if (fields.Contains("gender") || fields.Contains("@all"))
                {
                    if (!String.IsNullOrEmpty(p.gender))
                    {
                        person.setGender(p.gender.ToLower() == Person.Gender.male.ToString()
                                              ? Person.Gender.male
                                              : Person.Gender.female);
                    }
                }
                if (fields.Contains("smoker") || fields.Contains("@all"))
                {
                    if (!String.IsNullOrEmpty(p.smoker))
                    {
                        person.setSmoker(EnumBaseType<EnumTypes.Smoker>.GetBaseByKey(p.smoker));
                    }
                }
                if (fields.Contains("activities") || fields.Contains("@all"))
                {
                    var activities = Db.person_activities.Where(a => a.person_id == personId).Select(a => a.activity);
                    person.setActivities(activities.ToList());
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
                        var _addres = new AddressImpl(_row.unstructured_address);
                        _addres.setCountry(_row.country);
                        _addres.setLatitude(_row.latitude);
                        _addres.setLongitude(_row.longitude);
                        _addres.setLocality(_row.locality);
                        _addres.setPostalCode(_row.postal_code);
                        _addres.setRegion(_row.region);
                        _addres.setStreetAddress(_row.street_address);
                        _addres.setType(_row.address_type);
                        //FIXME quick and dirty hack to demo PC
                        _addres.setPrimary(true);
                        _addresses.Add(_addres);
                    }
                    person.setAddresses(_addresses);
                }

                if (fields.Contains("bodyType") || fields.Contains("@all"))
                {
                    var _row = Db.person_body_types.Where(x => x.person_id == personId).SingleOrDefault();
                    if (_row != null)
                    {
                        BodyTypeImpl _bodyType = new BodyTypeImpl();
                        _bodyType.setBuild(_row.build);
                        _bodyType.setEyeColor(_row.eye_color);
                        _bodyType.setHairColor(_row.hair_color);
                        if (_row.height.HasValue)
                            _bodyType.setHeight(float.Parse(_row.height.Value.ToString()));
                        if (_row.weight.HasValue)
                            _bodyType.setWeight(float.Parse(_row.weight.Value.ToString()));
                        person.setBodyType(_bodyType);
                    }
                }

                if (fields.Contains("books") || fields.Contains("@all"))
                {
                    var books = Db.person_books.Where(x => x.person_id == personId).Select(x => x.book);
                    person.setBooks(books.ToList());
                }

                if (fields.Contains("cars") || fields.Contains("@all"))
                {
                    var _cars = Db.person_cars.Where(x => x.person_id == personId).Select(x => x.car);
                    person.setCars(_cars.ToList());
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
                        var _addres = new AddressImpl(_row.unstructured_address);
                        _addres.setCountry(_row.country);
                        _addres.setLatitude(_row.latitude);
                        _addres.setLongitude(_row.longitude);
                        _addres.setLocality(_row.locality);
                        _addres.setPostalCode(_row.postal_code);
                        _addres.setRegion(_row.region);
                        _addres.setStreetAddress(_row.street_address);
                        _addres.setType(_row.address_type);
                        person.setCurrentLocation(_addres);
                    }
                }

                if (fields.Contains("emails") || fields.Contains("@all"))
                {
                    var _emails = Db.person_emails.Where(x => x.person_id == personId);
                    List<ListField> _emailList = new List<ListField>();
                    foreach (person_email _email in _emails)
                    {
                        _emailList.Add(new ListFieldImpl(_email.email_type, _email.address)); // TODO: better email canonicalization; remove dups
                    }
                    person.setEmails(_emailList);
                }

                if (fields.Contains("food") || fields.Contains("@all"))
                {
                    var _foods = Db.person_foods.Where(x => x.person_id == personId).Select(x => x.food);
                    person.setFood(_foods.ToList());
                }

                if (fields.Contains("heroes") || fields.Contains("@all"))
                {
                    var _strings = Db.person_heroes.Where(x => x.person_id == personId).Select(x => x.hero);
                    person.setHeroes(_strings.ToList());
                }

                if (fields.Contains("interests") || fields.Contains("@all"))
                {
                    var _strings = Db.person_interests.Where(x => x.person_id == personId).Select(x => x.interest);
                    person.setInterests(_strings.ToList());
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
                        var _organization = new OrganizationImpl();
                        _organization.setDescription(_row.description);
                        if (_row.end_date.HasValue)
                            _organization.setEndDate(new DateTime(_row.end_date.Value));
                        _organization.setField(_row.field);
                        _organization.setName(_row.name);
                        _organization.setSalary(_row.salary);
                        if (_row.start_date.HasValue)
                            _organization.setStartDate(new DateTime(_row.start_date.Value));
                        _organization.setSubField(_row.sub_field);
                        _organization.setTitle(_row.title);
                        _organization.setWebpage(_row.webpage);
                        _organization.setType("job");
                        if (_row.address_id.HasValue)
                        {
                            int addressid = _row.address_id.Value;
                            var _res3 = Db.addresses.Where(x => x.id == addressid).Single();
                            if (string.IsNullOrEmpty(_res3.unstructured_address))
                            {
                                _res3.unstructured_address = (_res3.street_address + " " + _res3.region + " " + _res3.country).Trim();
                            }
                            var _addres = new AddressImpl(_res3.unstructured_address);
                            _addres.setCountry(_res3.country);
                            _addres.setLatitude(_res3.latitude);
                            _addres.setLongitude(_res3.longitude);
                            _addres.setLocality(_res3.locality);
                            _addres.setPostalCode(_res3.postal_code);
                            _addres.setRegion(_res3.region);
                            _addres.setStreetAddress(_res3.street_address);
                            _addres.setType(_res3.address_type);
                            _organization.setAddress(_addres);
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
                        var _organization = new OrganizationImpl();
                        _organization.setDescription(_row.description);
                        if (_row.end_date.HasValue)
                            _organization.setEndDate(new DateTime(_row.end_date.Value));
                        _organization.setField(_row.field);
                        _organization.setName(_row.name);
                        _organization.setSalary(_row.salary);
                        if (_row.start_date.HasValue)
                            _organization.setStartDate(new DateTime(_row.start_date.Value));
                        _organization.setSubField(_row.sub_field);
                        _organization.setTitle(_row.title);
                        _organization.setWebpage(_row.webpage);
                        _organization.setType("school");
                        if (_row.address_id.HasValue)
                        {
                            int addressid = _row.address_id.Value;
                            var _res3 = Db.addresses.Where(x => x.id == addressid).Single();
                            if (string.IsNullOrEmpty(_res3.unstructured_address))
                            {
                                _res3.unstructured_address = (_res3.street_address + " " + _res3.region + " " + _res3.country).Trim();
                            }
                            var _addres = new AddressImpl(_res3.unstructured_address);
                            _addres.setCountry(_res3.country);
                            _addres.setLatitude(_res3.latitude);
                            _addres.setLongitude(_res3.longitude);
                            _addres.setLocality(_res3.locality);
                            _addres.setPostalCode(_res3.postal_code);
                            _addres.setRegion(_res3.region);
                            _addres.setStreetAddress(_res3.street_address);
                            _addres.setType(_res3.address_type);
                            _organization.setAddress(_addres);
                        }
                        _organizations.Add(_organization);
                    }
                    _fetchedOrg = true;
                }
                if (_fetchedOrg)
                {
                    person.setOrganizations(_organizations);
                }
                //TODO languagesSpoken, currently missing the languages / countries tables so can"t do this yet

                if (fields.Contains("movies") || fields.Contains("@all"))
                {
                    var _strings = Db.person_movies.Where(x => x.person_id == personId).Select(x => x.movie);
                    person.setMovies(_strings.ToList());
                }
                if (fields.Contains("music") || fields.Contains("@all"))
                {
                    var _strings = Db.person_musics.Where(x => x.person_id == personId).Select(x => x.music);
                    person.setMusic(_strings.ToList());
                }
                if (fields.Contains("phoneNumbers") || fields.Contains("@all"))
                {
                    List<ListField> numList = new List<ListField>();
                    var _numbers = Db.person_phone_numbers.Where(x => x.person_id == personId);
                    foreach (var _number in _numbers)
                    {
                        numList.Add(new ListFieldImpl(_number.number_type, _number.number));
                    }
                    person.setPhoneNumbers(numList);
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
                    _person.setIms(_ims);
                }
                if (_fields.Contains("accounts") || _fields.Contains("@all")) {
                _accounts = array();
                _res2 = mysqli_query(this._db, "select domain, userid, username from person_accounts where person_id = " + _person_id);
                while (list(_domain, _userid, _username) = @mysqli_fetch_row(_res2)) {
                _accounts[] = new Account(_domain, _userid, _username);
                }
                _person.setAccounts(_accounts);
                }*/
                if (fields.Contains("quotes") || fields.Contains("@all"))
                {
                    var _strings = Db.person_quotes.Where(x => x.person_id == personId).Select(x => x.quote);
                    person.setQuotes(_strings.ToList());
                }
                if (fields.Contains("sports") || fields.Contains("@all"))
                {
                    var _strings = Db.person_sports.Where(x => x.person_id == personId).Select(x => x.sport);
                    person.setSports(_strings.ToList());
                }
                if (fields.Contains("tags") || fields.Contains("@all"))
                {
                    var _strings = Db.person_tags.Where(x => x.person_id == personId).Select(x => x.tag);
                    person.setTags(_strings.ToList());
                }

                if (fields.Contains("turnOns") || fields.Contains("@all"))
                {
                    var _strings = Db.person_turn_ons.Where(x => x.person_id == personId).Select(x => x.turn_on);
                    person.setTurnOns(_strings.ToList());
                }
                if (fields.Contains("turnOffs") || fields.Contains("@all"))
                {
                    var _strings = Db.person_turn_offs.Where(x => x.person_id == personId).Select(x => x.turn_off);
                    person.setTurnOffs(_strings.ToList());
                }
                
                if (fields.Contains("urls") || fields.Contains("@all"))
                {
                    var _strings = Db.person_urls.Where(x => x.person_id == personId).Select(x => x.url);
                    List<ListField> urllist = new List<ListField>();
                    foreach (string s in _strings)
                    {
                        var url = new UrlImpl(s, null, null);
                        urllist.Add(url);
                    }
                    urllist.Add(new UrlImpl(urlPrefix + "/profile/" + personId, null, "profile"));
                    person.setUrls(urllist);
                }
                 
                _ret.Add(personId.ToString(), person);
            } // foreach

            return _ret;  
        }
    }
}
