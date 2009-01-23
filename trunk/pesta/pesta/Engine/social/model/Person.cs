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
using System.Collections.ObjectModel;
using Pesta.Engine.social.core.model;
using Pesta.Utilities;

namespace Pesta.Engine.social.model
{
    /// <summary>
    /// Summary description for Person
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    [ImplementedBy(typeof(PersonImpl))]
    public abstract class Person
    {
        public class Field : EnumBaseType<Field>
        {
            /// <summary>
            /// Initializes a new instance of the Person class.
            /// </summary>
            /// <summary>
            /// Initializes a new instance of the Field class.
            /// </summary>
            public Field()
            {
            }
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
         * The json field that the instance represents.
         */
            private readonly String urlString;

            /**
             * The set of required fields.
             */
            public static readonly HashSet<String> DEFAULT_FIELDS = new HashSet<string>(){ID.ToString(),
            NAME.ToString(),THUMBNAIL_URL.ToString()};

            /**
             * The set of all fields.
             */
            public static readonly ReadOnlyCollection<Field> ALL_FIELDS = Field.GetBaseValues();

            /**
             * a Map to convert json string to Field representations.
             */
            private static Dictionary<String, Field> URL_STRING_TO_FIELD_MAP;

            /**
             * create a field base on the a json element.
             *
             * @param urlString the name of the element
             */
            private Field(String urlString)
            {
                this.urlString = urlString;
            }

            /**
             * emit the field as a json element.
             *
             * @return the field name
             */
            public override String ToString()
            {
                return this.urlString;
            }

            /**
             * Converts from a url string (usually passed in the fields= parameter) into the
             * corresponding field enum.
             * @param urlString The string to translate.
             * @return The corresponding person field.
             */
            public static Person.Field fromUrlString(String urlString)
            {
                if (URL_STRING_TO_FIELD_MAP == null)
                {
                    URL_STRING_TO_FIELD_MAP = new Dictionary<string, Field>();
                    foreach (Person.Field field in Person.Field.GetBaseValues())
                    {
                        URL_STRING_TO_FIELD_MAP.Add(field.ToString(), field);
                    }
                }
                return URL_STRING_TO_FIELD_MAP[urlString];
            }
        }

        /**
        * The type of a profile url when represented as a list field.
        */
        public static readonly String PROFILE_URL_TYPE = "profile";

        /**
        * The type of thumbnail photo types when represented as list fields.
        */
        public static readonly String THUMBNAIL_PHOTO_TYPE = "thumbnail";

        public abstract String getDisplayName();

        public abstract void setDisplayName(String displayName);

        /**
        * Enumeration of genders.
        */
        public enum Gender
        {
            female, male
        }


        /**
       * Get a general statement about the person, specified as a string. Container support for this
       * field is OPTIONAL.
       *
       * @return the value of aboutMe
       */
        public abstract String getAboutMe();

        /**
         * Set a general statement about the person, specified as a string. Container support for this
         * field is OPTIONAL.
         *
         * @param aboutMe the value of aboutMe
         */
        public abstract void setAboutMe(String aboutMe);

        /**
         * Get the list of online accounts held by this person.
         * @return a list of Account objects
         */
        public abstract List<Account> getAccounts();

        /**
         * Set the list of online accounts held by this person.
         * @param accounts a list of Account objects
         */
        public abstract void setAccounts(List<Account> accounts);

        /**
         * Get the person's favorite activities, specified as an List of strings. Container support for
         * this field is OPTIONAL.
         *
         * @return list of activities.
         */
        public abstract List<String> getActivities();

        /**
         * Set the person's favorite activities, specified as an List of strings.
         *
         * @param activities a list of activities
         */
        public abstract void setActivities(List<String> activities);

        /**
         * Get addresses associated with the person, specified as an List of Address objects. Container
         * support for this field is OPTIONAL.
         *
         * @return a List of address objects
         */
        public abstract List<Address> getAddresses();

        /**
         * Set addresses associated with the person, specified as an List of Address objects. Container
         * support for this field is OPTIONAL.
         *
         * @param addresses a list of address objects
         */
        public abstract void setAddresses(List<Address> addresses);

