

using Microsoft.WindowsAzure.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class OAuthConsumerRow : TableServiceEntity
    {
        public OAuthConsumerRow()
        {

        }

        public OAuthConsumerRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public int id { get; set; }
        public string user_id { get; set; } // p
        public string app_id { get; set; }
        public string consumer_key { get; set; }
        public string consumer_secret { get; set; }
    }
}
