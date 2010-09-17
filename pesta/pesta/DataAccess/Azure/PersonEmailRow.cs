using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;


namespace Pesta.DataAccess.Azure
{
    public class PersonEmailRow : TableServiceEntity
    {
        public PersonEmailRow()
        {
            
        }
        public PersonEmailRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public int person_id { get; set; } //p //r

        public string address { get; set; } //r

        public string email_type { get; set; }
    }
}
