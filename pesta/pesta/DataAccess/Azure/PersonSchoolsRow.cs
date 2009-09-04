namespace Pesta.DataAccess.Azure
{
    public class PersonSchoolsRow : OrganizationRow
    {
        public PersonSchoolsRow()
        {
            
        }
        public PersonSchoolsRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }
        public string person_id { get; set; } //p //r
    }
}
