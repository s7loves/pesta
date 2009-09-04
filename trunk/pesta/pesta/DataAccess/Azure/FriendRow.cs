using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.ServiceHosting.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class FriendRow : TableStorageEntity
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