        /**
         * Get the person's age, specified as a number. Container support for this field is OPTIONAL.
         *
         * @return the persons age
         */
        public abstract int? getAge();

        /**
         * Set the person's age, specified as a number. Container support for this field is OPTIONAL.
         *
         * @param age the persons age
         */
        public abstract void setAge(int? age);

        /**
         * Get the person's body characteristics, specified as an BodyType. Container support for this
         * field is OPTIONAL.
         *
         * @return the BodyType
         */
        public abstract BodyType getBodyType();

        /**
         * Set the person's body characteristics, specified as an BodyType. Container support for this
         * field is OPTIONAL.
         *
         * @param bodyType the person's BodyType
         */
        public abstract void setBodyType(BodyType bodyType);

        /**
         * Get the person's favorite books, specified as an List of strings. Container support for this
         * field is OPTIONAL.
         *
         * @return list of books as strings
         */
        public abstract List<String> getBooks();

        /**
         * Set the person's favorite books, specified as an List of strings. Container support for this
         * field is OPTIONAL.
         *
         * @param books a list of the person's books
         */
        public abstract void setBooks(List<String> books);

        /**
         * Get the person's favorite cars, specified as an List of strings. Container support for this
         * field is OPTIONAL.
         *
         * @return the persons favorite cars
         */
        public abstract List<String> getCars();

        /**
         * Set the person's favorite cars, specified as an List of strings. Container support for this
         * field is OPTIONAL.
         *
         * @param cars a list of the persons favorite cars
         */
        public abstract void setCars(List<String> cars);

        /**
         * Get a description of the person's children, specified as a string. Container support for this
         * field is OPTIONAL.
         *
         * @return the persons children
         */
        public abstract String getChildren();

        /**
         * Set a description of the person's children, specified as a string. Container support for this
         * field is OPTIONAL.
         *
         * @param children the persons children
         */
        public abstract void setChildren(String children);

        /**
         * Get the person's current location, specified as an {@link Address}. Container support for this
         * field is OPTIONAL.
         *
         * @return the persons current location
         */
        public abstract Address getCurrentLocation();

        /**
         * Set the person's current location, specified as an {@link Address}. Container support for this
         * field is OPTIONAL.
         *
         * @param currentLocation the persons current location
         */
        public abstract void setCurrentLocation(Address currentLocation);

        /**
         * Get the person's date of birth, specified as a {@link Date} object. Container support for this
         * field is OPTIONAL.
         *
         * @return the person's data of birth
         */
        public abstract DateTime? getBirthday();

        /**
         * Set the person's date of birth, specified as a {@link Date} object. Container support for this
         * field is OPTIONAL.
         *
         * @param birthday the person's data of birth
         */
        public abstract void setBirthday(DateTime? birthday);

        /**
         * Get the person's drinking status, specified as an {@link Enum} with the enum's key referencing
         * {@link Enum.Drinker}. Container support for this field is OPTIONAL.
         *
         * @return the persons drinking status
         */
        public abstract EnumTypes.Drinker getDrinker();

        /**
         * Get the person's drinking status, specified as an {@link Enum} with the enum's key referencing
         * {@link Enum.Drinker}. Container support for this field is OPTIONAL.
         *
         * @param newDrinker the persons drinking status
         */
        public abstract void setDrinker(EnumTypes.Drinker newDrinker);

        /**
         * Get the person's Emails associated with the person.
         * Container support for this field is OPTIONAL.
         *
         * @return a list of the person's emails
         */
        public abstract List<ListField> getEmails();

        /**
         * Set the person's Emails associated with the person.
         * Container support for this field is OPTIONAL.
         *
         * @param emails a list of the person's emails
         */
        public abstract void setEmails(List<ListField> emails);

        /**
         * Get the person's ethnicity, specified as a string. Container support for this field is
         * OPTIONAL.
         *
         * @return the person's ethnicity
         */
        public abstract String getEthnicity();

