

using Microsoft.WindowsAzure.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class ImageRow : TableServiceEntity
    {
        public ImageRow()
        {
            
        }
        public ImageRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
            id = rowKey;
        }

        public string id { get; set; } //r
        public string userid { get; set; } //p 
        public string url { get; set; }
        public int image_type { get; set; }
    }
}
