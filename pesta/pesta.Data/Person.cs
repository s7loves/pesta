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
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using pesta.Data.Model.Helpers;

namespace pesta.Data
{
    [XmlRoot(ElementName = "person", Namespace = Constants.osNameSpace)]
    [DataContract(Name = "person", Namespace = Constants.osNameSpace)]
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
#if AZURE
        public long fb_userid { get; set; }
        public string gfc_userid { get; set; }
        public string openid_userid { get; set; }
#endif
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

        [XmlIgnore]
        public bool drinkerSpecified { get { return drinker.HasValue; } }
        [DataMember(EmitDefaultValue = false)]
        public Drinker? drinker { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public String displayName { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<ListField> emails { get; set; } // first entry is primary email
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
                    gender = (Gender?)value.ToEnum<Gender>();
                }
            } 
        }
        
        [DataMember(EmitDefaultValue = false)]
        public String happiestWhen { get; set; }

        [XmlIgnore]
        public bool hasAppSpecified { get { return hasApp.HasValue; } }
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
        private string _updated { get { return updated.HasValue ? DateHelper.ToString(updated.Value.ToUniversalTime()) : null; } set { updated = DateHelper.Parse(value); } }
        [XmlIgnore]
        public bool updatedSpecified { get { return updated.HasValue; } }

        [DataMember(EmitDefaultValue = false)]
        public String livingArrangement { get; set; }
        [DataMember(EmitDefaultValue = false)]
        public List<LookingFor> lookingFor { get; set; }

        [XmlIgnore]
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

        [XmlIgnore]
        public bool networkPresenceSpecified { get { return smoker.HasValue; } }
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

        [XmlIgnore]
        public bool smokerSpecified { get { return smoker.HasValue; } }
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

        public enum Field 
        {
            [Description("aboutMe")]
            ABOUT_ME,
            [Description("accounts")]
            ACCOUNTS,
            [Description("activities")]
            ACTIVITIES,
            [Description("addresses")]
            ADDRESSES,
            [Description("age")]
            AGE,
            [Description("birthday")]
            BIRTHDAY,
            [Description("bodyType")]
            BODY_TYPE,
            [Description("books")]
            BOOKS,
            [Description("cars")]
            CARS,
            [Description("children")]
            CHILDREN,
            [Description("currentLocation")]
            CURRENT_LOCATION,
            [Description("displayName")]
            DISPLAY_NAME,
            [Description("drinker")]
            DRINKER,
            [Description("emails")]
            EMAILS,
            [Description("ethnicity")]
            ETHNICITY,
            [Description("fashion")]
            FASHION,
            [Description("food")]
            FOOD,
            [Description("gender")]
            GENDER,
            [Description("happiestWhen")]
            HAPPIEST_WHEN,
            [Description("hasApp")]
            HAS_APP,
            [Description("heroes")]
            HEROES,
            [Description("humor")]
            HUMOR,
            [Description("id")]
            ID,
            [Description("ims")]
            IMS,
            [Description("interests")]
            INTERESTS,
            [Description("jobInterests")]
            JOB_INTERESTS,
            [Description("languagesSpoken")]
            LANGUAGES_SPOKEN,
            [Description("updated")]
            LAST_UPDATED,
            [Description("livingArrangement")]
            LIVING_ARRANGEMENT,
            [Description("lookingFor")]
            LOOKING_FOR,
            [Description("movies")]
            MOVIES,
            [Description("music")]
            MUSIC,
            [Description("name")]
            NAME,
            [Description("networkPresence")]
            NETWORKPRESENCE,
            [Description("nickname")]
            NICKNAME,
            [Description("organizations")]
            ORGANIZATIONS,
            [Description("pets")]
            PETS,
            [Description("phoneNumbers")]
            PHONE_NUMBERS,
            [Description("photos")]
            PHOTOS,
            [Description("politicalViews")]
            POLITICAL_VIEWS,
            [Description("profileSong")]
            PROFILE_SONG,
            [Description("profileUrl")]
            PROFILE_URL,
            [Description("profileVideo")]
            PROFILE_VIDEO,
            [Description("quotes")]
            QUOTES,
            [Description("relationshipStatus")]
            RELATIONSHIP_STATUS,
            [Description("religion")]
            RELIGION,
            [Description("romance")]
            ROMANCE,
            [Description("scaredOf")]
            SCARED_OF,
            [Description("sexualOrientation")]
            SEXUAL_ORIENTATION,
            [Description("smoker")]
            SMOKER,
            [Description("sports")]
            SPORTS,
            [Description("status")]
            STATUS,
            [Description("tags")]
            TAGS,
            [Description("thumbnailUrl")]
            THUMBNAIL_URL,
            [Description("utcOffset")]
            UTC_OFFSET,
            [Description("turnOffs")]
            TURN_OFFS,
            [Description("turnOns")]
            TURN_ONS,
            [Description("tvShows")]
            TV_SHOWS,
            [Description("urls")]
            URLS
        }

        public static readonly HashSet<String> DEFAULT_FIELDS = new HashSet<string>
                                                                    {
                                                                        Field.ID.ToDescriptionString(),
                                                                        Field.NAME.ToDescriptionString(),
                                                                        Field.THUMBNAIL_URL.ToDescriptionString()
                                                                    };
    }
}