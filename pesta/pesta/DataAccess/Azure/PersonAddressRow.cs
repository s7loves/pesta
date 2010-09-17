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
using System.Diagnostics;
using Microsoft.WindowsAzure.StorageClient;
using pesta.Data.Model;

namespace Pesta.DataAccess.Azure
{
    public class PersonAddressRow : TableServiceEntity
    {
        public PersonAddressRow()
        {
            
        }
        public PersonAddressRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
            person_id = partitionKey;
        }

        public string person_id { get; set; } //p //r
        public string country { get; set; }
        public double latitude { get; set; }
        public string locality { get; set; }
        public double longitude { get; set; }
        public string postal_code { get; set; }
        public string region { get; set; }
        public string street_address { get; set; }
        public string address_type { get; set; }
        public string unstructured_address { get; set; }
        public bool primary { get; set; }

    }
}
