using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.ServiceHosting.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class OAuthTokenRow: TableStorageEntity
    {
        public OAuthTokenRow()
        {
            
        }
        public OAuthTokenRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        public string user_id { get; set; }
        public string consumer_key { get; set; } // r
        public string type { get; set; }
        public string token_key { get; set; } // p //r
        public string token_secret { get; set; }
        public int authorized { get; set; }
    }
}
