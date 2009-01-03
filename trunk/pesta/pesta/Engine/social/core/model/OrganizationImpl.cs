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
using Pesta.Engine.social.model;

namespace Pesta.Engine.social.core.model
{
    /// <summary>
    /// Summary description for OrganizationImpl
    /// </summary>
    /// <remarks>
    /// <para>
    ///  Apache Software License 2.0 2008 Shindig
    /// </para>
    /// </remarks>
    public class OrganizationImpl : Organization
    {
        private Address address;
        private String description;
        private DateTime? endDate;
        private String field;
        private String name;
        private String salary;
        private DateTime? startDate;
        private String subField;
        private String title;
        private String webpage;
        private String type;
        private bool? primary;

        public override Address getAddress()
        {
            return address;
        }

        public override void setAddress(Address address)
        {
            this.address = address;
        }

        public override String getDescription()
        {
            return description;
        }

        public override void setDescription(String description)
        {
            this.description = description;
        }

        public override DateTime? getEndDate()
        {
            if (endDate == null)
            {
                return null;
            }
            return endDate;
        }

        public override void setEndDate(DateTime? endDate)
        {
            if (endDate == null)
            {
                this.endDate = null;
            }
            else
            {
                this.endDate = endDate;
            }
        }

        public override String getField()
        {
            return field;
        }

        public override void setField(String field)
        {
            this.field = field;
        }

        public override String getName()
        {
            return name;
        }

        public override void setName(String name)
        {
            this.name = name;
        }

        public override String getSalary()
        {
            return salary;
        }

        public override void setSalary(String salary)
        {
            this.salary = salary;
        }

        public override DateTime? getStartDate()
        {
            if (startDate == null)
            {
                return null;
            }
            return startDate;
        }

        public override void setStartDate(DateTime? startDate)
        {
            if (startDate == null)
            {
                this.startDate = null;
            }
            else
            {
                this.startDate = startDate;
            }
        }

        public override String getSubField()
        {
            return subField;
        }

        public override void setSubField(String subField)
        {
            this.subField = subField;
        }

        public override String getTitle()
        {
            return title;
        }

        public override void setTitle(String title)
        {
            this.title = title;
        }

        public override String getWebpage()
        {
            return webpage;
        }

        public override void setWebpage(String webpage)
        {
            this.webpage = webpage;
        }

        public override String getType()
        {
            return type;
        }

        public override void setType(String type)
        {
            this.type = type;
        }

        public override bool? getPrimary()
        {
            return primary;
        }

        public override void setPrimary(bool? primary)
        {
            this.primary = primary;
        }
    }
}