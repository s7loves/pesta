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
using pesta.Data;
using Pesta.Engine.auth;

namespace Pesta.Engine.social.spi
{
    /// <summary>
    /// Summary description for ActivityService
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public interface IActivityService
    {
        /**
       * Returns a list of activities that correspond to the passed in users and group.
       *
       * @param userIds The set of ids of the people to fetch activities for.
       * @param groupId Indicates whether to fetch activities for a group.
       * @param appId   The app id.
       * @param fields  The fields to return. Empty set implies all
       * @param token   A valid SecurityToken
       * @return a response item with the list of activities.
       */
        RestfulCollection<Activity> getActivities(HashSet<UserId> userIds,
                                                  GroupId groupId, String appId, CollectionOptions options, HashSet<String> fields, ISecurityToken token);

        /**
         * Returns a set of activities for the passed in user and group that corresponds to a list of
         * activityIds.
         *
         * @param userId      The set of ids of the people to fetch activities for.
         * @param groupId     Indicates whether to fetch activities for a group.
         * @param appId       The app id.
         * @param fields      The fields to return. Empty set implies all
         * @param activityIds The set of activity ids to fetch.
         * @param token       A valid SecurityToken
         * @return a response item with the list of activities.
         */
        RestfulCollection<Activity> getActivities(UserId userId, GroupId groupId,
                                                  String appId, HashSet<String> fields, HashSet<String> activityIds, ISecurityToken token);

        /**
         * Returns a set of activities for the passed in user and group that corresponds to a single of
         * activityId
         *
         * @param userId     The set of ids of the people to fetch activities for.
         * @param groupId    Indicates whether to fetch activities for a group.
         * @param appId      The app id.
         * @param fields     The fields to return. Empty set implies all
         * @param activityId The activity id to fetch.
         * @param token      A valid SecurityToken
         * @return a response item with the list of activities.
         */
        Activity getActivity(UserId userId, GroupId groupId, String appId, CollectionOptions options,
                             HashSet<String> fields, String activityId, ISecurityToken token);

        /**
         * Deletes the activity for the passed in user and group that corresponds to the activityId.
         *
         * @param userId      The user.
         * @param groupId     The group.
         * @param appId       The app id.
         * @param activityIds A list of activity ids to delete.
         * @param token       A valid SecurityToken.
         * @return a response item containing any errors
         */
        void deleteActivities(UserId userId, GroupId groupId, String appId,
                              HashSet<String> activityIds, ISecurityToken token);

        /**
         * Creates the passed in activity for the passed in user and group. Once createActivity is called,
         * getActivities will be able to return the Activity.
         *
         * @param userId   The id of the person to create the activity for.
         * @param groupId  The group.
         * @param appId    The app id.
         * @param fields   The fields to return.
         * @param activity The activity to create.
         * @param token    A valid SecurityToken
         * @return a response item containing any errors
         */
        void createActivity(UserId userId, GroupId groupId, String appId,
                            HashSet<String> fields, Activity activity, ISecurityToken token);
    }
}