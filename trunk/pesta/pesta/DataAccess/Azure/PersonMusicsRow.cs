using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.ServiceHosting.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class PersonMusicsRow: TableStorageEntity
    {
        public PersonMusicsRow()
        {
            
        }
        public PersonMusicsRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public int person_id { get; set; }

        public string music { get; set; }
    }
}