        /**
         * Set the person's ethnicity, specified as a string. Container support for this field is
         * OPTIONAL.
         *
         * @param ethnicity the person's ethnicity
         */
        public abstract void setEthnicity(String ethnicity);

        /**
         * Get the person's thoughts on fashion, specified as a string. Container support for this field
         * is OPTIONAL.
         *
         * @return the person's thoughts on fashion
         */
        public abstract String getFashion();

        /**
         * Set the person's thoughts on fashion, specified as a string. Container support for this field
         * is OPTIONAL.
         *
         * @param fashion the person's thoughts on fashion
         */
        public abstract void setFashion(String fashion);

        /**
         * Get the person's favorite food, specified as an List of strings. Container support for this
         * field is OPTIONAL.
         *
         * @return the person's favorite food
         */
        public abstract List<String> getFood();

        /**
         * Set the person's favorite food, specified as an List of strings. Container support for this
         * field is OPTIONAL.
         *
         * @param food the person's favorite food
         */
        public abstract void setFood(List<String> food);

        /**
         * Get a person's gender, specified as an {@link Gender}.
         *
         * @return the person's gender
         */
        public abstract Gender? getGender();

        /**
         * Set a person's gender, specified as an {@link Gender}.
         *
         * @param newGender the person's gender
         */
        public abstract void setGender(Gender? newGender);

        /**
         * Get a description of when the person is happiest, specified as a string. Container support for
         * this field is OPTIONAL.
         *
         * @return a description of when the person is happiest
         */
        public abstract String getHappiestWhen();

        /**
         * Set a description of when the person is happiest, specified as a string. Container support for
         * this field is OPTIONAL.
         *
         * @param happiestWhen a description of when the person is happiest
         */
        public abstract void setHappiestWhen(String happiestWhen);

        /**
         * Get if the person has used the current app. Container support for this field is OPTIONAL.
         * Has app needs to take account of the context of the application that is performing the
         * query on this person object.
         * @return true the current app has been used
         */
        public abstract bool? getHasApp();

        /**
         * Set if the person has used the current app. Container support for this field is OPTIONAL.
         *
         * @param hasApp set true the current app has been used
         */
        public abstract void setHasApp(bool? hasApp);

        /**
         * Get a person's favorite heroes, specified as an Array of strings. Container support for this
         * field is OPTIONAL.
         *
         * @return a list of the person's favorite heroes
         */
        public abstract List<String> getHeroes();

        /**
         * Set a person's favorite heroes, specified as an Array of strings. Container support for this
         * field is OPTIONAL.
         *
         * @param heroes a list of the person's favorite heroes
         */
        public abstract void setHeroes(List<String> heroes);

        /**
         * Get the person's thoughts on humor, specified as a string. Container support for this field is
         * OPTIONAL.
         *
         * @return the person's thoughts on humor
         */
        public abstract String getHumor();

        /**
         * Set the person's thoughts on humor, specified as a string. Container support for this field is
         * OPTIONAL.
         *
         * @param humor the person's thoughts on humor
         */
        public abstract void setHumor(String humor);

        /**
         * Get A string ID that can be permanently associated with this person. Container support for this
         * field is REQUIRED.
         *
         * @return the permanent ID of the person
         */
        public abstract String getId();

        /**
         * Set A string ID that can be permanently associated with this person. Container support for this
         * field is REQUIRED.
         *
         * @param id the permanent ID of the person
         */
        public abstract void setId(String id);

        /**
         * Get a list of Instant messaging address for this Person. No official canonicalization rules
         * exist for all instant messaging addresses, but Service Providers SHOULD remove all whitespace
         * and convert the address to lowercase, if this is appropriate for the service this IM address is
         * used for. Instead of the standard Canonical Values for type, this field defines the following
         * Canonical Values to represent currently popular IM services: aim, gtalk, icq, xmpp, msn, skype,
         * qq, and yahoo.
         *
         * @return A list of IM addresses
         */
        public abstract List<ListField> getIms();

