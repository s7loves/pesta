namespace Pesta.DataAccess.Azure
{
    public class PersonJobsRow : OrganizationRow
    {
        public PersonJobsRow()
        {
            
        }
        public PersonJobsRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }
        public string person_id { get; set; } //p //r
    }
}
