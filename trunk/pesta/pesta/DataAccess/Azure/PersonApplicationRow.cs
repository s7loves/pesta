using System;

namespace Pesta.DataAccess.Azure
{
    public class PersonApplicationRow : ApplicationRow
    {
        public PersonApplicationRow()
        {
            
        }
        public PersonApplicationRow(string partitionKey, string rowKey, ApplicationRow row)
            : base(partitionKey, rowKey)
        {
            person_id = partitionKey;
            mod_id = DateTime.UtcNow.Ticks.ToString("x");
            id = row.id;
            url = row.url;
            title = row.title;
            directory_title = row.directory_title;
            screenshot = row.screenshot;
            thumbnail = row.thumbnail;
            author = row.author;
            author_email = row.author_email;
            description = row.description;
            settings = row.settings;
            views = row.views;
            version = row.version;
            height = row.height;
            scrolling = row.scrolling;
            modified = row.modified;
            type = row.type;
        }

        public string person_id { get; set; } //p
        public string mod_id { get; set; } // instance 
    }
}