        /**
         * Set a list of Instant messaging address for this Person. No official canonicalization rules
         * exist for all instant messaging addresses, but Service Providers SHOULD remove all whitespace
         * and convert the address to lowercase, if this is appropriate for the service this IM address is
         * used for. Instead of the standard Canonical Values for type, this field defines the following
         * Canonical Values to represent currently popular IM services: aim, gtalk, icq, xmpp, msn, skype,
         * qq, and yahoo.
         *
         * @param ims a list ListFields representing IM addresses.
         */
        public abstract void setIms(List<ListField> ims);

        /**
         * Get the person's interests, hobbies or passions, specified as an List of strings. Container
         * support for this field is OPTIONAL.
         *
         * @return the person's interests, hobbies or passions
         */
        public abstract List<String> getInterests();

        /**
         * Set the person's interests, hobbies or passions, specified as an List of strings. Container
         * support for this field is OPTIONAL.
         *
         * @param interests the person's interests, hobbies or passions
         */
        public abstract void setInterests(List<String> interests);

        /**
         * Get the Person's favorite jobs, or job interests and skills, specified as a string. Container
         * support for this field is OPTIONAL
         *
         * @return the Person's favorite jobs, or job interests and skills
         */
        public abstract String getJobInterests();

        /**
         * Set the Person's favorite jobs, or job interests and skills, specified as a string. Container
         * support for this field is OPTIONAL
         *
         * @param jobInterests the Person's favorite jobs, or job interests and skills
         */
        public abstract void setJobInterests(String jobInterests);

        /**
         * Get a List of the languages that the person speaks as ISO 639-1 codes, specified as an List of
         * strings. Container support for this field is OPTIONAL.
         *
         * @return a List of the languages that the person speaks
         */
        public abstract List<String> getLanguagesSpoken();

        /**
         * Set a List of the languages that the person speaks as ISO 639-1 codes, specified as an List of
         * strings. Container support for this field is OPTIONAL.
         *
         * @param languagesSpoken a List of the languages that the person speaks
         */
        public abstract void setLanguagesSpoken(List<String> languagesSpoken);

        /**
         * The time this person was last updated.
         *
         * @return the last update time
         */
        public abstract DateTime? getUpdated();

        /**
         * Set the time this record was last updated.
         *
         * @param updated the last update time
         */
        public abstract void setUpdated(DateTime? updated);

        /**
         * Get a description of the person's living arrangement, specified as a string. Container support
         * for this field is OPTIONAL.
         *
         * @return a description of the person's living arrangement
         */
        public abstract String getLivingArrangement();

        /**
         * Set a description of the person's living arrangement, specified as a string. Container support
         * for this field is OPTIONAL.
         *
         * @param livingArrangement a description of the person's living arrangement
         */
        public abstract void setLivingArrangement(String livingArrangement);

        /**
         * Get a person's statement about who or what they are looking for, or what they are interested in
         * meeting people for. Specified as an List of {@link Enum} with the enum's key referencing
         * {@link Enum.LookingFor} Container support for this field is OPTIONAL.
         *
         * @return person's statement about who or what they are looking for
         */
        public abstract List<EnumTypes.LookingFor> getLookingFor();

        /**
         * Get a person's statement about who or what they are looking for, or what they are interested in
         * meeting people for. Specified as an List of {@link Enum} with the enum's key referencing
         * {@link Enum.LookingFor} Container support for this field is OPTIONAL.
         *
         * @param lookingFor person's statement about who or what they are looking for
         */
        public abstract void setLookingFor(List<EnumTypes.LookingFor> lookingFor);

        /**
         * Get the Person's favorite movies, specified as an List of strings. Container support for this
         * field is OPTIONAL.
         *
         * @return the Person's favorite movies
         */
        public abstract List<String> getMovies();

        /**
         * Set the Person's favorite movies, specified as an List of strings. Container support for this
         * field is OPTIONAL.
         *
         * @param movies the Person's favorite movies
         */
        public abstract void setMovies(List<String> movies);

        /**
         * Get the Person's favorite music, specified as an List of strings Container support for this
         * field is OPTIONAL.
         *
         * @return Person's favorite music
         */
        public abstract List<String> getMusic();

