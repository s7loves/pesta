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

/// <summary>
/// Summary description for PersonImpl
/// </summary>
/// <remarks>
/// <para>
///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
/// </para>
/// </remarks>
public class PersonImpl : Person
{
    private String aboutMe;
    private List<Account> accounts;
    private List<String> activities;
    private List<Address> addresses;
    private int? age;
    private BodyType bodyType;
    private List<String> books;
    private List<String> cars;
    private String children;
    private Address currentLocation;
    private DateTime? birthday;
    private EnumTypes.Drinker drinker;
    private String displayName;
    private List<ListField> emails;
    private String ethnicity;
    private String fashion;
    private List<String> food;
    private Gender? gender;
    private String happiestWhen;
    private bool? hasApp;
    private List<String> heroes;
    private String humor;
    private String id;
    private List<ListField> ims;
    private List<String> interests;
    private String jobInterests;
    private List<String> languagesSpoken;
    private DateTime? updated;
    private String livingArrangement;
    private List<EnumTypes.LookingFor> lookingFor;
    private List<String> movies;
    private List<String> music;
    private Name name;
    private EnumTypes.NetworkPresence networkPresence;
    private String nickname;
    private List<Organization> organizations;
    private String pets;
    private List<ListField> phoneNumbers;
    private List<ListField> photos;
    private String politicalViews;
    private Url profileSong;
    private Url profileVideo;
    private List<String> quotes;
    private String relationshipStatus;
    private String religion;
    private String romance;
    private String scaredOf;
    private String sexualOrientation;
    private EnumTypes.Smoker smoker;
    private List<String> sports;
    private String status;
    private List<String> tags;
    private long? utcOffset;
    private List<String> turnOffs;
    private List<String> turnOns;
    private List<String> tvShows;
    private List<ListField> urls;

    // Note: Not in the opensocial js person object directly
    private bool? isOwner = false;
    private bool? isViewer = false;

    public PersonImpl() {
    }

    /**
    * A constructor which contains all of the required fields on a person object
    * @param id The id of the person
    * @param displayName The displayName of the person
    * @param name The person's name broken down into components
    */
    public PersonImpl(String id, String displayName, Name name) 
    {
        this.id = id;
        this.displayName = displayName;
        this.name = name;
    }

    override public String getAboutMe()
    {
        return aboutMe;
    }

    override public void setAboutMe(String aboutMe) 
    {
        this.aboutMe = aboutMe;
    }

    override public List<Account> getAccounts() 
    {
        return accounts;
    }

    override public void setAccounts(List<Account> accounts)
    {
        this.accounts = accounts;
    }

    override public List<String> getActivities()
    {
        return activities;
    }

    override public void setActivities(List<String> activities)
    {
        this.activities = activities;
    }

    override public List<Address> getAddresses()
    {
        return addresses;
    }

    override public void setAddresses(List<Address> addresses)
    {
        this.addresses = addresses;
    }

    override public int? getAge() 
    {
        return age;
    }

    override public void setAge(int? age) 
    {
        this.age = age;
    }

    override public BodyType getBodyType() 
    {
        return bodyType;
    }

    override public void setBodyType(BodyType bodyType)
    {
        this.bodyType = bodyType;
    }

    override public List<String> getBooks() 
    {
        return books;
    }

    override public void setBooks(List<String> books) 
    {
        this.books = books;
    }

    override public List<String> getCars()
    {
        return cars;
    }

    override public void setCars(List<String> cars)
    {
        this.cars = cars;
    }

    override public String getChildren()
    {
        return children;
    }

    override public void setChildren(String children) 
    {
        this.children = children;
    }

    override public Address getCurrentLocation() 
    {
        return currentLocation;
    }

    override public void setCurrentLocation(Address currentLocation)
    {
        this.currentLocation = currentLocation;
    }

    override public DateTime? getBirthday() 
    {
        if (birthday == null) 
        {
            return null;
        }
        return birthday;
    }

    override public void setBirthday(DateTime? birthday) 
    {
        if (birthday == null) 
        {
            this.birthday = null;
        } 
        else 
        {
            this.birthday = birthday;
        }
    }

    override public String getDisplayName() 
    {
        return displayName;
    }

    override public void setDisplayName(String displayName) 
    {
        this.displayName = displayName;
    }

    override public EnumTypes.Drinker getDrinker() 
    {
        return this.drinker;
    }

    override public void setDrinker(EnumTypes.Drinker newDrinker) 
    {
        this.drinker = newDrinker;
    }

    override public List<ListField> getEmails() 
    {
        return emails;
    }

    override public void setEmails(List<ListField> emails) 
    {
        this.emails = emails;
    }

    override public String getEthnicity() 
    {
        return ethnicity;
    }

