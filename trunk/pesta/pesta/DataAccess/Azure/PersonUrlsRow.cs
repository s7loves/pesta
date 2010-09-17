using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class PersonUrlsRow: TableServiceEntity
    {
        public PersonUrlsRow()
        {
            
        }
        public PersonUrlsRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public int person_id { get; set; } // p

        public string url { get; set; }
    }
}
