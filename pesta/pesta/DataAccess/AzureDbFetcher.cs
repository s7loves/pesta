using System;
using System.Collections.Generic;
using System.Linq;
using Jayrock;
using pesta.Data;
using Pesta.DataAccess.Azure;
using Pesta.Engine.social.spi;
using Pesta.Utilities;
using Pesta.DataAccess.Azure;

namespace Pesta.DataAccess
{
#if AZURE
    public class AzureDbFetcher : IPestaDbProvider, IDisposable
    {
        private readonly string urlPrefix;
        public AzureDbFetcher()
        {
            urlPrefix = PestaSettings.ContainerUrlPrefix;
        }

        public AzureDbFetcher(AzureRayaDataContext ctx)
        {
        }

        public void Dispose()
        {
            
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
            var act = new ActivityRow(personId, time.ToString())
                          {
                              person_id = personId,
                              app_id = appId,
                              title = title,
                              body = body,
                              created = time
                          };
            using (var db = new AzureRayaDataContext())
            {
                db.InsertOnSubmit(AzureRayaDataContext.TableNames.activities, act);
                db.SubmitChanges();

                var mediaItems = activity.mediaItems;
                if (mediaItems.Count != 0)
                {
                    foreach (var mediaItem in mediaItems)
                    {
                        var actm = new MediaItemRow(act.id, mediaItem.url)
                                       {
                                           activity_id = act.id,
                                           media_type = mediaItem.type.ToString().ToLower(),
                                           mime_type = mediaItem.mimeType,
                                           url = mediaItem.url
                                       };
                        if (!string.IsNullOrEmpty(actm.mime_type) &&
                            !string.IsNullOrEmpty(actm.url))
                        {
                            db.InsertOnSubmit(AzureRayaDataContext.TableNames.activityMediaItems, actm);
                            db.SubmitChanges();
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public List<Activity> GetActivities(HashSet<string> ids, string appId, HashSet<String> fields, CollectionOptions options)
        {
            var activityList = new List<ActivityRow>();
            using (var db = new AzureRayaDataContext())
            {
                foreach (var id in ids)
                {
                    if (string.IsNullOrEmpty(appId))
                    {
                        var activities = db.activities
                            .Where(x => x.PartitionKey == id);
                        foreach (var row in activities)
                        {
                            activityList.Add(row);
                        }

                    }
                    else
                    {
                        var activities = db.activities
                            .Where(x => x.PartitionKey == id && x.app_id == appId);
                        foreach (var row in activities)
                        {
                            activityList.Add(row);
                        }
                    }

                }
            }
            
            IEnumerable<ActivityRow> ordered = activityList.OrderByDescending(x => x.id);
            int first = options.getFirst();
            int max = options.getMax();
            if (first != 0)
            {
                ordered = ordered.Skip(first);
            }
            if (max != 0)
            {
                ordered = ordered.Take(max);
            }
            List<Activity> actList = new List<Activity>();
            foreach (var row in ordered)
            {
                actList.Add(ConvertToActivity(row));
            }
            return actList;
        }

        public List<Activity> GetActivities(HashSet<string> ids, string appId, HashSet<String> fields, HashSet<String> activityIds)
        {
            var activityList = new List<ActivityRow>();
            using (var db = new AzureRayaDataContext())
            {
                foreach (var id in ids)
                {
                    if (string.IsNullOrEmpty(appId))
                    {
                        var activities = db.activities
                            .Where(x => x.PartitionKey == id);
                        foreach (var row in activities)
                        {
                            activityList.Add(row);
                        }
                    }
                    else
                    {
                        var activities = db.activities
                            .Where(x => x.PartitionKey == id && x.app_id == appId);
                        foreach (var row in activities)
                        {
                            activityList.Add(row);
                        }
                    }
                }
            }

            var ordered = activityList.OrderByDescending(x => x.id);
            List<Activity> actList = new List<Activity>();
            if (activityIds != null)
            {
                foreach (var row in ordered)
                {
                    if (activityIds.Contains(row.id))
                    {
                        actList.Add(ConvertToActivity(row));
                    }
                }
            }
            return actList;
        }

        private Activity ConvertToActivity(ActivityRow row)
        {
            var act = new Activity(row.id, row.person_id);
            act.streamTitle = "activities";
            act.title = row.title;
            act.body = row.body;
            act.postedTime = row.created;
            act.mediaItems = GetMediaItems(row.id);
            return act;
        }

        public bool DeleteActivities(string userId, string appId, HashSet<string> activityIds)
        {
            var activityList = new List<ActivityRow>();
            using (var db = new AzureRayaDataContext())
            {
                foreach (var id in activityIds)
                {
                    var res = db.activities
                        .Where(x => x.id == id &&
                                    x.PartitionKey == userId &&
                                    x.app_id == appId).SingleOrDefault();
                    if (res != null)
                    {
                        activityList.Add(res);
                    }
                }
                db.DeleteAllOnSubmit(activityList.AsQueryable());
                db.SubmitChanges();
            }
            
            return true;
        }

        public List<MediaItem> GetMediaItems(string activityId)
        {
            var media = new List<MediaItem>();
            using (var db = new AzureRayaDataContext())
            {
                var result = db.activityMediaItems
                    .Where(x => x.PartitionKey == activityId);
                foreach (var entry in result)
                {
                    media.Add(new MediaItem(entry.mime_type,
                                            (MediaItem.Type)Enum.Parse(typeof(MediaItem.Type),
                                                                       entry.media_type, true),
                                            entry.url));
                }
            }
            
            return media;
        }

        public HashSet<string> GetFriendIds(string personId)
        {
            HashSet<string> ids = new HashSet<string>();
            using (var db = new AzureRayaDataContext())
            {
                var result =
                    db.friends.Where(x => x.PartitionKey == personId ||
                                          x.friend_id == personId);

                foreach (var entry in result)
                {
                    string id = (entry.person_id == personId) ? entry.friend_id : entry.person_id;
                    ids.Add(id);
                }
            }
            
            return ids;
        }

        public bool SetAppData(string personId, string key, string value, string appId)
        {
            string rowKey = string.Concat(personId, "-", appId, "-", key);
            using (var db = new AzureRayaDataContext())
            {
                if (string.IsNullOrEmpty(value))
                {
                    // empty key kind of became to mean "delete data" (was an old orkut hack that became part of the spec spec)
                    var ret = new ApplicationSettingRow(personId, rowKey)
                                  {
                                      application_id = appId,
                                      person_id = personId,
                                      name = key
                                  };
                    db.DeleteOnSubmit(ret);
                }
                else
                {
                    var ret = db.applicationSettings
                        .Where(x => x.RowKey == rowKey && x.PartitionKey == personId).SingleOrDefault();
                    if (ret == null)
                    {
                        ret = new ApplicationSettingRow(personId, rowKey)
                                  {
                                      application_id = appId,
                                      person_id = personId,
                                      name = key,
                                      value = value
                                  };
                        db.InsertOnSubmit(AzureRayaDataContext.TableNames.applicationSettings, ret);
                    }
                    else
                    {
                        ret.value = value;
                        db.UpdateOnSubmit(ret);
                    }
                }
                db.SubmitChanges();
            }
            
            return true;
        }

        public bool DeleteAppData(string personId, HashSet<string> keys, string appId)
        {
            using (var db = new AzureRayaDataContext())
            {
                if (keys.Count == 0)
                {
                    var ret = db.applicationSettings
                        .Where(x => x.application_id == appId &&
                                    x.PartitionKey == personId);
                    db.DeleteAllOnSubmit(ret);
                }
                else
                {
                    foreach (var key in keys)
                    {
                        var ret = db.applicationSettings
                            .Where(x => x.application_id == appId &&
                                        x.PartitionKey == personId &&
                                        x.name == key);
                        db.DeleteOnSubmit(ret);
                    }
                }
                db.SubmitChanges();
            }
            return true;
        }

        public DataCollection GetAppData(HashSet<String> ids, HashSet<String> keys, String appId)
        {
            var data = new Dictionary<string, Dictionary<string, string>>();
            using (var db = new AzureRayaDataContext())
            {
                foreach (var id in ids)
                {
                    if (keys.Count == 0)
                    {
                        IQueryable<ApplicationSettingRow> result;
                        if (!string.IsNullOrEmpty(appId))
                        {
                            result = db.applicationSettings
                                .Where(x => x.PartitionKey == id && x.application_id == appId);
                        }
                        else
                        {
                            result = db.applicationSettings
                                .Where(x => x.PartitionKey == id);
                        }

                        foreach (var row in result)
                        {
                            if (!data.ContainsKey(row.person_id))
                            {
                                data.Add(row.person_id, new Dictionary<string, string>());
                            }
                            data[row.person_id].Add(row.name, row.value);
                        }
                    }
                    else
                    {
                        foreach (var key in keys)
                        {
                            IQueryable<ApplicationSettingRow> result;
                            if (!String.IsNullOrEmpty(appId))
                            {
                                result = db.applicationSettings
                                    .Where(x => x.PartitionKey == id &&
                                                x.application_id == appId &&
                                                x.name == key);
                            }
                            else
                            {
                                result = db.applicationSettings
                                    .Where(x => x.PartitionKey == id &&
                                                x.name == key);
                            }
                            foreach (var row in result)
                            {
                                if (!data.ContainsKey(row.person_id))
                                {
                                    data.Add(row.person_id, new Dictionary<string, string>());
                                }
                                data[row.person_id].Add(row.name, row.value);
                            }
                        }
                    }
                }
            }
            return new DataCollection(data);
        }

        public Dictionary<string, Person> GetPeople(HashSet<String> ids, HashSet<String> fields, CollectionOptions options)
        {
            var result = new Dictionary<string, Person>();
            using (var db = new AzureRayaDataContext())
            {
                // TODO filter first then fill dictionary
                foreach (var id in ids)
                {
                    var p = db.persons.Where(x => x.PartitionKey == id).SingleOrDefault();
                    if (p == null)
                    {
                        continue;
                    }
                    string personId = p.id;
                    var name = new Name();
                    var person = new Person();

                    name.givenName = p.first_name;
                    name.familyName = p.last_name;
                    name.formatted = p.first_name + " " + p.last_name;
                    person.displayName = name.formatted;
                    person.name = name;
                    person.id = personId;
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
                        var activities = db.activities.Where(a => a.PartitionKey == personId);
                        person.activities = new List<string>();
                        foreach (var act in activities)
                        {
                            person.activities.Add(act.title);
                        }
                    }

                    if (fields.Contains("addresses") || fields.Contains("@all"))
                    {
                        var personAddresses = db.addressesPerson
                            .Where(x => x.PartitionKey == personId);
                        List<Address> addresses = new List<Address>();
                        foreach (var row in personAddresses)
                        {
                            if (String.IsNullOrEmpty(row.unstructured_address))
                            {
                                row.unstructured_address = (row.street_address + " " + row.region + " " + row.country).Trim();
                            }
                            var addr = new Address(row.unstructured_address);
                            addr.country = row.country;
                            addr.latitude = row.latitude;
                            addr.longitude = row.longitude;
                            addr.locality = row.locality;
                            addr.postalCode = row.postal_code;
                            addr.region = row.region;
                            addr.streetAddress = row.street_address;
                            addr.type = row.address_type;
                            //FIXME quick and dirty hack to demo PC
                            addr.primary = true;
                            addresses.Add(addr);
                        }
                        person.addresses = addresses;
                    }

                    if (fields.Contains("bodyType") || fields.Contains("@all"))
                    {
                        var row = db.personBodyTypes.Where(x => x.PartitionKey == personId).SingleOrDefault();
                        if (row != null)
                        {
                            BodyType bodyType = new BodyType();
                            bodyType.build = row.build;
                            bodyType.eyeColor = row.eye_color;
                            bodyType.hairColor = row.hair_color;
                            bodyType.height = row.height;
                            bodyType.weight = row.weight;
                            person.bodyType = bodyType;
                        }
                    }

                    if (fields.Contains("books") || fields.Contains("@all"))
                    {
                        var books = db.personBooks.Where(x => x.PartitionKey == personId);
                        var bookList = new List<string>();
                        foreach (var book in books)
                        {
                            bookList.Add(book.book);
                        }
                        person.books = bookList;
                    }

                    if (fields.Contains("cars") || fields.Contains("@all"))
                    {
                        var cars = db.personCars.Where(x => x.PartitionKey == personId);
                        var carList = new List<string>();
                        foreach (var car in cars)
                        {
                            carList.Add(car.car);
                        }
                        person.cars = carList;
                    }

                    if (fields.Contains("currentLocation") || fields.Contains("@all"))
                    {
                        var row = db.personCurrentLocations
                            .Where(x => x.PartitionKey == personId).SingleOrDefault();
                        if (row != null)
                        {
                            if (string.IsNullOrEmpty(row.unstructured_address))
                            {
                                row.unstructured_address = (row.street_address + " " + row.region + " " + row.country).Trim();
                            }
                            var addr = new Address(row.unstructured_address);
                            addr.country = row.country;
                            addr.latitude = row.latitude;
                            addr.longitude = row.longitude;
                            addr.locality = row.locality;
                            addr.postalCode = row.postal_code;
                            addr.region = row.region;
                            addr.streetAddress = row.street_address;
                            addr.type = row.address_type;
                            person.currentLocation = addr;
                        }
                    }

                    if (fields.Contains("emails") || fields.Contains("@all"))
                    {
                        var emails = db.personEmails.Where(x => x.PartitionKey == personId);
                        List<ListField> emailList = new List<ListField>();
                        foreach (var email in emails)
                        {
                            emailList.Add(new ListField(email.email_type, email.address)); // TODO: better email canonicalization; remove dups
                        }
                        person.emails = emailList;
                    }

                    if (fields.Contains("food") || fields.Contains("@all"))
                    {
                        var foods = db.personFoods.Where(x => x.PartitionKey == personId);
                        var foodList = new List<string>();
                        foreach (var food in foods)
                        {
                            foodList.Add(food.food);
                        }
                        person.food = foodList;
                    }

                    if (fields.Contains("heroes") || fields.Contains("@all"))
                    {
                        var heroes = db.personHeroes.Where(x => x.PartitionKey == personId);
                        var heroList = new List<string>();
                        foreach (var hero in heroes)
                        {
                            heroList.Add(hero.hero);
                        }
                        person.heroes = heroList;
                    }

                    if (fields.Contains("interests") || fields.Contains("@all"))
                    {
                        var interests = db.personInterests.Where(x => x.PartitionKey == personId);
                        var interestList = new List<string>();
                        foreach (var interest in interests)
                        {
                            interestList.Add(interest.interest);
                        }
                        person.interests = interestList;
                    }
                    List<Organization> organizations = new List<Organization>();
                    bool fetchedOrg = false;
                    if (fields.Contains("jobs") || fields.Contains("@all"))
                    {
                        var jobs = db.personJobs
                            .Where(x => x.PartitionKey == personId);
                        foreach (var job in jobs)
                        {
                            var organization = new Organization();
                            organization.description = job.description;
                            if (job.end_date.HasValue)
                                organization.endDate = UnixTime.ToDateTime(job.end_date.Value);
                            organization.field = job.field;
                            organization.name = job.name;
                            organization.salary = job.salary;
                            if (job.start_date.HasValue)
                                organization.startDate = UnixTime.ToDateTime(job.start_date.Value);
                            organization.subField = job.sub_field;
                            organization.title = job.title;
                            organization.webpage = job.webpage;
                            organization.type = "job";
                            if (!string.IsNullOrEmpty(job.id))
                            {
                                var addresses = db.addressesOrganization.Where(x => x.organization_id == job.id).Single();
                                if (string.IsNullOrEmpty(addresses.unstructured_address))
                                {
                                    addresses.unstructured_address = (addresses.street_address + " " + addresses.region + " " + addresses.country).Trim();
                                }
                                var addr = new Address(addresses.unstructured_address);
                                addr.country = addresses.country;
                                addr.latitude = addresses.latitude;
                                addr.longitude = addresses.longitude;
                                addr.locality = addresses.locality;
                                addr.postalCode = addresses.postal_code;
                                addr.region = addresses.region;
                                addr.streetAddress = addresses.street_address;
                                addr.type = addresses.address_type;
                                organization.address = addr;
                            }
                            organizations.Add(organization);
                        }
                        fetchedOrg = true;
                    }

                    if (fields.Contains("schools") || fields.Contains("@all"))
                    {
                        var schools = db.personSchools
                            .Where(x => x.PartitionKey == personId);
                        foreach (var school in schools)
                        {
                            var organization = new Organization();
                            organization.description = school.description;
                            if (school.end_date.HasValue)
                                organization.endDate = UnixTime.ToDateTime(school.end_date.Value);
                            organization.field = school.field;
                            organization.name = school.name;
                            organization.salary = school.salary;
                            if (school.start_date.HasValue)
                                organization.startDate = UnixTime.ToDateTime(school.start_date.Value);
                            organization.subField = school.sub_field;
                            organization.title = school.title;
                            organization.webpage = school.webpage;
                            organization.type = "school";
                            if (!string.IsNullOrEmpty(school.id))
                            {
                                var res3 = db.addressesOrganization.Where(x => x.organization_id == school.id).Single();
                                if (string.IsNullOrEmpty(res3.unstructured_address))
                                {
                                    res3.unstructured_address = (res3.street_address + " " + res3.region + " " + res3.country).Trim();
                                }
                                var addres = new Address(res3.unstructured_address);
                                addres.country = res3.country;
                                addres.latitude = res3.latitude;
                                addres.longitude = res3.longitude;
                                addres.locality = res3.locality;
                                addres.postalCode = res3.postal_code;
                                addres.region = res3.region;
                                addres.streetAddress = res3.street_address;
                                addres.type = res3.address_type;
                                organization.address = addres;
                            }
                            organizations.Add(organization);
                        }
                        fetchedOrg = true;
                    }
                    if (fetchedOrg)
                    {
                        person.organizations = organizations;
                    }
                    //TODO languagesSpoken, currently missing the languages / countries tables so can"t do this yet

                    if (fields.Contains("movies") || fields.Contains("@all"))
                    {
                        var movies = db.personMovies.Where(x => x.PartitionKey == personId);
                        var movieList = new List<string>();
                        foreach (var movie in movies)
                        {
                            movieList.Add(movie.movie);
                        }
                        person.movies = movieList;
                    }
                    if (fields.Contains("music") || fields.Contains("@all"))
                    {
                        var musics = db.personMusics.Where(x => x.PartitionKey == personId);
                        var musicList = new List<string>();
                        foreach (var music in musics)
                        {
                            musicList.Add(music.music);
                        }
                        person.music = musicList;
                    }
                    if (fields.Contains("phoneNumbers") || fields.Contains("@all"))
                    {
                        List<ListField> numList = new List<ListField>();
                        var numbers = db.personPhoneNumbers.Where(x => x.PartitionKey == personId);
                        foreach (var number in numbers)
                        {
                            numList.Add(new ListField(number.number_type, number.number));
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
                    /*
                    if (fields.Contains("quotes") || fields.Contains("@all"))
                    {
                        var _strings = db.person_quotes.Where(x => x.person_id == personId).Select(x => x.quote);
                        person.quotes = _strings.ToList();
                    }
                    if (fields.Contains("sports") || fields.Contains("@all"))
                    {
                        var _strings = db.person_sports.Where(x => x.person_id == personId).Select(x => x.sport);
                        person.sports = _strings.ToList();
                    }
                
                    if (fields.Contains("tags") || fields.Contains("@all"))
                    {
                        var _strings = db.person_tags.Where(x => x.person_id == personId).Select(x => x.tag);
                        person.tags = _strings.ToList();
                    }

                    if (fields.Contains("turnOns") || fields.Contains("@all"))
                    {
                        var _strings = db.person_turn_ons.Where(x => x.person_id == personId).Select(x => x.turn_on);
                        person.turnOns = _strings.ToList();
                    }
                    if (fields.Contains("turnOffs") || fields.Contains("@all"))
                    {
                        var _strings = db.person_turn_offs.Where(x => x.person_id == personId).Select(x => x.turn_off);
                        person.turnOffs = _strings.ToList();
                    }
                     * */
                    if (fields.Contains("urls") || fields.Contains("@all"))
                    {
                        var urls = db.personUrls.Where(x => x.PartitionKey == personId);
                        List<ListField> urllist = new List<ListField>();
                        foreach (var u in urls)
                        {
                            var url = new Url(u.url, null, null);
                            urllist.Add(url);
                        }
                        //urllist.Add(new Url(urlPrefix + "/profile/" + personId, null, "profile"));
                        person.urls = urllist;
                    }

                    result.Add(personId, person);
                } // foreach
            }

            return result;
        }
    }
#endif
}