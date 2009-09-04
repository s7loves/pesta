using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.ServiceHosting.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class PersonBodyTypeRow : TableStorageEntity
    {
        public PersonBodyTypeRow()
        {
            
        }
        public PersonBodyTypeRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public int person_id { get; set; } // p // r

        public string build { get; set; }

        public string eye_color { get; set; }

        public string hair_color { get; set; }

        public int height { get; set; }

        public int weight { get; set; }
    }
}
