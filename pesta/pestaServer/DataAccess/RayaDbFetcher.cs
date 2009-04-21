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
        private readonly string url_prefix;

        private RayaDbFetcher()
        {
            Db = new LinqRayaDataContext(ConfigurationManager.ConnectionStrings["rayaConnectionString"].ConnectionString);
            url_prefix = PestaSettings.ContainerUrlPrefix;
        }
        public static RayaDbFetcher Get()
        {
            return new RayaDbFetcher();
        }
        public static LinqRayaDataContext GetDb()
        {
            return new LinqRayaDataContext(ConfigurationManager.ConnectionStrings["rayaConnectionString"].ConnectionString);
        }
        public void Dispose()
        {
            if (Db != null)
            {
                Db.Dispose();
            }
        }

        public bool createActivity(string _person_id, Activity _activity, string _app_id) 
        {
            string _title = (_activity.getTitle() ?? "").Trim();
            if (string.IsNullOrEmpty(_title)) 
            {
                throw new Exception("Invalid activity: empty title");
            }
            string _body = (_activity.getBody() ?? "").Trim();
            var _time = UnixTime.ConvertToUnixTimestamp(DateTime.UtcNow);
            var act = new activity
                          {
                              person_id = int.Parse(_person_id),
                              app_id = int.Parse(_app_id),
                              title = _title,
                              body = _body,
                              created = (long)_time
                          };
            Db.activities.InsertOnSubmit(act);
            Db.SubmitChanges();
            if (Db.GetChangeSet().Inserts.Count != 0)
                return false;
        
            var _mediaItems = _activity.getMediaItems();
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

        public IQueryable<activity> getActivities(HashSet<string> _ids, string _appId, HashSet<String> _fields) 
        {
            var activities = Db.activities
                .Where(x => _ids.AsEnumerable().Contains(x.person_id.ToString()));
            
            return activities;
        }

        public bool deleteActivities(string _userId, string _appId, HashSet<string> _activityIds) 
        {
            var res =
                Db.activities.Where(
                    x => _activityIds.AsEnumerable().Contains(x.id.ToString()) && x.person_id.ToString() == _userId && x.app_id.ToString() == _appId);
            Db.activities.DeleteAllOnSubmit(res);
            Db.SubmitChanges();
            return (Db.GetChangeSet().Deletes.Count == 0);
        }

        public List<MediaItem> getMediaItems(int _activity_id) 
        {
            var _media = new List<MediaItem>();
            var _res = Db.activity_media_items.Where(x=>x.activity_id == _activity_id).Select(x=> new{x.mime_type,x.media_type,x.url});
            foreach (var _re in _res)
            {
                _media.Add(new MediaItemImpl(_re.mime_type, EnumBaseType<MediaItem.Type>.GetBaseByKey(Convert.ToInt32(_re.media_type)),_re.url));
            }
            return _media;
        }

        public HashSet<int> getFriendIds(int _person_id) 
        {
            HashSet<int> _ret = new HashSet<int>();
            var _res =
                Db.friends.Where(x => x.person_id == _person_id || x.friend_id == _person_id)
                            .Select(x =>  new {x.person_id, x.friend_id});

            foreach (var _re in _res)
            {
                int _id = (_re.person_id == _person_id) ? _re.friend_id : _re.person_id;
                _ret.Add(_id);
            }
            return _ret;
        }

        public bool setAppData(string _person_id, string _key, string _value, string _app_id) 
        {
            if (string.IsNullOrEmpty(_value))
            {
                // empty key kind of became to mean "delete data" (was an old orkut hack that became part of the spec spec)
                var ret = new application_setting
                              {
                                  application_id = int.Parse(_app_id),
                                  person_id = int.Parse(_person_id),
                                  name = _key
                              };
                Db.application_settings.DeleteOnSubmit(ret);
                Db.SubmitChanges();
                if (Db.GetChangeSet().Deletes.Count != 0)
                    return false;
            } 
            else 
            {
                var ret = Db.application_settings
                    .Where(x => x.application_id.ToString() == _app_id && x.person_id.ToString() == _person_id && x.name == _key).SingleOrDefault();
                if (ret == null)
                {
                    ret = new application_setting
                              {
                                  application_id = int.Parse(_app_id),
                                  person_id = int.Parse(_person_id),
                                  name = _key,
                                  value = _value
                              };
                    Db.application_settings.InsertOnSubmit(ret);
                    Db.SubmitChanges();
                    if (Db.GetChangeSet().Inserts.Count != 0)
                        return false;
                }
                else
                {
                    ret.value = _value;
                    Db.SubmitChanges();
                    if (Db.GetChangeSet().Updates.Count != 0)
                        return false;
                }
            }
            return true;
        }

        public bool deleteAppData(string _person_id, string _key, string _app_id) 
        {
            var ret = Db.application_settings.Where(
                x => x.application_id.ToString() == _app_id && x.person_id.ToString() == _person_id && x.name == _key).Single();
            Db.application_settings.DeleteOnSubmit(ret);
            Db.SubmitChanges();
            return Db.GetChangeSet().Deletes.Count == 0;
        }

        public DataCollection getAppData(HashSet<String> _ids, HashSet<String> _keys, String _app_id) 
        {
            var _data = new Dictionary<string, Dictionary<string, string>>();
            var _res = Db.application_settings
                .Where(x => (!String.IsNullOrEmpty(_app_id)?x.application_id.ToString() == _app_id : true) && _ids.AsEnumerable().Contains(x.person_id.ToString()) && (_keys.Count == 0 ? true : _keys.AsEnumerable().Contains(x.name)))
                .Select(x => new { x.person_id, x.name, x.value });
            
            foreach (var _re in _res)
            {
                if (!_data.ContainsKey(_re.person_id.ToString()))
                {
                    _data.Add(_re.person_id.ToString(), new Dictionary<string, string>());
                }
                _data[_re.person_id.ToString()].Add(_re.name, _re.value);
            }
            return new DataCollection(_data);
        }

        public Dictionary<string,Person> getPeople(HashSet<String> _ids, HashSet<String> _fields, CollectionOptions _options)
        {
            var _ret = new Dictionary<string, Person>();
            var persons = Db.persons.Where(x => _ids.AsEnumerable().Contains(x.id.ToString()));

            // TODO filter first then fill dictionary

            foreach (var p in persons)
            {
                int _person_id = p.id;
                var _name = new NameImpl();
                var _person = new PersonImpl();
                
                _name.setGivenName(p.first_name);
                _name.setFamilyName(p.last_name);
                _name.setFormatted(p.first_name + " " + p.last_name);
                _person.setDisplayName(_name.getFormatted());
                _person.setName(_name);
                _person.setId(_person_id.ToString());
                if (_fields.Contains("about_me") || _fields.Contains("@all"))
                {
                    _person.setAboutMe(p.about_me);
                }
                if (_fields.Contains("age") || _fields.Contains("@all"))
                {
                    _person.setAge(p.age);
                }
                if (_fields.Contains("children") || _fields.Contains("@all"))
                {
                    _person.setChildren(p.children);
                }
                if (_fields.Contains("date_of_birth") || _fields.Contains("@all"))
                {
                    if (p.date_of_birth.HasValue)
                        _person.setBirthday(new DateTime(p.date_of_birth.Value));
                }
                if (_fields.Contains("ethnicity") || _fields.Contains("@all"))
                {
                    _person.setEthnicity(p.ethnicity);
                }
                if (_fields.Contains("fashion") || _fields.Contains("@all"))
                {
                    _person.setFashion(p.fashion);
                }
                if (_fields.Contains("happiest_when") || _fields.Contains("@all"))
                {
                    _person.setHappiestWhen(p.happiest_when);
                }
                if (_fields.Contains("humor") || _fields.Contains("@all"))
                {
                    _person.setHumor(p.humor);
                }
                if (_fields.Contains("job_interests") || _fields.Contains("@all"))
                {
                    _person.setJobInterests(p.job_interests);
                }
                if (_fields.Contains("living_arrangement") || _fields.Contains("@all"))
                {
                    _person.setLivingArrangement(p.living_arrangement);
                }
                if (_fields.Contains("looking_for") || _fields.Contains("@all"))
                {
                    if (!string.IsNullOrEmpty(p.looking_for))
                    {
                        string[] lookingfors = p.looking_for.Split(',');
                        var lfs = new List<EnumTypes.LookingFor>();
                        foreach (var s in lookingfors)
                        {
                            lfs.Add(EnumBaseType<EnumTypes.LookingFor>.GetBaseByKey(s));
                        }
                        _person.setLookingFor(lfs);
                    }
                }
                if (_fields.Contains("nickname") || _fields.Contains("@all"))
                {
                    _person.setNickname(p.nickname);
                }
                if (_fields.Contains("pets") || _fields.Contains("@all"))
                {
                    _person.setPets(p.pets);
                }
                if (_fields.Contains("political_views") || _fields.Contains("@all"))
                {
                    _person.setPoliticalViews(p.political_views);
                }
                if (_fields.Contains("profile_song") || _fields.Contains("@all"))
                {
                    if (!string.IsNullOrEmpty(p.profile_song))
                    {
                        _person.setProfileSong(new UrlImpl(p.profile_song, "", ""));
                    }
                }
                if (_fields.Contains("profile_url") || _fields.Contains("@all"))
                {
                    _person.setProfileUrl(url_prefix + "/profile/" + _person_id);
                }
                if (_fields.Contains("profile_video") || _fields.Contains("@all"))
                {
                    if (!string.IsNullOrEmpty(p.profile_video))
                    {
                        _person.setProfileVideo(new UrlImpl(p.profile_video, "", ""));
                    }
                }
                if (_fields.Contains("relationship_status") || _fields.Contains("@all"))
                {
                    _person.setRelationshipStatus(p.relationship_status);
                }
                if (_fields.Contains("religion") || _fields.Contains("@all"))
                {
                    _person.setReligion(p.religion);
                }
                if (_fields.Contains("romance") || _fields.Contains("@all"))
                {
                    _person.setRomance(p.romance);
                }
                if (_fields.Contains("scared_of") || _fields.Contains("@all"))
                {
                    _person.setScaredOf(p.scared_of);
                }
                if (_fields.Contains("sexual_orientation") || _fields.Contains("@all"))
                {
                    _person.setSexualOrientation(p.sexual_orientation);
                }
                if (_fields.Contains("status") || _fields.Contains("@all"))
                {
                    _person.setStatus(p.status);
                }
                if (_fields.Contains("thumbnail_url") || _fields.Contains("@all"))
                {
                    _person.setThumbnailUrl(!string.IsNullOrEmpty(p.thumbnail_url) ? url_prefix + p.thumbnail_url : "");
                    if (!string.IsNullOrEmpty(p.thumbnail_url))
                    {
                        _person.setThumbnailUrl(url_prefix + p.thumbnail_url);
                        // also report thumbnail_url in standard photos field (this is the only photo supported by partuza)
                        _person.setPhotos(new List<ListField>
                                              {
                                                  new UrlImpl(url_prefix + p.thumbnail_url, "thumbnail", "thumbnail")
                                              });
                    }
                }
                if (_fields.Contains("time_zone") || _fields.Contains("@all"))
                {
                    _person.setUtcOffset(p.time_zone); // force "-00:00" utc-offset format
                }
                if (_fields.Contains("drinker") || _fields.Contains("@all"))
                {
                    if (!String.IsNullOrEmpty(p.drinker))
                    {
                        _person.setDrinker(EnumBaseType<EnumTypes.Drinker>.GetBaseByKey(p.drinker));
                    }
                }
                if (_fields.Contains("gender") || _fields.Contains("@all"))
                {
                    if (!String.IsNullOrEmpty(p.gender))
                    {
                        _person.setGender(p.gender.ToLower() == Person.Gender.male.ToString()
                                              ? Person.Gender.male
                                              : Person.Gender.female);
                    }
                }
                if (_fields.Contains("smoker") || _fields.Contains("@all"))
                {
                    if (!String.IsNullOrEmpty(p.smoker))
                    {
                        _person.setSmoker(EnumBaseType<EnumTypes.Smoker>.GetBaseByKey(p.smoker));
                    }
                }
                if (_fields.Contains("activities") || _fields.Contains("@all"))
                {
                    var activities = Db.person_activities.Where(a => a.person_id == _person_id).Select(a => a.activity);
                    _person.setActivities(activities.ToList());
                }

                if (_fields.Contains("addresses") || _fields.Contains("@all"))
                {
                    var person_addresses = Db.addresses.
                        Join(Db.person_addresses, a => a.id, b => b.address_id, (a, b) => new { a, b }).
                        Where(x => x.b.person_id == _person_id).
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
                    _person.setAddresses(_addresses);
                }

                if (_fields.Contains("bodyType") || _fields.Contains("@all"))
                {
                    var _row = Db.person_body_types.Where(x => x.person_id == _person_id).SingleOrDefault();
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
                        _person.setBodyType(_bodyType);
                    }
                }

                if (_fields.Contains("books") || _fields.Contains("@all"))
                {
                    var _books = Db.person_books.Where(x => x.person_id == _person_id).Select(x => x.book);
                    _person.setBooks(_books.ToList());
                }

                if (_fields.Contains("cars") || _fields.Contains("@all"))
                {
                    var _cars = Db.person_cars.Where(x => x.person_id == _person_id).Select(x => x.car);
                    _person.setCars(_cars.ToList());
                }

                if (_fields.Contains("currentLocation") || _fields.Contains("@all"))
                {
                    var _row = Db.addresses.
                            Join(Db.person_current_locations, a => a.id, b => b.address_id, (a, b) => new { a, b }).
                            Where(x => x.b.person_id == _person_id).Select(x => x.a).SingleOrDefault();
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
                        _person.setCurrentLocation(_addres);
                    }
                }

                if (_fields.Contains("emails") || _fields.Contains("@all"))
                {
                    var _emails = Db.person_emails.Where(x => x.person_id == _person_id);
                    List<ListField> _emailList = new List<ListField>();
                    foreach (person_email _email in _emails)
                    {
                        _emailList.Add(new ListFieldImpl(_email.email_type, _email.address)); // TODO: better email canonicalization; remove dups
                    }
                    _person.setEmails(_emailList);
                }

                if (_fields.Contains("food") || _fields.Contains("@all"))
                {
                    var _foods = Db.person_foods.Where(x => x.person_id == _person_id).Select(x => x.food);
                    _person.setFood(_foods.ToList());
                }

                if (_fields.Contains("heroes") || _fields.Contains("@all"))
                {
                    var _strings = Db.person_heroes.Where(x => x.person_id == _person_id).Select(x => x.hero);
                    _person.setHeroes(_strings.ToList());
                }

                if (_fields.Contains("interests") || _fields.Contains("@all"))
                {
                    var _strings = Db.person_interests.Where(x => x.person_id == _person_id).Select(x => x.interest);
                    _person.setInterests(_strings.ToList());
                }
                List<Organization> _organizations = new List<Organization>();
                bool _fetchedOrg = false;
                if (_fields.Contains("jobs") || _fields.Contains("@all"))
                {
                    var _org = Db.organizations.
                        Join(Db.person_jobs, a => a.id, b => b.organization_id, (a, b) => new { a, b }).
                        Where(x => x.b.person_id == _person_id).
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

                if (_fields.Contains("schools") || _fields.Contains("@all"))
                {
                    var _res2 = Db.organizations.
                        Join(Db.person_schools, a => a.id, b => b.organization_id, (a, b) => new { a, b }).
                        Where(x => x.b.person_id == _person_id).
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
                    _person.setOrganizations(_organizations);
                }
                //TODO languagesSpoken, currently missing the languages / countries tables so can"t do this yet

                if (_fields.Contains("movies") || _fields.Contains("@all"))
                {
                    var _strings = Db.person_movies.Where(x => x.person_id == _person_id).Select(x => x.movie);
                    _person.setMovies(_strings.ToList());
                }
                if (_fields.Contains("music") || _fields.Contains("@all"))
                {
                    var _strings = Db.person_musics.Where(x => x.person_id == _person_id).Select(x => x.music);
                    _person.setMusic(_strings.ToList());
                }
                if (_fields.Contains("phoneNumbers") || _fields.Contains("@all"))
                {
                    List<ListField> numList = new List<ListField>();
                    var _numbers = Db.person_phone_numbers.Where(x => x.person_id == _person_id);
                    foreach (var _number in _numbers)
                    {
                        numList.Add(new ListFieldImpl(_number.number_type, _number.number));
                    }
                    _person.setPhoneNumbers(numList);
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
                if (_fields.Contains("quotes") || _fields.Contains("@all"))
                {
                    var _strings = Db.person_quotes.Where(x => x.person_id == _person_id).Select(x => x.quote);
                    _person.setQuotes(_strings.ToList());
                }
                if (_fields.Contains("sports") || _fields.Contains("@all"))
                {
                    var _strings = Db.person_sports.Where(x => x.person_id == _person_id).Select(x => x.sport);
                    _person.setSports(_strings.ToList());
                }
                if (_fields.Contains("tags") || _fields.Contains("@all"))
                {
                    var _strings = Db.person_tags.Where(x => x.person_id == _person_id).Select(x => x.tag);
                    _person.setTags(_strings.ToList());
                }

                if (_fields.Contains("turnOns") || _fields.Contains("@all"))
                {
                    var _strings = Db.person_turn_ons.Where(x => x.person_id == _person_id).Select(x => x.turn_on);
                    _person.setTurnOns(_strings.ToList());
                }
                if (_fields.Contains("turnOffs") || _fields.Contains("@all"))
                {
                    var _strings = Db.person_turn_offs.Where(x => x.person_id == _person_id).Select(x => x.turn_off);
                    _person.setTurnOffs(_strings.ToList());
                }
                if (_fields.Contains("urls") || _fields.Contains("@all"))
                {
                    var _strings = Db.person_urls.Where(x => x.person_id == _person_id).Select(x => x.url);
                    List<ListField> urllist = new List<ListField>();
                    foreach (string s in _strings)
                    {
                        var url = new UrlImpl(s, null, null);
                        urllist.Add(url);
                    }
                    urllist.Add(new UrlImpl(url_prefix + "/profile/" + _person_id, null, "profile"));
                    _person.setUrls(urllist);
                }
                _ret.Add(_person_id.ToString(), _person);
            } // foreach

            return _ret;  
        }
    }
}
