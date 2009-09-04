using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Samples.ServiceHosting.StorageClient;

namespace Pesta.DataAccess.Azure
{
    public class tagsCount : TableStorageEntity
    {
        public tagsCount()
        {
            Timestamp = DateTime.UtcNow;
            PartitionKey = "";
            RowKey = XmlConvert.ToString(Timestamp, XmlDateTimeSerializationMode.Utc);
        }

        public int tagid { get; set; }
        public int refid { get; set; }
    }

    public class language : TableStorageEntity
    {
        public int id { get; set; }

        public string code { get; set; }

        public string name { get; set; }
    }

    public class notification : TableStorageEntity
    {
        public notification()
        {
            Debug.Assert(!string.IsNullOrEmpty(creator.ToString()));
            Timestamp = DateTime.UtcNow;
            PartitionKey = creator.ToString();
            RowKey = XmlConvert.ToString(Timestamp, XmlDateTimeSerializationMode.Utc);
        }


        public int creator { get; set; }

        public int recipient { get; set; }

        public int type { get; set; }
    }


    public class oauthNonce : TableStorageEntity
    {
        public oauthNonce()
        {
            Debug.Assert(!string.IsNullOrEmpty(nonce));
            PartitionKey = nonce;
            RowKey = nonce_timestamp.ToString();
        }


        public string nonce { get; set; }

        public long nonce_timestamp { get; set; }
    }

    public class personJob : TableStorageEntity
    {
        public personJob()
        {
            Debug.Assert(!string.IsNullOrEmpty(person_id.ToString()));
            Timestamp = DateTime.UtcNow;
            PartitionKey = person_id.ToString();
            RowKey = XmlConvert.ToString(Timestamp, XmlDateTimeSerializationMode.Utc);
        }


        public int person_id { get; set; }

        public int organization_id { get; set; }
    }

    public class person_languages_spoken : TableStorageEntity
    {
        public person_languages_spoken()
        {
            Debug.Assert(!string.IsNullOrEmpty(person_id.ToString()));
            Timestamp = DateTime.UtcNow;
            PartitionKey = person_id.ToString();
            RowKey = XmlConvert.ToString(Timestamp, XmlDateTimeSerializationMode.Utc);
        }


        public int person_id { get; set; }

        public int language_id { get; set; }
    }

    public class personQuote : TableStorageEntity
    {
        public personQuote()
        {
            Debug.Assert(!string.IsNullOrEmpty(person_id.ToString()));
            Timestamp = DateTime.UtcNow;
            PartitionKey = person_id.ToString();
            RowKey = XmlConvert.ToString(Timestamp, XmlDateTimeSerializationMode.Utc);
        }


        public int person_id { get; set; }

        public string quote { get; set; }
    }

    public class personSchool : TableStorageEntity
    {
        public personSchool()
        {
            Debug.Assert(!string.IsNullOrEmpty(person_id.ToString()));
            Timestamp = DateTime.UtcNow;
            PartitionKey = person_id.ToString();
            RowKey = XmlConvert.ToString(Timestamp, XmlDateTimeSerializationMode.Utc);
        }


        public int person_id { get; set; }

        public int organization_id { get; set; }
    }


    public class personSport : TableStorageEntity
    {
        public personSport()
        {
            Debug.Assert(!string.IsNullOrEmpty(person_id.ToString()));
            Timestamp = DateTime.UtcNow;
            PartitionKey = person_id.ToString();
            RowKey = XmlConvert.ToString(Timestamp, XmlDateTimeSerializationMode.Utc);
        }


        public int person_id { get; set; }

        public string sport { get; set; }
    }


    public class personTag : TableStorageEntity
    {
        public personTag()
        {
            Debug.Assert(!string.IsNullOrEmpty(person_id.ToString()));
            Timestamp = DateTime.UtcNow;
            PartitionKey = person_id.ToString();
            RowKey = XmlConvert.ToString(Timestamp, XmlDateTimeSerializationMode.Utc);
        }


        public int person_id { get; set; }

        public string tag { get; set; }
    }


    public class personTurnOff : TableStorageEntity
    {
        public personTurnOff()
        {
            Debug.Assert(!string.IsNullOrEmpty(person_id.ToString()));
            Timestamp = DateTime.UtcNow;
            PartitionKey = person_id.ToString();
            RowKey = XmlConvert.ToString(Timestamp, XmlDateTimeSerializationMode.Utc);
        }


        public int person_id { get; set; }

        public string turn_off { get; set; }
    }


    public class personTurnOn : TableStorageEntity
    {
        public personTurnOn()
        {
            Debug.Assert(!string.IsNullOrEmpty(person_id.ToString()));
            Timestamp = DateTime.UtcNow;
            PartitionKey = person_id.ToString();
            RowKey = XmlConvert.ToString(Timestamp, XmlDateTimeSerializationMode.Utc);
        }


        public int person_id { get; set; }

        public string turn_on { get; set; }
    }


    public class personTvShow : TableStorageEntity
    {
        public personTvShow()
        {
            Debug.Assert(!string.IsNullOrEmpty(person_id.ToString()));
            Timestamp = DateTime.UtcNow;
            PartitionKey = person_id.ToString();
            RowKey = XmlConvert.ToString(Timestamp, XmlDateTimeSerializationMode.Utc);
        }


        public int person_id { get; set; }

        public string tv_show { get; set; }
    }

    public class tag : TableStorageEntity
    {
        public tag()
        {
            Debug.Assert(id != 0);
            Timestamp = DateTime.UtcNow;
            PartitionKey = weight.ToString();
            RowKey = XmlConvert.ToString(Timestamp, XmlDateTimeSerializationMode.Utc);
        }


        public int id { get; set; }

        public string name { get; set; }

        public double weight { get; set; }

        public int type { get; set; }
    }
}
