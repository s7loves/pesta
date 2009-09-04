using Microsoft.Samples.ServiceHosting.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class OrganizationAddressRow : TableStorageEntity
    {
        public OrganizationAddressRow()
        {
            
        }
        public OrganizationAddressRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }

        public string organization_id { get; set; } //p //r
        public string country { get; set; }
        public double latitude { get; set; }
        public string locality { get; set; }
        public double longitude { get; set; }
        public string postal_code { get; set; }
        public string region { get; set; }
        public string street_address { get; set; }
        public string address_type { get; set; }
        public string unstructured_address { get; set; }
        public bool primary { get; set; }
    }
}
