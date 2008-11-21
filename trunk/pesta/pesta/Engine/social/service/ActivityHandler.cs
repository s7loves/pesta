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

namespace Pesta
{
    /// <summary>
    /// Summary description for ActivityHandler
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig, ported to C# by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class ActivityHandler : DataRequestHandler
    {
        private readonly ActivityService service;

        private static readonly String ACTIVITY_ID_PATH = "/activities/{userId}+/{groupId}/{appId}/{activityId}+";

        public ActivityHandler()
        {
            this.service = JsonDbOpensocialService.Instance;
        }

        /**
        * Allowed end-points /activities/{userId}/@self/{actvityId}+
        *
        * examples: /activities/john.doe/@self/1
        */
        protected override object handleDelete(RequestItem request)
        {
            request.applyUrlTemplate(ACTIVITY_ID_PATH);
            HashSet<UserId> userIds = request.getUsers();
            HashSet<String> activityIds = new HashSet<string>(request.getListParameter("activityId"));
            DataRequestHandler.Preconditions<UserId>.requireNotEmpty(userIds, "No userId specified");
            DataRequestHandler.Preconditions<UserId>.requireSingular(userIds, "Multiple userIds not supported");
            IEnumerator<UserId> iuserid = userIds.GetEnumerator();
            iuserid.MoveNext();
            service.deleteActivities(iuserid.Current, request.getGroup(),
                            request.getAppId(), activityIds, request.getToken());
            return null;
        }

        /**
        * Allowed end-points /activities/{userId}/@self
        *
        * examples: /activities/john.doe/@self - postBody is an activity object
        */
        protected override object handlePut(RequestItem request)
        {
            handlePost(request);
            return null;
        }

        /**
        * Allowed end-points /activities/{userId}/@self
        *
        * examples: /activities/john.doe/@self - postBody is an activity object
        */
        protected override object handlePost(RequestItem request)
        {
            request.applyUrlTemplate(ACTIVITY_ID_PATH);

            HashSet<UserId> userIds = request.getUsers();
            HashSet<String> activityIds = new HashSet<string>(request.getListParameter("activityId"));

            DataRequestHandler.Preconditions<UserId>.requireNotEmpty(userIds, "No userId specified");
            DataRequestHandler.Preconditions<UserId>.requireSingular(userIds, "Multiple userIds not supported");
            // TODO(lryan) This seems reasonable to allow on PUT but we don't have an update verb.
            DataRequestHandler.Preconditions<String>.requireEmpty(activityIds, "Cannot specify activityId in create");
            IEnumerator<UserId> iuserid = userIds.GetEnumerator();
            iuserid.MoveNext();
            service.createActivity(iuserid.Current, request.getGroup(),
                            request.getAppId(), request.getFields(),
                            (Activity)request.getTypedParameter("activity", typeof(Activity)),
                            request.getToken());
            return null;
        }

        /**
        * Allowed end-points /activities/{userId}/{groupId}/{optionalActvityId}+
        * /activities/{userId}+/{groupId}
        *
        * examples: /activities/john.doe/@self/1 /activities/john.doe/@self
        * /activities/john.doe,jane.doe/@friends
        */
        protected override object handleGet(RequestItem request)
        {
            request.applyUrlTemplate(ACTIVITY_ID_PATH);

            HashSet<UserId> userIds = request.getUsers();
            HashSet<String> optionalActivityIds = new HashSet<string>(request.getListParameter("activityId"));

            // Preconditions
            DataRequestHandler.Preconditions<UserId>.requireNotEmpty(userIds, "No userId specified");
            if (userIds.Count > 1 && optionalActivityIds.Count != 0)
            {
                throw new ArgumentException("Cannot fetch same activityIds for multiple userIds");
            }

            if (optionalActivityIds.Count != 0)
            {
                if (optionalActivityIds.Count == 1)
                {
                    IEnumerator<UserId> iuserid = userIds.GetEnumerator();
                    iuserid.MoveNext();
                    IEnumerator<string> iactivity = optionalActivityIds.GetEnumerator();
                    iactivity.MoveNext();
                    return service.getActivity(iuserid.Current, request.getGroup(),
                                request.getAppId(), request.getFields(), iactivity.Current,
                                request.getToken());
                }
                else
                {
                    IEnumerator<UserId> iuserid = userIds.GetEnumerator();
                    iuserid.MoveNext();
                    return service.getActivities(iuserid.Current, request.getGroup(),
                        request.getAppId(), request.getFields(), optionalActivityIds, request.getToken());
                }
            }

            return service.getActivities(userIds, request.getGroup(), request.getAppId(),
                // TODO: add pagination and sorting support
                // getSortBy(params), getFilterBy(params), getStartIndex(params), getCount(params),
                                    request.getFields(), request.getToken());
        }
    } 
}
