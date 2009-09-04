using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Samples.ServiceHosting.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class PersonRow : TableStorageEntity
    {
        public PersonRow()
        {
            
        }
        public PersonRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {
        }

        public string id
        {
            get { return PartitionKey; }
            set { }
        } 
        public string email { get; set; }
        public string email_hash { get; set; }
        public string password { get; set; }
        public string about_me { get; set; }
        public int? age { get; set; }
        public string children { get; set; }
        public long? date_of_birth { get; set; }
        public string drinker { get; set; }
        public string ethnicity { get; set; }
        public string fashion { get; set; }
        public long fb_userid { get; set; }
        public string gender { get; set; }
        public string gfc_id { get; set; }
        public string open_id { get; set; }
        public string happiest_when { get; set; }
        public string humor { get; set; }
        public string job_interests { get; set; }
        public string living_arrangement { get; set; }
        public int? looking_for { get; set; }
        public string nickname { get; set; }
        public string pets { get; set; }
        public string political_views { get; set; }
        public string profile_song { get; set; }
        public string profile_url { get; set; }
        public string profile_video { get; set; }
        public string relationship_status { get; set; }
        public string religion { get; set; }
        public string romance { get; set; }
        public string scared_of { get; set; }
        public string sexual_orientation { get; set; }
        public string smoker { get; set; }
        public string status { get; set; }
        public string thumbnail_url { get; set; }
        public long? time_zone { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
    }
}
