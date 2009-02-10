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
using Pesta.Engine.social;
using Pesta.Engine.social.spi;
using pestaServer.DataAccess;


namespace pestaServer.Models.social.service
{
    /// <summary>
    /// Summary description for AppDataHandler
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class AppDataHandler : DataRequestHandler
    {
        private readonly IAppDataService service;

        private const string APP_DATA_PATH = "/appdata/{userId}+/{groupId}/{appId}";

        public AppDataHandler()
        {
            //service = JsonDbOpensocialService.Instance;
            service = RayaService.Instance;
        }

        /**
        * Allowed endpoints /appdata/{userId}/{groupId}/{appId} - fields={field1, field2}
        *
        * examples: /appdata/john.doe/@friends/app?fields=count /appdata/john.doe/@self/app
        *
        * The post data should be a regular json object. All of the fields vars will be pulled from the
        * values and set on the person object. If there are no fields vars then all of the data will be
        * overridden.
        */

        protected override object handleDelete(RequestItem request)
        {
            request.applyUrlTemplate(APP_DATA_PATH);

            HashSet<UserId> userIds = request.getUsers();

            Preconditions<UserId>.requireNotEmpty(userIds, "No userId specified");
            Preconditions<UserId>.requireSingular(userIds, "Multiple userIds not supported");
            IEnumerator<UserId> iuserid = userIds.GetEnumerator();
            iuserid.MoveNext();
            service.deletePersonData(iuserid.Current, request.getGroup(),
                                     request.getAppId(), request.getFields(), request.getToken());
            return null;
        }

        /**
        * Allowed endpoints /appdata/{userId}/{groupId}/{appId} - fields={field1, field2}
        *
        * examples: /appdata/john.doe/@friends/app?fields=count /appdata/john.doe/@self/app
        *
        * The post data should be a regular json object. All of the fields vars will be pulled from the
        * values and set on the person object. If there are no fields vars then all of the data will be
        * overridden.
        */

        protected override object handlePut(RequestItem request)
        {
            handlePost(request);
            return null;
        }

        /**
        * /appdata/{userId}/{groupId}/{appId} - fields={field1, field2}
        *
        * examples: /appdata/john.doe/@friends/app?fields=count /appdata/john.doe/@self/app
        *
        * The post data should be a regular json object. All of the fields vars will be pulled from the
        * values and set. If there are no fields vars then all of the data will be overridden.
        */
        protected override object handlePost(RequestItem request)
        {
            request.applyUrlTemplate(APP_DATA_PATH);

            HashSet<UserId> userIds = request.getUsers();

            Preconditions<UserId>.requireNotEmpty(userIds, "No userId specified");
            Preconditions<UserId>.requireSingular(userIds, "Multiple userIds not supported");

            Dictionary<String, String> values = (Dictionary<String, String>)request.getTypedParameter("data", typeof(Dictionary<String, String>));
            foreach (String key in values.Keys)
            {
                if (!isValidKey(key))
                {
                    throw new SocialSpiException(ResponseError.BAD_REQUEST,
                                                 "One or more of the app data keys are invalid: " + key);
                }
            }
            IEnumerator<UserId> iuserid = userIds.GetEnumerator();
            iuserid.MoveNext();
            service.updatePersonData(iuserid.Current, request.getGroup(),
                                     request.getAppId(), request.getFields(), values, request.getToken());
            return null;
        }

        /**
        * /appdata/{userId}+/{groupId}/{appId} - fields={field1, field2}
        *
        * examples: /appdata/john.doe/@friends/app?fields=count /appdata/john.doe/@self/app
        */
        protected override object handleGet(RequestItem request)
        {
            request.applyUrlTemplate(APP_DATA_PATH);

            HashSet<UserId> userIds = request.getUsers();

            // Preconditions
            Preconditions<UserId>.requireNotEmpty(userIds, "No userId specified");

            return service.getPersonData(userIds, request.getGroup(),
                                         request.getAppId(), request.getFields(), request.getToken());
        }

        /**
        * Determines whether the input is a valid key. Valid keys match the regular expression [\w\-\.]+.
        * The logic is not done using java.util.regex.* as that is 20X slower.
        *
        * @param key the key to validate.
        * @return true if the key is a valid appdata key, false otherwise.
        */
        public static bool isValidKey(String key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }
            for (int i = 0; i < key.Length; ++i)
            {
                char c = key[i];
                if ((c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c >= '0' && c <= '9') ||
                    (c == '-') ||
                    (c == '_') ||
                    (c == '.'))
                {
                    continue;
                }
                return false;
            }
            return true;
        }
    }
}