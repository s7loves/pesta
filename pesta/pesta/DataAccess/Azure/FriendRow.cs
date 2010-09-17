using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;


namespace Pesta.DataAccess.Azure
{
    public class FriendRow : TableServiceEntity
    {
        public FriendRow()
        {
            
        }
        public FriendRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public string person_id { get; set; } // p
        public string friend_id { get; set; } // r
    }
}
