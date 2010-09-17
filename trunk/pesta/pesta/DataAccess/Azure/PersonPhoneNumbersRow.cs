using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;


namespace Pesta.DataAccess.Azure
{
    public class PersonPhoneNumbersRow : TableServiceEntity
    {
        public PersonPhoneNumbersRow()
        {
            
        }
        public PersonPhoneNumbersRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public int person_id { get; set; } //p

        public string number { get; set; } //r

        public string number_type { get; set; }

    }
}