        /**
         * Set the Person's favorite music, specified as an List of strings Container support for this
         * field is OPTIONAL.
         *
         * @param music Person's favorite music
         */
        public abstract void setMusic(List<String> music);

        /**
         * Get the person's name Container support for this field is REQUIRED.
         *
         * @return the person's name
         */
        public abstract Name getName();

        /**
         * Set the person's name Container support for this field is REQUIRED.
         *
         * @param name the person's name
         */
        public abstract void setName(Name name);

        /**
         * Get the person's current network status. Specified as an {@link Enum} with the enum's key
         * referencing {@link Enum.Presence}. Container support for this field is OPTIONAL.
         *
         * @return the person's current network status
         */
        public abstract EnumTypes.NetworkPresence getNetworkPresence();

        /**
         * Set the person's current network status. Specified as an {@link Enum} with the enum's key
         * referencing {@link Enum.Presence}. Container support for this field is OPTIONAL.
         *
         * @param networkPresence the person's current network status
         */
        public abstract void setNetworkPresence(EnumTypes.NetworkPresence networkPresence);

        /**
         * Get the person's nickname. Container support for this field is REQUIRED.
         *
         * @return the person's nickname.
         */
        public abstract String getNickname();

        /**
         * Set the the person's nickname. Container support for this field is REQUIRED.
         *
         * @param nickname the person's nickname.
         */
        public abstract void setNickname(String nickname);

        /**
         * Get a list of current or past organizational affiliations of this Person.
         * @return a list of Organization objects
         */
        public abstract List<Organization> getOrganizations();

        /**
         * Set a list of current or past organizational affiliations of this Person.
         * @param organizations a list of Organisation objects
         */
        public abstract void setOrganizations(List<Organization> organizations);

        /**
         * Get a description of the person's pets Container support for this field is OPTIONAL.
         *
         * @return a description of the person's pets
         */
        public abstract String getPets();

        /**
         * Set a description of the person's pets Container support for this field is OPTIONAL.
         *
         * @param pets a description of the person's pets
         */
        public abstract void setPets(String pets);

        /**
         * Get the Phone numbers associated with the person.
         *
         * @return the Phone numbers associated with the person
         */
        public abstract List<ListField> getPhoneNumbers();

        /**
         * Set the Phone numbers associated with the person.
         *
         * @param phoneNumbers the Phone numbers associated with the person
         */
        public abstract void setPhoneNumbers(List<ListField> phoneNumbers);

        /**
         * URL of a photo of this person. The value SHOULD be a canonicalized URL, and MUST point to an
         * actual image file (e.g. a GIF, JPEG, or PNG image file) rather than to a web page containing an
         * image. Service Providers MAY return the same image at different sizes, though it is recognized
         * that no standard for describing images of various sizes currently exists. Note that this field
         * SHOULD NOT be used to send down arbitrary photos taken by this user, but specifically profile
         * photos of the contact suitable for display when describing the contact.
         *
         * @return a list of Photos
         */
        public abstract List<ListField> getPhotos();

        /**
         * Set a list of Photos for the person.
         * @see Person.getPhotos()
         * @param photos a list of photos.
         */
        public abstract void setPhotos(List<ListField> photos);

        /**
         * Get the Person's political views, specified as a string. Container support for this field is
         * OPTIONAL.
         *
         * @return the Person's political views
         */
        public abstract String getPoliticalViews();

        /**
         * Set the Person's political views, specified as a string. Container support for this field is
         * OPTIONAL.
         *
         * @param politicalViews the Person's political views
         */
        public abstract void setPoliticalViews(String politicalViews);

        /**
         * Get the Person's profile song, specified as an {@link Url}. Container support for this field
         * is OPTIONAL.
         *
         * @return the Person's profile song
         */
        public abstract Url getProfileSong();

        /**
         * Set the Person's profile song, specified as an {@link Url}. Container support for this field
         * is OPTIONAL.
         *
         * @param profileSong the Person's profile song
         */
        public abstract void setProfileSong(Url profileSong);

        /**
         * Get the Person's profile video. Container support for this field is OPTIONAL.
         *
         * @return the Person's profile video
         */
        public abstract Url getProfileVideo();

