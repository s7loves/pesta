using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.ServiceHosting.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class PersonBooksRow: TableStorageEntity
    {
        public PersonBooksRow()
        {
            
        }
        public PersonBooksRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public int person_id { get; set; } //p //r
        public string book { get; set; } //r
    }
}
