using System.Diagnostics;
using Microsoft.WindowsAzure.StorageClient;


namespace Pesta.DataAccess.Azure
{
    public class ApplicationRow : TableServiceEntity
    {
        public ApplicationRow()
        {
            
        }

        public ApplicationRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
            id = rowKey;
        }
        public string id { get; set; }
        public string url { get; set; } // p
        public string title { get; set; }
        public string directory_title { get; set; }
        public string screenshot { get; set; }
        public string thumbnail { get; set; }
        public string author { get; set; }
        public string author_email { get; set; }
        public string description { get; set; }
        public string settings { get; set; }
        public string views { get; set; }
        public string version { get; set; }
        public int height { get; set; }
        public string scrolling { get; set; }
        public long modified { get; set; }
        public int type { get; set; }
    }
}