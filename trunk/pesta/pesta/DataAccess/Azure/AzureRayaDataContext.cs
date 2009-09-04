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
using System.Data.Services.Client;
using System.Linq;
using Microsoft.Samples.ServiceHosting.StorageClient;
using Pesta.DataAccess.Azure;

namespace Pesta.DataAccess.Azure
{
    /// <summary>
    /// Used to init Table Storage from Application_BeginRequest only once by using the singleton pattern
    /// </summary>
    public class ApplicationInitAzureTables
    {
        public static readonly ApplicationInitAzureTables Init = new ApplicationInitAzureTables();

        private ApplicationInitAzureTables()
        {
            TableStorage.CreateTablesFromModel(typeof (AzureRayaDataContext));
        }
    }

    public class AzureRayaDataContext : TableStorageDataServiceContext, IDisposable
    {
        public enum TableNames
        {
            activities,
            activityMediaItems,
            addressesPerson,
            addressesOrganization,
            applicationSettings,
            applications,
            authenticateds,
            fbUsers,
            friends,
            gfcUsers,
            images,
            messages,
            notifications,
            oauthConsumers,
            oauthTokens,
            oauthNonces,
            openidUsers,
            organizations,
            persons,
            personActivities,
            personAddresses,
            personApplications,
            personBodyTypes,
            personBooks,
            personCars,
            personCurrentLocations,
            personEmails,
            personFoods,
            personHeroes,
            personInterests,
            personJobs,
            personMovies,
            personMusics,
            personPhoneNumbers,
            personQuotes,
            personSchools,
            personSports,
            personTags,
            personTurnOffs,
            personTurnOns,
            personTvShows,
            personUrls,
            tags,
            tagsCounts
        }

        public AzureRayaDataContext()
            : base(StorageAccountInfo.GetDefaultTableStorageAccountFromConfiguration())
        {
            RetryPolicy = RetryPolicies.RetryN(3, TimeSpan.FromSeconds(1));
        }

        public AzureRayaDataContext(StorageAccountInfo account)
            : base(account)
        {
            RetryPolicy = RetryPolicies.RetryN(3, TimeSpan.FromSeconds(1));
        }

        public IQueryable<ActivityRow> activities
        {
            get { return this.CreateQuery<ActivityRow>(TableNames.activities.ToString()); }
        }

        public IQueryable<MediaItemRow> activityMediaItems
        {
            get { return this.CreateQuery<MediaItemRow>(TableNames.activityMediaItems.ToString()); }
        }

        public IQueryable<PersonAddressRow> addressesPerson
        {
            get { return this.CreateQuery<PersonAddressRow>(TableNames.addressesPerson.ToString()); }
        }

        public IQueryable<OrganizationAddressRow> addressesOrganization
        {
            get { return this.CreateQuery<OrganizationAddressRow>(TableNames.addressesOrganization.ToString()); }
        }

        public IQueryable<ApplicationRow> applications
        {
            get { return this.CreateQuery<ApplicationRow>(TableNames.applications.ToString()); }
        }

        public IQueryable<ApplicationSettingRow> applicationSettings
        {
            get { return this.CreateQuery<ApplicationSettingRow>(TableNames.applicationSettings.ToString()); }
        }

        public IQueryable<FriendRow> friends
        {
            get { return this.CreateQuery<FriendRow>(TableNames.friends.ToString()); }
        }

        public IQueryable<ImageRow> images
        {
            get { return this.CreateQuery<ImageRow>(TableNames.images.ToString()); }
        }

        public IQueryable<MessageRow> messages
        {
            get { return this.CreateQuery<MessageRow>(TableNames.messages.ToString()); }
        }

        public IQueryable<NotificationRow> notifications
        {
            get { return this.CreateQuery<NotificationRow>(TableNames.notifications.ToString()); }
        }

        public IQueryable<OAuthConsumerRow> oauthConsumers
        {
            get { return this.CreateQuery<OAuthConsumerRow>(TableNames.oauthConsumers.ToString()); }
        }

        public IQueryable<oauthNonce> oauthNonces
        {
            get { return this.CreateQuery<oauthNonce>(TableNames.oauthNonces.ToString()); }
        }

        public IQueryable<OAuthTokenRow> oauthTokens
        {
            get { return this.CreateQuery<OAuthTokenRow>(TableNames.oauthTokens.ToString()); }
        }

        public IQueryable<OrganizationRow> organizations
        {
            get { return this.CreateQuery<OrganizationRow>(TableNames.organizations.ToString()); }
        }

        public IQueryable<PersonRow> persons
        {
            get { return this.CreateQuery<PersonRow>(TableNames.persons.ToString()); }
        }

