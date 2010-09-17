using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;


namespace Pesta.DataAccess.Azure
{
    public class OrganizationRow : TableServiceEntity
    {
        public OrganizationRow()
        {
            
        }
        public OrganizationRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public string id { get; set; }

        public string description { get; set; }

        public long? end_date { get; set; }

        public string field { get; set; }

        public string name { get; set; }

        public string salary { get; set; }

        public long? start_date { get; set; }

        public string sub_field { get; set; }

        public string title { get; set; }

        public string webpage { get; set; }
    }
}