        /**
         * Set the Person's profile video. Container support for this field is OPTIONAL.
         *
         * @param profileVideo the Person's profile video
         */
        public abstract void setProfileVideo(Url profileVideo);

        /**
         * Get the person's favorite quotes Container support for this field is OPTIONAL.
         *
         * @return the person's favorite quotes
         */
        public abstract List<String> getQuotes();

        /**
         * Set the person's favorite quotes. Container support for this field is OPTIONAL.
         *
         * @param quotes the person's favorite quotes
         */
        public abstract void setQuotes(List<String> quotes);

        /**
         * Get the person's relationship status. Container support for this field is OPTIONAL.
         *
         * @return the person's relationship status
         */
        public abstract String getRelationshipStatus();

        /**
         * Set the person's relationship status. Container support for this field is OPTIONAL.
         *
         * @param relationshipStatus the person's relationship status
         */
        public abstract void setRelationshipStatus(String relationshipStatus);

        /**
         * Get the person's relgion or religious views. Container support for this field is OPTIONAL.
         *
         * @return the person's relgion or religious views
         */
        public abstract String getReligion();

        /**
         * Set the person's relgion or religious views. Container support for this field is OPTIONAL.
         *
         * @param religion the person's relgion or religious views
         */
        public abstract void setReligion(String religion);

        /**
         * Get the person's comments about romance. Container support for this field is OPTIONAL.
         *
         * @return the person's comments about romance,
         */
        public abstract String getRomance();

        /**
         * Set a the person's comments about romance, Container support for this field is OPTIONAL.
         *
         * @param romance the person's comments about romance,
         */
        public abstract void setRomance(String romance);

        /**
         * Get what the person is scared of Container support for this field is OPTIONAL.
         *
         * @return what the person is scared of
         */
        public abstract String getScaredOf();

        /**
         * Set what the person is scared of Container support for this field is OPTIONAL.
         *
         * @param scaredOf what the person is scared of
         */
        public abstract void setScaredOf(String scaredOf);

        /**
         * Get the person's sexual orientation. Container support for this field is OPTIONAL.
         *
         * @return the person's sexual orientation
         */
        public abstract String getSexualOrientation();

        /**
         * Set the person's sexual orientation Container support for this field is OPTIONAL.
         *
         * @param sexualOrientation the person's sexual orientation
         */
        public abstract void setSexualOrientation(String sexualOrientation);

        /**
         * Get the person's smoking status. Container support for this field is OPTIONAL.
         *
         * @return the person's smoking status
         */
        public abstract EnumTypes.Smoker getSmoker();

        /**
         * Set the person's smoking status. Container support for this field is OPTIONAL.
         *
         * @param newSmoker the person's smoking status
         */
        public abstract void setSmoker(EnumTypes.Smoker newSmoker);

        /**
         * Get the person's favorite sports. Container support for this field is OPTIONAL.
         *
         * @return the person's favorite sports
         */
        public abstract List<String> getSports();

        /**
         * Set the person's favorite sports. Container support for this field is OPTIONAL.
         *
         * @param sports the person's favorite sports
         */
        public abstract void setSports(List<String> sports);

        /**
         * Get the person's status, headline or shoutout. Container support for this field is OPTIONAL.
         *
         * @return the person's status, headline or shoutout
         */
        public abstract String getStatus();

        /**
         * Set the person's status, headline or shoutout. Container support for this field is OPTIONAL.
         *
         * @param status the person's status, headline or shoutout
         */
        public abstract void setStatus(String status);

        /**
         * Get arbitrary tags about the person. Container support for this field is OPTIONAL.
         *
         * @return arbitrary tags about the person.
         */
        public abstract List<String> getTags();

        /**
         * Set arbitrary tags about the person. Container support for this field is OPTIONAL.
         *
         * @param tags arbitrary tags about the person.
         */
        public abstract void setTags(List<String> tags);