        public IQueryable<PersonApplicationRow> personApplications
        {
            get { return this.CreateQuery<PersonApplicationRow>(TableNames.personApplications.ToString()); }
        }

        public IQueryable<PersonBooksRow> personBooks
        {
            get { return this.CreateQuery<PersonBooksRow>(TableNames.personBooks.ToString()); }
        }

        public IQueryable<PersonBodyTypeRow> personBodyTypes
        {
            get { return this.CreateQuery<PersonBodyTypeRow>(TableNames.personBodyTypes.ToString()); }
        }

        public IQueryable<PersonCarsRow> personCars
        {
            get { return this.CreateQuery<PersonCarsRow>(TableNames.personCars.ToString()); }
        }

        public IQueryable<PersonCurrentLocationRow> personCurrentLocations
        {
            get { return this.CreateQuery<PersonCurrentLocationRow>(TableNames.personCurrentLocations.ToString()); }
        }

        public IQueryable<PersonEmailRow> personEmails
        {
            get { return this.CreateQuery<PersonEmailRow>(TableNames.personEmails.ToString()); }
        }

        public IQueryable<PersonFoodsRow> personFoods
        {
            get { return this.CreateQuery<PersonFoodsRow>(TableNames.personFoods.ToString()); }
        }

        public IQueryable<PersonHeroesRow> personHeroes
        {
            get { return this.CreateQuery<PersonHeroesRow>(TableNames.personHeroes.ToString()); }
        }

        public IQueryable<PersonInterestsRow> personInterests
        {
            get { return this.CreateQuery<PersonInterestsRow>(TableNames.personInterests.ToString()); }
        }

        public IQueryable<PersonJobsRow> personJobs
        {
            get { return this.CreateQuery<PersonJobsRow>(TableNames.personJobs.ToString()); }
        }

        public IQueryable<PersonMoviesRow> personMovies
        {
            get { return this.CreateQuery<PersonMoviesRow>(TableNames.personMovies.ToString()); }
        }

        public IQueryable<PersonMusicsRow> personMusics
        {
            get { return this.CreateQuery<PersonMusicsRow>(TableNames.personMusics.ToString()); }
        }

        public IQueryable<PersonPhoneNumbersRow> personPhoneNumbers
        {
            get { return this.CreateQuery<PersonPhoneNumbersRow>(TableNames.personPhoneNumbers.ToString()); }
        }
        /*
        public IQueryable<personQuote> personQuotes
        {
            get { return this.CreateQuery<personQuote>(TableNames.personQuotes.ToString()); }
        }
        */
        public IQueryable<PersonSchoolsRow> personSchools
        {
            get { return this.CreateQuery<PersonSchoolsRow>(TableNames.personSchools.ToString()); }
        }

        public IQueryable<personSport> personSports
        {
            get { return this.CreateQuery<personSport>(TableNames.personSports.ToString()); }
        }

        public IQueryable<personTag> personTags
        {
            get { return this.CreateQuery<personTag>(TableNames.personTags.ToString()); }
        }
        /*
        public IQueryable<personTurnOff> personTurnOffs
        {
            get { return this.CreateQuery<personTurnOff>(TableNames.personTurnOffs.ToString()); }
        }

        public IQueryable<personTurnOn> personTurnOns
        {
            get { return this.CreateQuery<personTurnOn>(TableNames.personTurnOns.ToString()); }
        }
        */
        public IQueryable<PersonUrlsRow> personUrls
        {
            get { return this.CreateQuery<PersonUrlsRow>(TableNames.personUrls.ToString()); }
        }

        public IQueryable<personTvShow> personTvShows
        {
            get { return this.CreateQuery<personTvShow>(TableNames.personTvShows.ToString()); }
        }

        public IQueryable<tag> tags
        {
            get { return this.CreateQuery<tag>(TableNames.tags.ToString()); }
        }


        public IQueryable<tagsCount> tagsCounts
        {
            get { return this.CreateQuery<tagsCount>(TableNames.tagsCounts.ToString()); }
        }

        public void Dispose()
        {
        }

        public void InsertOnSubmit(TableNames entityName, object entity)
        {
            this.AddObject(entityName.ToString(), entity);
        }

        public void DeleteOnSubmit(object entity)
        {
            this.DeleteObject(entity);
        }

        public void DeleteAllOnSubmit(IQueryable entityList)
        {
            foreach (var entity in entityList)
            {
                this.DeleteObject(entity);
            }
        }

        public void UpdateOnSubmit(object entity)
        {
            this.UpdateObject(entity);
        }

        public DataServiceResponse SubmitChanges()
        {
            return this.SaveChanges();
        }
    }
}