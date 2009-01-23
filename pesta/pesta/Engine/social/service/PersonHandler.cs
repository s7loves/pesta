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
using Pesta.DataAccess;
using Pesta.Engine.social.model;
using Pesta.Engine.social.spi;

namespace Pesta.Engine.social.service
{
    /// <summary>
    /// Summary description for PersonHandler
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public class PersonHandler : DataRequestHandler
    {
        private readonly PersonService personService;

        private const string PEOPLE_PATH = "/people/{userId}+/{groupId}/{personId}+";

        public PersonHandler()
        {
            //personService = JsonDbOpensocialService.Instance;
            personService = RayaService.Instance;
        }

        protected override object handleDelete(RequestItem request)
        {
            throw new SocialSpiException(ResponseError.BAD_REQUEST, "You can't delete people.");
        }

        protected override object handlePut(RequestItem request)
        {
            throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED, "You can't update right now.");
        }

        protected override object handlePost(RequestItem request)
        {
            throw new SocialSpiException(ResponseError.NOT_IMPLEMENTED, "You can't add people right now.");
        }

        /**
        * Allowed end-points /people/{userId}+/{groupId} /people/{userId}/{groupId}/{optionalPersonId}+
        *
        * examples: /people/john.doe/@all /people/john.doe/@friends /people/john.doe/@self
        */
        protected override object handleGet(RequestItem request)
        {
            request.applyUrlTemplate(PEOPLE_PATH);

            GroupId groupId = request.getGroup();
            HashSet<String> optionalPersonId = new HashSet<string>(request.getListParameter("personId"));
            HashSet<String> fields = request.getFields(Person.Field.DEFAULT_FIELDS);
            HashSet<UserId> userIds = request.getUsers();

            // Preconditions
            Preconditions<UserId>.requireNotEmpty(userIds, "No userId specified");
            if (userIds.Count > 1 && optionalPersonId.Count != 0)
            {
                throw new ArgumentException("Cannot fetch personIds for multiple userIds");
            }

            CollectionOptions options = new CollectionOptions();
            options.setSortBy(request.getSortBy());
            options.setSortOrder(request.getSortOrder());
            options.setFilter(request.getFilterBy());
            options.setFilterOperation(request.getFilterOperation());
            options.setFilterValue(request.getFilterValue());
            options.setFirst(request.getStartIndex());
            options.setMax(request.getCount());

            if (userIds.Count == 1)
            {
                if (optionalPersonId.Count == 0)
                {
                    if (groupId.getType() == GroupId.Type.self)
                    {
                        IEnumerator<UserId> iuserid = userIds.GetEnumerator();
                        iuserid.MoveNext();
                        return personService.getPerson(iuserid.Current, fields, request.getToken());
                    }
                    return personService.getPeople(userIds, groupId, options, fields, request.getToken());
                }
                if (optionalPersonId.Count == 1)
                {
                    IEnumerator<string> ipersonid = optionalPersonId.GetEnumerator();
                    ipersonid.MoveNext();
                    return personService.getPerson(new UserId(UserId.Type.userId,
                                                              ipersonid.Current), fields, request.getToken());
                }
                HashSet<UserId> personIds = new HashSet<UserId>();
                foreach (String pid in optionalPersonId)
                {
                    personIds.Add(new UserId(UserId.Type.userId, pid));
                }
                // Every other case is a collection response of optional person ids
                return personService.getPeople(personIds, new GroupId(GroupId.Type.self, null),
                                               options, fields, request.getToken());
            }

            // Every other case is a collection response.
            return personService.getPeople(userIds, groupId, options, fields, request.getToken());
        }
    }
}