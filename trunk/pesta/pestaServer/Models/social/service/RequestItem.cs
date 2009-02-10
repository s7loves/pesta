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
using Pesta.Engine.social;
using Pesta.Engine.social.spi;

namespace pestaServer.Models.social.service
{
    /// <summary>
    /// Summary description for RequestItem
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig ported to Pesta by Sean Lin M.T. (my6solutions.com)
    /// </para>
    /// </remarks>
    public abstract class RequestItem
    {
        // Common OpenSocial API fields
        public static readonly String APP_ID = "appId";

        public static readonly String USER_ID = "userId";

        public static readonly String GROUP_ID = "groupId";

        public static readonly String START_INDEX = "startIndex";

        public static readonly String COUNT = "count";

        public static readonly String SORT_BY = "sortBy";
        public static readonly String SORT_ORDER = "sortOrder";

        public static readonly String FILTER_BY = "filterBy";
        public static readonly String FILTER_OPERATION = "filterOp";
        public static readonly String FILTER_VALUE = "filterValue";

        public static readonly String FIELDS = "fields";

        // Opensocial defaults
        public static readonly int DEFAULT_START_INDEX = 0;

        public static readonly int DEFAULT_COUNT = 20;

        public static readonly String APP_SUBSTITUTION_TOKEN = "@app";

        private readonly ISecurityToken token;

        protected readonly BeanConverter converter;

        private readonly String operation;

        private readonly String service;

        public RequestItem(String service, String operation, ISecurityToken token, BeanConverter converter)
        {
            this.service = service;
            this.operation = operation;
            this.token = token;
            this.converter = converter;
        }

        public String getAppId()
        {
            String appId = getParameter(APP_ID);
            if (appId != null && appId.Equals(APP_SUBSTITUTION_TOKEN))
            {
                return token.getAppId();
            }
            else
            {
                return appId;
            }
        }

        public HashSet<UserId> getUsers()
        {
            List<String> ids = getListParameter(USER_ID);
            if (ids.Count == 0)
            {
                if (token.getViewerId() != null)
                {
                    // Assume @me
                    ids = new List<string>() { "@me" };
                }
                else
                {
                    throw new ArgumentException("No userId provided and viewer not available");
                }
            }
            HashSet<UserId> userIds = new HashSet<UserId>();
            foreach (String id in ids)
            {
                userIds.Add(UserId.fromJson(id));
            }
            return userIds;
        }


        public GroupId getGroup()
        {
            return GroupId.fromJson(getParameter(GROUP_ID, "@self"));
        }

        public int getStartIndex()
        {
            String startIndex = getParameter(START_INDEX);
            try
            {
                return startIndex == null ? DEFAULT_START_INDEX : int.Parse(startIndex);
            }
            catch (Exception nfe)
            {
                throw new SocialSpiException(ResponseError.BAD_REQUEST,
                                             "Parameter " + START_INDEX + " (" + startIndex + ") is not a number.");
            }
        }

        public int getCount()
        {
            String count = getParameter(COUNT);
            try
            {
                return count == null ? DEFAULT_COUNT : int.Parse(count);
            }
            catch (FormatException nfe)
            {
                throw new SocialSpiException(ResponseError.BAD_REQUEST,
                                             "Parameter " + COUNT + " (" + count + ") is not a number.");
            }
        }

        public String getSortBy()
        {
            String sortBy = getParameter(SORT_BY);
            return sortBy == null ? IPersonService.TOP_FRIENDS_SORT : sortBy;
        }

        public IPersonService.SortOrder getSortOrder()
        {
            String sortOrder = getParameter(SORT_ORDER);
            try
            {
                return sortOrder == null
                           ? IPersonService.SortOrder.ascending
                           : (IPersonService.SortOrder)Enum.Parse(typeof(IPersonService.SortOrder), sortOrder, true);
            }
            catch
            {
                throw new SocialSpiException(ResponseError.BAD_REQUEST,
                                             "Parameter " + SORT_ORDER + " (" + sortOrder + ") is not valid.");
            }
        }

        public String getFilterBy()
        {
            return getParameter(FILTER_BY);
        }

        public IPersonService.FilterOperation getFilterOperation()
        {
            String filterOp = getParameter(FILTER_OPERATION);
            try
            {
                return filterOp == null
                           ? IPersonService.FilterOperation.contains
                           : (IPersonService.FilterOperation)Enum.Parse(typeof(IPersonService.FilterOperation), filterOp, true);
            }
            catch
            {
                throw new SocialSpiException(ResponseError.BAD_REQUEST,
                                             "Parameter " + FILTER_OPERATION + " (" + filterOp + ") is not valid.");
            }
        }

        public String getFilterValue()
        {
            String filterValue = getParameter(FILTER_VALUE);
            return filterValue == null ? "" : filterValue;
        }

        public HashSet<String> getFields()
        {
            return getFields(new HashSet<String>());
        }

        public HashSet<String> getFields(HashSet<String> defaultValue)
        {
            HashSet<String> result = new HashSet<string>();
            result.UnionWith(getListParameter(FIELDS));
            if (result.Count == 0)
            {
                return defaultValue;
            }
            return result;
        }

        public String getOperation()
        {
            return operation;
        }

        public String getService()
        {
            return service;
        }

        public ISecurityToken getToken()
        {
            return token;
        }

        public abstract object getTypedParameter(String parameterName, Type dataTypeClass);

        public abstract object getTypedParameters(Type dataTypeClass);

        public abstract void applyUrlTemplate(String urlTemplate);

        public abstract String getParameter(String paramName);

        public abstract String getParameter(String paramName, String defaultValue);

        public abstract List<String> getListParameter(String paramName);
    }
}