    override public void setEthnicity(String ethnicity) 
    {
        this.ethnicity = ethnicity;
    }

    override public String getFashion() 
    {
        return fashion;
    }

    override public void setFashion(String fashion) 
    {
        this.fashion = fashion;
    }

    override public List<String> getFood() 
    {
        return food;
    }

    override public void setFood(List<String> food) 
    {
        this.food = food;
    }

    override public Gender? getGender() 
    {
        return gender;
    }

    override public void setGender(Gender? newGender) 
    {
        this.gender = newGender;
    }

    override public String getHappiestWhen() 
    {
        return happiestWhen;
    }

    override public void setHappiestWhen(String happiestWhen) 
    {
        this.happiestWhen = happiestWhen;
    }

    override public bool? getHasApp() 
    {
        return hasApp;
    }

    override public void setHasApp(bool? hasApp) 
    {
        this.hasApp = hasApp;
    }

    override public List<String> getHeroes() 
    {
        return heroes;
    }

    override public void setHeroes(List<String> heroes) 
    {
        this.heroes = heroes;
    }

    override public String getHumor() 
    {
        return humor;
    }

    override public void setHumor(String humor) 
    {
        this.humor = humor;
    }

    override public String getId() 
    {
        return id;
    }

    override public void setId(String id) 
    {
        this.id = id;
    }

    override public List<ListField> getIms() 
    {
        return ims;
    }

    override public void setIms(List<ListField> ims) 
    {
        this.ims = ims;
    }

    override public List<String> getInterests() 
    {
        return interests;
    }

    override public void setInterests(List<String> interests) 
    {
        this.interests = interests;
    }

    override public String getJobInterests() 
    {
        return jobInterests;
    }

    override public void setJobInterests(String jobInterests) 
    {
        this.jobInterests = jobInterests;
    }

    override public List<String> getLanguagesSpoken() 
    {
        return languagesSpoken;
    }

    override public void setLanguagesSpoken(List<String> languagesSpoken) 
    {
        this.languagesSpoken = languagesSpoken;
    }

    override public DateTime? getUpdated() 
    {
        if (updated == null) 
        {
            return null;
        }
        return updated;
    }

    override public void setUpdated(DateTime? updated)
    {
        if (updated == null)
        {
            this.updated = null;
        } 
        else 
        {
            this.updated = updated;
        }
    }

    override public String getLivingArrangement() 
    {
        return livingArrangement;
    }

    override public void setLivingArrangement(String livingArrangement) 
    {
        this.livingArrangement = livingArrangement;
    }

    override public List<EnumTypes.LookingFor> getLookingFor() 
    {
        return lookingFor;
    }

    override public void setLookingFor(List<EnumTypes.LookingFor> lookingFor)
    {
        this.lookingFor = lookingFor;
    }

    override public List<String> getMovies()
    {
        return movies;
    }

    override public void setMovies(List<String> movies) 
    {
        this.movies = movies;
    }

    override public List<String> getMusic() 
    {
        return music;
    }

    override public void setMusic(List<String> music) 
    {
        this.music = music;
    }

    override public Name getName()
    {
        return name;
    }

    override public void setName(Name name)
    {
        this.name = name;
    }

    override public EnumTypes.NetworkPresence getNetworkPresence() 
    {
        return networkPresence;
    }

    override public void setNetworkPresence(EnumTypes.NetworkPresence networkPresence)
    {
        this.networkPresence = networkPresence;
    }

    override public String getNickname()
    {
        return nickname;
    }

    override public void setNickname(String nickname) 
    {
        this.nickname = nickname;
    }

    override public List<Organization> getOrganizations() 
    {
        return organizations;
    }

    override public void setOrganizations(List<Organization> organizations) 
    {
        this.organizations = organizations;
    }

    override public String getPets()
    {
        return pets;
    }

    override public void setPets(String pets) 
    {
        this.pets = pets;
    }

    override public List<ListField> getPhoneNumbers()
    {
        return phoneNumbers;
    }

    override public void setPhoneNumbers(List<ListField> phoneNumbers) 
    {
        this.phoneNumbers = phoneNumbers;
    }

    override public List<ListField> getPhotos() 
    {
        return photos;
    }

    override public void setPhotos(List<ListField> photos)
    {
        this.photos = photos;
    }

    override public String getPoliticalViews() 
    {
        return politicalViews;
    }

    override public void setPoliticalViews(String politicalViews)
    {
        this.politicalViews = politicalViews;
    }

    override public Url getProfileSong() 
    {
        return profileSong;
    }

    override public void setProfileSong(Url profileSong) 
    {
        this.profileSong = profileSong;
    }

