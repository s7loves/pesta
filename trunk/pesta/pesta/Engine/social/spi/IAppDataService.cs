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
using Pesta.Engine.auth;

namespace Pesta.Engine.social.spi
{
    /// <summary>
    /// Summary description for AppDataService
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public interface IAppDataService
    {
        /**
       * Retrives app data for the specified user list and group.
       *
       * @param userIds A set of UserIds.
       * @param groupId The group
       * @param appId   The app
       * @param fields  The fields to filter the data by. Empty set implies all
       * @param token   The security token
       * @return The data fetched
       */
        DataCollection getPersonData(HashSet<UserId> userIds, GroupId groupId,
                                     String appId, HashSet<String> fields, ISecurityToken token);

        /**
         * Deletes data for the specified user and group.
         *
         * @param userId  The user
         * @param groupId The group
         * @param appId   The app
         * @param fields  The fields to delete. Empty set implies all
         * @param token   The security token
         * @return an error if one occurs
         */
        void deletePersonData(UserId userId, GroupId groupId,
                              String appId, HashSet<String> fields, ISecurityToken token);

        /**
         * Updates app data for the specified user and group with the new values.
         *
         * @param userId  The user
         * @param groupId The group
         * @param appId   The app
         * @param fields  The fields to filter the data by. Empty set implies all
         * @param values  The values to set
         * @param token   The security token
         * @return an error if one occurs
         */
        void updatePersonData(UserId userId, GroupId groupId,
                              String appId, HashSet<String> fields, Dictionary<String, String> values, ISecurityToken token);
    }
}