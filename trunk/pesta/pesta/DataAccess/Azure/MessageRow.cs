using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.ServiceHosting.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class MessageRow: TableStorageEntity
    {
        public MessageRow()
        {
            
        }
        public MessageRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public string id { get; set; } // r
        public string sender { get; set; } //p
        public string recipient { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string read { get; set; }
        public string to_deleted { get; set; }
        public string from_deleted { get; set; }
        public long created { get; set; }
        public string senderName { get; set; }
        public string senderThumbnailUrl { get; set; }
        public string recipientName { get; set; }
        public string recipientThumbnailUrl { get; set; }
    }
}