    override public Url getProfileVideo() 
    {
        return profileVideo;
    }

    override public void setProfileVideo(Url profileVideo)
    {
        this.profileVideo = profileVideo;
    }

    override public List<String> getQuotes()
    {
        return quotes;
    }

    override public void setQuotes(List<String> quotes) 
    {
        this.quotes = quotes;
    }

    override public String getRelationshipStatus()
    {
        return relationshipStatus;
    }

    override public void setRelationshipStatus(String relationshipStatus) 
    {
        this.relationshipStatus = relationshipStatus;
    }

    override public String getReligion()
    {
        return religion;
    }

    override public void setReligion(String religion) 
    {
        this.religion = religion;
    }

    override public String getRomance() 
    {
        return romance;
    }

    override public void setRomance(String romance) 
    {
        this.romance = romance;
    }

    override public String getScaredOf()
    {
        return scaredOf;
    }

    override public void setScaredOf(String scaredOf)
    {
        this.scaredOf = scaredOf;
    }

    override public String getSexualOrientation() 
    {
        return sexualOrientation;
    }

    override public void setSexualOrientation(String sexualOrientation) 
    {
        this.sexualOrientation = sexualOrientation;
    }

    override public EnumTypes.Smoker getSmoker() 
    {
        return this.smoker;
    }

    override public void setSmoker(EnumTypes.Smoker newSmoker)
    {
        this.smoker = newSmoker;
    }

    override public List<String> getSports()
    {
        return sports;
    }

    override public void setSports(List<String> sports) 
    {
        this.sports = sports;
    }

    override public String getStatus()
    {
        return status;
    }

    override public void setStatus(String status)
    {
        this.status = status;
    }

    override public List<String> getTags()
    {
        return tags;
    }

    override public void setTags(List<String> tags) 
    {
        this.tags = tags;
    }

    override public long? getUtcOffset() 
    {
        return utcOffset;
    }

    override public void setUtcOffset(long? utcOffset)
    {
        this.utcOffset = utcOffset;
    }

    override public List<String> getTurnOffs()
    {
        return turnOffs;
    }

    override public void setTurnOffs(List<String> turnOffs) 
    {
        this.turnOffs = turnOffs;
    }

    override public List<String> getTurnOns()
    {
        return turnOns;
    }

    override public void setTurnOns(List<String> turnOns) 
    {
        this.turnOns = turnOns;
    }

    override public List<String> getTvShows() 
    {
        return tvShows;
    }

    override public void setTvShows(List<String> tvShows)
    {
        this.tvShows = tvShows;
    }

    override public List<ListField> getUrls()
    {
        return urls;
    }

    override public void setUrls(List<ListField> urls) 
    {
        this.urls = urls;
    }

    override public bool? getIsOwner()
    {
        return isOwner;
    }

    override public void setIsOwner(bool? isOwner) 
    {
        this.isOwner = isOwner;
    }

    override public bool? getIsViewer()
    {
        return isViewer;
    }

    override public void setIsViewer(bool? isViewer)
    {
        this.isViewer = isViewer;
    }


    // Proxied fields

    override public String getProfileUrl()
    {
        ListField url = getListFieldWithType(PROFILE_URL_TYPE, getUrls());
        return url == null ? null : url.getValue();
    }

    override public void setProfileUrl(String profileUrl) 
    {
        ListField url = getListFieldWithType(PROFILE_URL_TYPE, getUrls());
        if (url != null) 
        {
            url.setValue(profileUrl);
        } 
        else 
        {
            setUrls(addListField(new UrlImpl(profileUrl, null, PROFILE_URL_TYPE), getUrls()));
        }
    }

    override public String getThumbnailUrl() 
    {
        ListField photo = getListFieldWithType(THUMBNAIL_PHOTO_TYPE, getPhotos());
        return photo == null ? null : photo.getValue();
    }

    override public void setThumbnailUrl(String thumbnailUrl)
    {
        ListField photo = getListFieldWithType(THUMBNAIL_PHOTO_TYPE, getPhotos());
        if (photo != null)
        {
            photo.setValue(thumbnailUrl);
        }
        else 
        {
            setPhotos(addListField(new ListFieldImpl(THUMBNAIL_PHOTO_TYPE, thumbnailUrl), getPhotos()));
        }
    }

    private ListField getListFieldWithType(String type, List<ListField> list) 
    {
        if (list != null) 
        {
            foreach (ListField url in list)
            {
                if (type.ToLower().Equals(url.getType()))
                {
                    return url;
                }
            }
        }

        return null;
    }

    private List<ListField> addListField(ListField field, List<ListField> list)
    {
        if (list == null)
        {
            list = new List<ListField>();
        }
        list.Add(field);
        return list;
    }
}
