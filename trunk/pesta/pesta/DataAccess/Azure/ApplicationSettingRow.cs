using System.Diagnostics;
using Microsoft.WindowsAzure.StorageClient;


namespace Pesta.DataAccess.Azure
{
    public class ApplicationSettingRow : TableServiceEntity
    {
        public ApplicationSettingRow()
        {
            
        }
        /// <summary>
        /// partitionKey = personID, rowKey = string.Concat(personId, "-", appId, "-", key)
        /// </summary>
        /// <param name="partitionKey"></param>
        /// <param name="rowKey"></param>
        public ApplicationSettingRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public string application_id { get; set; } //r 
        public string person_id { get; set; } // p //r 
        public string name { get; set; } // r
        public string value { get; set; }
    }
}
