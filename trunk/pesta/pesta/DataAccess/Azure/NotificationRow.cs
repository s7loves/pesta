using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.WindowsAzure.StorageClient;


namespace Pesta.DataAccess.Azure
{
    public class NotificationRow : TableServiceEntity
    {
        public NotificationRow()
        {
            
        }
        public NotificationRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        public string creator { get; set; } //p
        public string creatorName { get; set; }
        public string recipient { get; set; }
        public int type { get; set; }
    }
}
