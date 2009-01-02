using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Pesta.Engine.social.core.model;
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;
using Pesta.Interop;

namespace Pesta.DataAccess
{
    public class PartuzaDbFetcher : IDisposable
    {
        protected readonly LinqRayaDataContext _db;
        protected string url_prefix;
        protected PartuzaDbFetcher()
        {
            _db = new LinqRayaDataContext(ConfigurationManager.ConnectionStrings["rayaConnectionString"].ConnectionString);
            url_prefix = "";
        }
        public static PartuzaDbFetcher get()
        {
            return new PartuzaDbFetcher();
        }
        public static LinqRayaDataContext getDB()
        {
            return new LinqRayaDataContext(ConfigurationManager.ConnectionStrings["rayaConnectionString"].ConnectionString);
        }
        public void Dispose()
        {
            if (_db != null)
            {
                _db.Dispose();
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
            var _time = DateTime.UtcNow.Ticks;
            var act = new activity
                          {
                              person_id = int.Parse(_person_id),
                              app_id = int.Parse(_app_id),
                              title = _title,
                              body = _body,
                              created = _time
                          };
            _db.activities.InsertOnSubmit(act);
            _db.SubmitChanges();
            if (_db.GetChangeSet().Inserts.Count != 0)
                return false;
        
            var _mediaItems = _activity.getMediaItems();
            if (_mediaItems.Count != 0)
            {
                foreach (var _mediaItem in _mediaItems) 
                {
                    var actm = new activity_media_item
                                   {
                                       activity_id = act.id,
                                       media_type = Convert.ToByte(_mediaItem.getType().Key),
                                       mime_type = _mediaItem.getMimeType(),
                                       url = _mediaItem.getUrl()
                                   };
                    if (!string.IsNullOrEmpty(actm.mime_type) && 
                        actm.media_type != null && 
                        !string.IsNullOrEmpty(actm.url)) 
                    {
                        _db.activity_media_items.InsertOnSubmit(actm);
                        _db.SubmitChanges();
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

        public List<Activity> getActivities(HashSet<string> _ids, string _appId, HashSet<String> _fields) 
        {
            var _activities = new List<Activity>();
            var _res2 = _db.activities
                .Where(x => _ids.AsEnumerable().Contains(x.person_id.ToString()))
                .OrderByDescending(x => x.created)
                .Select(x => new {x.person_id, x.id, x.title, x.body, x.created});
            foreach (var _row in _res2)
            {
                var _activity = new ActivityImpl(_row.id.ToString(), _row.person_id.ToString());
                _activity.setStreamTitle("activities");
                _activity.setTitle(_row.title);
                _activity.setBody(_row.body);
                _activity.setPostedTime(_row.created);
                _activity.setMediaItems(getMediaItems(_row.id));
                _activities.Add(_activity);
            }
            return _activities;
        }

        public bool deleteActivities(string _userId, string _appId, HashSet<string> _activityIds) 
        {
            var res =
                _db.activities.Where(
                    x => _activityIds.AsEnumerable().Contains(x.id.ToString()) && x.person_id.ToString() == _userId && x.app_id.ToString() == _appId);
            _db.activities.DeleteAllOnSubmit(res);
            _db.SubmitChanges();
            return (_db.GetChangeSet().Deletes.Count == 0);
        }

        private List<MediaItem> getMediaItems(int _activity_id) 
        {
            var _media = new List<MediaItem>();
            var _res = _db.activity_media_items.Where(x=>x.activity_id == _activity_id).Select(x=> new{x.mime_type,x.media_type,x.url});
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
                _db.friends.Where(x => x.person_id == _person_id || x.friend_id == _person_id)
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
                _db.application_settings.DeleteOnSubmit(ret);
                _db.SubmitChanges();
                if (_db.GetChangeSet().Deletes.Count != 0)
                    return false;
            } 
            else 
            {
                var ret = _db.application_settings
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
                    _db.application_settings.InsertOnSubmit(ret);
                    _db.SubmitChanges();
                    if (_db.GetChangeSet().Inserts.Count != 0)
                        return false;
                }
                else
                {
                    ret.value = _value;
                    _db.SubmitChanges();
                    if (_db.GetChangeSet().Updates.Count != 0)
                        return false;
                }
            }
            return true;
        }

        public bool deleteAppData(string _person_id, string _key, string _app_id) 
        {
            var ret = _db.application_settings.Where(
                x => x.application_id.ToString() == _app_id && x.person_id.ToString() == _person_id && x.name == _key).Single();
            _db.application_settings.DeleteOnSubmit(ret);
            _db.SubmitChanges();
            return _db.GetChangeSet().Deletes.Count == 0;
        }

        public DataCollection getAppData(HashSet<String> _ids, HashSet<String> _keys, String _app_id) 
        {
            var _data = new Dictionary<string,Dictionary<string, string>>();
            var _res = _db.application_settings
                .Where(x => x.application_id.ToString() == _app_id && _ids.AsEnumerable().Contains(x.person_id.ToString()));
            if (_keys.Count != 0)
            {
                _res = _res.Where(x => _keys.AsEnumerable().Contains(x.name));
            }
            var result = _res.Select(x => new {x.person_id, x.name, x.value});
            foreach (var id in _ids)
            {
                var data = new Dictionary<string, string>();
                foreach (var _re in result)
                {
                    if (_re.person_id.ToString() == id)
                    {
                        data.Add(_re.name, _re.value);
                    }
                }
                _data.Add(id, data);
            }
            
            return new DataCollection(_data);
        }

        public Dictionary<string,Person> getPeople(HashSet<String> _ids, HashSet<String> _fields, CollectionOptions _options)
        {
            var _ret = new Dictionary<string, Person>();
            var persons = _db.persons.Where(x => _ids.AsEnumerable().Contains(x.id.ToString()));

            // TODO filter first then fill dictionary

            foreach (var p in persons)
            {
                int _person_id = p.id;
                var _name = new NameImpl(p.first_name + " " + p.last_name);
                _name.setGivenName(p.first_name);
                _name.setFamilyName(p.last_name);
                var _person = new PersonImpl(_person_id.ToString(), p.nickname, _name);
                _person.setDisplayName(_name.getFormatted());
                _person.setAboutMe(p.about_me);
                _person.setAge(p.age);
                _person.setChildren(p.children);
                if (p.date_of_birth.HasValue)
                    _person.setBirthday(new DateTime(p.date_of_birth.Value));
                _person.setEthnicity(p.ethnicity);
                _person.setFashion(p.fashion);
                _person.setHappiestWhen(p.happiest_when);
                _person.setHumor(p.humor);
                _person.setJobInterests(p.job_interests);
                _person.setLivingArrangement(p.living_arrangement);
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
                _person.setNickname(p.nickname);
                _person.setPets(p.pets);
                _person.setPoliticalViews(p.political_views);
                _person.setProfileSong(new UrlImpl(p.profile_song, "", ""));
                _person.setProfileUrl(url_prefix + "/profile/" + _person_id);
                _person.setProfileVideo(new UrlImpl(p.profile_video, "", ""));
                _person.setRelationshipStatus(p.relationship_status);
                _person.setReligion(p.religion);
                _person.setRomance(p.romance);
                _person.setScaredOf(p.scared_of);
                _person.setSexualOrientation(p.sexual_orientation);
                _person.setStatus(p.status);

                _person.setThumbnailUrl(!string.IsNullOrEmpty(p.thumbnail_url) ? url_prefix + p.thumbnail_url : "");
                if (!string.IsNullOrEmpty(p.thumbnail_url))
                {
                    // also report thumbnail_url in standard photos field (this is the only photo supported by partuza)
                    _person.setPhotos(new List<ListField>{
                  new UrlImpl(url_prefix + p.thumbnail_url, "thumbnail", "thumbnail")});
                }
                _person.setUtcOffset(p.time_zone); // force "-00:00" utc-offset format
                if (!String.IsNullOrEmpty(p.drinker))
                {
                    _person.setDrinker(EnumBaseType<EnumTypes.Drinker>.GetBaseByKey(p.drinker));
                }
                if (!String.IsNullOrEmpty(p.gender))
                {
                    _person.setGender(p.gender.ToLower() == Person.Gender.male.ToString() ? Person.Gender.male: Person.Gender.female);
                }
                if (!String.IsNullOrEmpty(p.smoker))
                {
                    _person.setSmoker(EnumBaseType<EnumTypes.Smoker>.GetBaseByKey(p.smoker));
                }

                if (_fields.Contains("activities") || _fields.Contains("@all"))
                {
                    var activities = _db.person_activities.Where(a => a.person_id == _person_id).Select(a => a.activity);
                    _person.setActivities(activities.ToList());
                }

                if (_fields.Contains("addresses") || _fields.Contains("@all"))
                {
                    var person_addresses = _db.addresses.
                        Join(_db.person_addresses, a => a.id, b => b.address_id, (a, b) => new { a, b }).
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
                    var _row = _db.person_body_types.Where(x => x.person_id == _person_id).SingleOrDefault();
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
                    var _books = _db.person_books.Where(x => x.person_id == _person_id).Select(x => x.book);
                    _person.setBooks(_books.ToList());
                }

                if (_fields.Contains("cars") || _fields.Contains("@all"))
                {
                    var _cars = _db.person_cars.Where(x => x.person_id == _person_id).Select(x => x.car);
                    _person.setCars(_cars.ToList());
                }

                if (_fields.Contains("currentLocation") || _fields.Contains("@all"))
                {
                    var _row = _db.addresses.
                            Join(_db.person_current_locations, a => a.id, b => b.address_id, (a, b) => new { a, b }).
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
                    var _emails = _db.person_emails.Where(x => x.person_id == _person_id);
                    List<ListField> _emailList = new List<ListField>();
                    foreach (person_email _email in _emails)
                    {
                        _emailList.Add(new ListFieldImpl(_email.email_type, _email.address)); // TODO: better email canonicalization; remove dups
                    }
                    _person.setEmails(_emailList);
                }

                if (_fields.Contains("food") || _fields.Contains("@all"))
                {
                    var _foods = _db.person_foods.Where(x => x.person_id == _person_id).Select(x => x.food);
                    _person.setFood(_foods.ToList());
                }

                if (_fields.Contains("heroes") || _fields.Contains("@all"))
                {
                    var _strings = _db.person_heroes.Where(x => x.person_id == _person_id).Select(x => x.hero);
                    _person.setHeroes(_strings.ToList());
                }

                if (_fields.Contains("interests") || _fields.Contains("@all"))
                {
                    var _strings = _db.person_interests.Where(x => x.person_id == _person_id).Select(x => x.interest);
                    _person.setInterests(_strings.ToList());
                }
                List<Organization> _organizations = new List<Organization>();
                bool _fetchedOrg = false;
                if (_fields.Contains("jobs") || _fields.Contains("@all"))
                {
                    var _org = _db.organizations.
                        Join(_db.person_jobs, a => a.id, b => b.organization_id, (a, b) => new { a, b }).
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
                            var _res3 = _db.addresses.Where(x => x.id == addressid).Single();
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
                    var _res2 = _db.organizations.
                        Join(_db.person_schools, a => a.id, b => b.organization_id, (a, b) => new { a, b }).
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
                            var _res3 = _db.addresses.Where(x => x.id == addressid).Single();
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
                    var _strings = _db.person_movies.Where(x => x.person_id == _person_id).Select(x => x.movie);
                    _person.setMovies(_strings.ToList());
                }
                if (_fields.Contains("music") || _fields.Contains("@all"))
                {
                    var _strings = _db.person_musics.Where(x => x.person_id == _person_id).Select(x => x.music);
                    _person.setMusic(_strings.ToList());
                }
                if (_fields.Contains("phoneNumbers") || _fields.Contains("@all"))
                {
                    List<ListField> numList = new List<ListField>();
                    var _numbers = _db.person_phone_numbers.Where(x => x.person_id == _person_id);
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
                    var _strings = _db.person_quotes.Where(x => x.person_id == _person_id).Select(x => x.quote);
                    _person.setQuotes(_strings.ToList());
                }
                if (_fields.Contains("sports") || _fields.Contains("@all"))
                {
                    var _strings = _db.person_sports.Where(x => x.person_id == _person_id).Select(x => x.sport);
                    _person.setSports(_strings.ToList());
                }
                if (_fields.Contains("tags") || _fields.Contains("@all"))
                {
                    var _strings = _db.person_tags.Where(x => x.person_id == _person_id).Select(x => x.tag);
                    _person.setTags(_strings.ToList());
                }

                if (_fields.Contains("turnOns") || _fields.Contains("@all"))
                {
                    var _strings = _db.person_turn_ons.Where(x => x.person_id == _person_id).Select(x => x.turn_on);
                    _person.setTurnOns(_strings.ToList());
                }
                if (_fields.Contains("turnOffs") || _fields.Contains("@all"))
                {
                    var _strings = _db.person_turn_offs.Where(x => x.person_id == _person_id).Select(x => x.turn_off);
                    _person.setTurnOffs(_strings.ToList());
                }
                if (_fields.Contains("urls") || _fields.Contains("@all"))
                {
                    var _strings = _db.person_urls.Where(x => x.person_id == _person_id).Select(x => x.url);
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