        /**
         * Get the Person's time zone, specified as the difference in minutes between Greenwich Mean Time
         * (GMT) and the user's local time. Container support for this field is OPTIONAL.
         *
         * @return the Person's time zone
         */
        public abstract long? getUtcOffset();

        /**
         * Set the Person's time zone, specified as the difference in minutes between Greenwich Mean Time
         * (GMT) and the user's local time. Container support for this field is OPTIONAL.
         *
         * @param utcOffset the Person's time zone
         */
        public abstract void setUtcOffset(long? utcOffset);

        /**
         * Get the person's turn offs. Container support for this field is OPTIONAL.
         *
         * @return the person's turn offs
         */
        public abstract List<String> getTurnOffs();

        /**
         * Set the person's turn offs. Container support for this field is OPTIONAL.
         *
         * @param turnOffs the person's turn offs
         */
        public abstract void setTurnOffs(List<String> turnOffs);

        /**
         * Get the person's turn ons. Container support for this field is OPTIONAL.
         *
         * @return the person's turn ons
         */
        public abstract List<String> getTurnOns();

        /**
         * Set the person's turn ons. Container support for this field is OPTIONAL.
         *
         * @param turnOns the person's turn ons
         */
        public abstract void setTurnOns(List<String> turnOns);

        /**
         * Get the person's favorite TV shows. Container support for this field is OPTIONAL.
         *
         * @return the person's favorite TV shows.
         */
        public abstract List<String> getTvShows();

        /**
         * Set the person's favorite TV shows. Container support for this field is OPTIONAL.
         *
         * @param tvShows the person's favorite TV shows.
         */
        public abstract void setTvShows(List<String> tvShows);

        /**
         * Get the URLs related to the person, their webpages, or feeds Container support for this field
         * is OPTIONAL.
         *
         * @return the URLs related to the person, their webpages, or feeds
         */
        public abstract List<ListField> getUrls();

        /**
         * Set the URLs related to the person, their webpages, or feeds Container support for this field
         * is OPTIONAL.
         *
         * @param urls the URLs related to the person, their webpages, or feeds
         */
        public abstract void setUrls(List<ListField> urls);

        /**
         * @return true if this person object represents the owner of the current page.
         */
        public abstract bool? getIsOwner();

        /**
         * Set the owner flag.
         * @param isOwner the isOwnerflag
         */
        public abstract void setIsOwner(bool? isOwner);

        /**
         * Returns true if this person object represents the currently logged in user.
         * @return true if the person accessing this object is a viewer.
         */
        public abstract bool? getIsViewer();

        /**
         * Returns true if this person object represents the currently logged in user.
         * @param isViewer the isViewer Flag
         */
        public abstract void setIsViewer(bool? isViewer);


        // Proxied fields

        /**
         * Get the person's profile URL. This URL must be fully qualified. Relative URLs will not work in
         * gadgets. This field MUST be stored in the urls list with a type of "profile".
         *
         * Container support for this field is OPTIONAL.
         *
         * @return the person's profile URL
         */
        public abstract String getProfileUrl();

        /**
         * Set the person's profile URL. This URL must be fully qualified. Relative URLs will not work in
         * gadgets. This field MUST be stored in the urls list with a type of "profile".
         *
         * Container support for this field is OPTIONAL.
         *
         * @param profileUrl the person's profile URL
         */
        public abstract void setProfileUrl(String profileUrl);

        /**
         * Get the person's photo thumbnail URL, specified as a string. This URL must be fully qualified.
         * Relative URLs will not work in gadgets.
         * This field MUST be stored in the photos list with a type of "thumbnail".
         *
         * Container support for this field is OPTIONAL.
         *
         * @return the person's photo thumbnail URL
         */
        public abstract String getThumbnailUrl();

        /**
         * Set the person's photo thumbnail URL, specified as a string. This URL must be fully qualified.
         * Relative URLs will not work in gadgets.
         * This field MUST be stored in the photos list with a type of "thumbnail".
         *
         * Container support for this field is OPTIONAL.
         *
         * @param thumbnailUrl the person's photo thumbnail URL
         */
        public abstract void setThumbnailUrl(String thumbnailUrl);
    } 
}
