using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.ServiceHosting.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class PersonCarsRow : TableStorageEntity
    {
        public PersonCarsRow()
        {
            
        }
        public PersonCarsRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public int person_id { get; set; } //p //r
        public string car { get; set; }
    }
}
