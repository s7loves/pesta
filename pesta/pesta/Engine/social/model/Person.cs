#region License, Terms and Conditions
/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License. You may obtain [DataMember(EmitDefaultValue = false)] copy of the License at
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
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Pesta.Engine.protocol.conversion;
using Pesta.Utilities;

namespace Pesta.Engine.social.model
{
    [XmlRoot(ElementName = "person", Namespace = BeanConverter.osNameSpace)]
    [DataContract(Name = "person", Namespace = BeanConverter.osNameSpace)]
    public class Person
    {
        public static readonly String PROFILE_URL_TYPE = "profile";

        public static readonly String THUMBNAIL_PHOTO_TYPE = "thumbnail";

        public enum Gender
        {
            female, male
        }
        public Person()
        {
        }

        public Person(String id, String displayName, Name name)
        {
            this.id = id;
            this.displayName = displayName;
            this.name = name;
        }

        [DataMember(EmitDefaultValue = false)]
        public String aboutMe { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<Account> accounts { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> activities { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<Address> addresses { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? age { get; set; }
        [XmlIgnore]
        public bool ageSpecified { get { return age.HasValue; } }

        public DateTime? birthday { get; set; }
        [XmlIgnore]
        public bool birthdaySpecified { get { return birthday.HasValue; } }
        [DataMember(Name = "birthday", EmitDefaultValue = false)]
        private string _birthday { get { return birthday.HasValue ? birthday.Value.ToUniversalTime().ToString("s") : null; } set { birthday = DateTime.Parse(value); } }
        
        
        [DataMember(EmitDefaultValue = false)]
        public BodyType bodyType { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> books { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> cars { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String children { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public Address currentLocation { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public Drinker? drinker { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String displayName { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<ListField> emails { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String ethnicity { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String fashion { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> food { get; set; }

        [XmlIgnore]
        public bool genderSpecified { get { return gender.HasValue; } }
        public Gender? gender { get; set; }
        [DataMember(Name = "gender", EmitDefaultValue = false)]
        public string _gender 
        { 
            get
            {
                return gender.HasValue ? gender.ToString() : null;
            } 
            set
            {
                if (value != null)
                {
                    gender = (Gender)Enum.Parse(typeof(Gender), value, true);
                }
            } 
        }
        
        [DataMember(EmitDefaultValue = false)]
        public String happiestWhen { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public bool? hasApp { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> heroes { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String humor { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String id { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<ListField> ims { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> interests { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String jobInterests { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> languagesSpoken { get; set; }

        public DateTime? updated { get; set; }
        [DataMember(Name = "updated", EmitDefaultValue = false)]
        private string _updated { get { return updated.HasValue ? DateUtil.ToString(updated.Value.ToUniversalTime()) : null; } set { updated = DateUtil.Parse(value); } }
        [XmlIgnore]
        public bool updatedSpecified { get { return updated.HasValue; } }

        [DataMember(EmitDefaultValue = false)]
        public String livingArrangement { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<LookingFor> lookingFor { get; set; }

        public int? _lookingFor
        {
            get
            {
                if (lookingFor == null)
                {
                    return null;
                }
                int result = 0;
                foreach (var l in lookingFor)
                {
                    result |= (int)l;
                }
                return result;
            } 
            set
            {
                if (value == null)
                {
                    return;
                }
                lookingFor = new List<LookingFor>();
                if ((value & (int)LookingFor.ACTIVITY_PARTNERS) != 0)
                {
                    lookingFor.Add(LookingFor.ACTIVITY_PARTNERS);
                }
                if ((value & (int)LookingFor.DATING) != 0)
                {
                    lookingFor.Add(LookingFor.DATING);
                }
                if ((value & (int)LookingFor.FRIENDS) != 0)
                {
                    lookingFor.Add(LookingFor.FRIENDS);
                }
                if ((value & (int)LookingFor.NETWORKING) != 0)
                {
                    lookingFor.Add(LookingFor.NETWORKING);
                }
                if ((value & (int)LookingFor.RANDOM) != 0)
                {
                    lookingFor.Add(LookingFor.RANDOM);
                }
                if ((value & (int)LookingFor.RELATIONSHIP) != 0)
                {
                    lookingFor.Add(LookingFor.RELATIONSHIP);
                }
            }
        }
        [DataMember(EmitDefaultValue = false)]
        public List<String> movies { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> music { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public Name name { get; set; }
        public NetworkPresence? networkPresence { get; set; }
        [DataMember(Name = "networkPresence", EmitDefaultValue = false)]
        public string _networkPresence
        {
            get
            {
                return networkPresence.HasValue ? networkPresence.ToString() : null;
            }
            set
            {
                if (value != null)
                {
                    networkPresence = (NetworkPresence)Enum.Parse(typeof(NetworkPresence), value, true);
                }
            }
        }
        [DataMember(EmitDefaultValue = false)]
        public String nickname { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<Organization> organizations { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String pets { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<ListField> phoneNumbers { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<ListField> photos { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String politicalViews { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public Url profileSong { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public Url profileVideo { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> quotes { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String relationshipStatus { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String religion { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String romance { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String scaredOf { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String sexualOrientation { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public Smoker? smoker { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> sports { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String status { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> tags { get; set; }
        
        [DataMember(EmitDefaultValue = false)]
        public long? utcOffset { get; set; }
        [XmlIgnore]
        public bool utcOffsetSpecified { get { return utcOffset.HasValue; } }

        [DataMember(EmitDefaultValue = false)]
        public List<String> turnOffs { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> turnOns { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<String> tvShows { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<ListField> urls { get; set; }

        // Note: Not in the opensocial js person object directly
        [DataMember(EmitDefaultValue = false)]
        public bool isOwner { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public bool isViewer { get; set; }

        

        [DataMember(EmitDefaultValue = false)]
        public string profileUrl
        {
            get
            {
                ListField url = getListFieldWithType(PROFILE_URL_TYPE, urls);
                return url == null ? null : url.value;
            }
            set
            {
                ListField url = getListFieldWithType(PROFILE_URL_TYPE, urls);
                if (url != null)
                {
                    url.value = value;
                }
                else
                {
                    urls = addListField(new Url(value, null, PROFILE_URL_TYPE), urls);
                }
            }
        }

        [DataMember(EmitDefaultValue = false)]
        public String thumbnailUrl
        {
            get
            {
                ListField photo = getListFieldWithType(THUMBNAIL_PHOTO_TYPE, photos);
                return photo == null ? null : photo.value;
            }
            set
            {
                ListField photo = getListFieldWithType(THUMBNAIL_PHOTO_TYPE, photos);
                if (photo != null)
                {
                    photo.value = value;
                }
                else
                {
                    photos = addListField(new ListField(THUMBNAIL_PHOTO_TYPE, value), photos);
                }
            }
        }

        public bool DoesLookForExist(LookingFor lkf)
        {
            if (lookingFor == null)
            {
                return false;
            }
            foreach (var entry in lookingFor)
            {
                if (entry == lkf)
                {
                    return true;
                }
            }
            return false;
        }

        private static ListField getListFieldWithType(String type, IEnumerable<ListField> list)
        {
            if (list != null)
            {
                foreach (ListField url in list)
                {
                    if (type.ToLower().Equals(url.type))
                    {
                        return url;
                    }
                }
            }

            return null;
        }

        private static List<ListField> addListField(ListField field, List<ListField> list)
        {
            if (list == null)
            {
                list = new List<ListField>();
            }
            list.Add(field);
            return list;
        }

        public class Field : EnumBaseType<Field>
        {
            public Field(int key, string value)
                : base(key, value)
            {

            }
            public static readonly Field ABOUT_ME = new Field(1, "aboutMe");
            public static readonly Field ACCOUNTS = new Field(2, "accounts");
            public static readonly Field ACTIVITIES = new Field(3, "activities");
            public static readonly Field ADDRESSES = new Field(4, "addresses");
            public static readonly Field AGE = new Field(5, "age");
            public static readonly Field BODY_TYPE = new Field(6, "bodyType");
            public static readonly Field BOOKS = new Field(7, "books");
            public static readonly Field CARS = new Field(8, "cars");
            public static readonly Field CHILDREN = new Field(9, "children");
            public static readonly Field CURRENT_LOCATION = new Field(10, "currentLocation");

            public static readonly Field BIRTHDAY = new Field(11, "birthday");
            /** the json field for display name. */
            public static readonly Field DISPLAY_NAME = new Field(12, "displayName");
            /** Needed to support the RESTful api. */
            /** the json field for drinker. */
            public static readonly Field DRINKER = new Field(13, "drinker");
            /** the json field for emails. */
            public static readonly Field EMAILS = new Field(14, "emails");
            /** the json field for ethnicity. */
            public static readonly Field ETHNICITY = new Field(15, "ethnicity");
            /** the json field for fashion. */
            public static readonly Field FASHION = new Field(16, "fashion");
            /** the json field for food. */
            public static readonly Field FOOD = new Field(17, "food");
            /** the json field for gender. */
            public static readonly Field GENDER = new Field(18, "gender");
            /** the json field for happiestWhen. */
            public static readonly Field HAPPIEST_WHEN = new Field(19, "happiestWhen");
            /** the json field for hasApp. */
            public static readonly Field HAS_APP = new Field(20, "hasApp");
            /** the json field for heroes. */
            public static readonly Field HEROES = new Field(21, "heroes");
            /** the json field for humor. */
            public static readonly Field HUMOR = new Field(22, "humor");
            /** the json field for id. */
            public static readonly Field ID = new Field(23, "id");
            /** the json field for IM accounts. */
            public static readonly Field IMS = new Field(24, "ims");
            /** the json field for interests. */
            public static readonly Field INTERESTS = new Field(25, "interests");
            /** the json field for jobInterests. */
            public static readonly Field JOB_INTERESTS = new Field(26, "jobInterests");
            /** the json field for languagesSpoken. */
            public static readonly Field LANGUAGES_SPOKEN = new Field(27, "languagesSpoken");
            /** the json field for updated. */
            public static readonly Field LAST_UPDATED = new Field(28, "updated"); /** Needed to support the RESTful api. */
            /** the json field for livingArrangement. */
            public static readonly Field LIVING_ARRANGEMENT = new Field(29, "livingArrangement");
            /** the json field for lookingFor. */
            public static readonly Field LOOKING_FOR = new Field(30, "lookingFor");
            /** the json field for movies. */
            public static readonly Field MOVIES = new Field(31, "movies");
            /** the json field for music. */
            public static readonly Field MUSIC = new Field(32, "music");
            /** the json field for name. */
            public static readonly Field NAME = new Field(33, "name");
            /** the json field for networkPresence. */
            public static readonly Field NETWORKPRESENCE = new Field(34, "networkPresence");
            /** the json field for nickname. */
            public static readonly Field NICKNAME = new Field(35, "nickname");
            /** the json field for organiztions. */
            public static readonly Field ORGANIZATIONS = new Field(36, "organizations");
            /** the json field for pets. */
            public static readonly Field PETS = new Field(37, "pets");
            /** the json field for phoneNumbers. */
            public static readonly Field PHONE_NUMBERS = new Field(38, "phoneNumbers");
            /** the json field for photos. */
            public static readonly Field PHOTOS = new Field(39, "photos");
            /** the json field for politicalViews. */
            public static readonly Field POLITICAL_VIEWS = new Field(40, "politicalViews");
            /** the json field for profileSong. */
            public static readonly Field PROFILE_SONG = new Field(41, "profileSong");
            /** the json field for profileUrl. */
            public static readonly Field PROFILE_URL = new Field(42, "profileUrl");
            /** the json field for profileVideo. */
            public static readonly Field PROFILE_VIDEO = new Field(43, "profileVideo");
            /** the json field for quotes. */
            public static readonly Field QUOTES = new Field(44, "quotes");
            /** the json field for relationshipStatus. */
            public static readonly Field RELATIONSHIP_STATUS = new Field(45, "relationshipStatus");
            /** the json field for religion. */
            public static readonly Field RELIGION = new Field(46, "religion");
            /** the json field for romance. */
            public static readonly Field ROMANCE = new Field(47, "romance");
            /** the json field for scaredOf. */
            public static readonly Field SCARED_OF = new Field(48, "scaredOf");
            /** the json field for sexualOrientation. */
            public static readonly Field SEXUAL_ORIENTATION = new Field(49, "sexualOrientation");
            /** the json field for smoker. */
            public static readonly Field SMOKER = new Field(50, "smoker");
            /** the json field for sports. */
            public static readonly Field SPORTS = new Field(51, "sports");
            /** the json field for status. */
            public static readonly Field STATUS = new Field(52, "status");
            /** the json field for tags. */
            public static readonly Field TAGS = new Field(53, "tags");
            /** the json field for thumbnailUrl. */
            public static readonly Field THUMBNAIL_URL = new Field(54, "thumbnailUrl");
            /** the json field for utcOffset. */
            public static readonly Field UTC_OFFSET = new Field(55, "utcOffset");
            /** the json field for turnOffs. */
            public static readonly Field TURN_OFFS = new Field(56, "turnOffs");
            /** the json field for turnOns. */
            public static readonly Field TURN_ONS = new Field(57, "turnOns");
            /** the json field for tvShows. */
            public static readonly Field TV_SHOWS = new Field(58, "tvShows");
            /** the json field for urls. */
            public static readonly Field URLS = new Field(59, "urls");

            /**
             * The set of required fields.
             */
            public static readonly HashSet<String> DEFAULT_FIELDS = new HashSet<string>
                                                                        {ID.ToString(),
            NAME.ToString(),THUMBNAIL_URL.ToString()};

            /**
             * The set of all fields.
             */
            public static readonly ReadOnlyCollection<Field> ALL_FIELDS = GetBaseValues();

            /**
             * [DataMember(EmitDefaultValue = false)] Map to convert json string to Field representations.
             */
            private static Dictionary<String, Field> URL_STRING_TO_FIELD_MAP;

            public static Field GetByValue(string value)
            {
                return GetBaseByValue(value);
            }

            public override String ToString()
            {
                return Value;
            }

            /**
             * Converts from [DataMember(EmitDefaultValue = false)] url string (usually passed in the fields= parameter) into the
             * corresponding field enum.
             * @param urlString The string to translate.
             * @return The corresponding person field.
             */
            public static Field fromUrlString(String urlString)
            {
                if (URL_STRING_TO_FIELD_MAP == null)
                {
                    URL_STRING_TO_FIELD_MAP = new Dictionary<string, Field>();
                    foreach (Field field in GetBaseValues())
                    {
                        URL_STRING_TO_FIELD_MAP.Add(field.ToString(), field);
                    }
                }
                return URL_STRING_TO_FIELD_MAP[urlString];
            }
        }

    } 
}
