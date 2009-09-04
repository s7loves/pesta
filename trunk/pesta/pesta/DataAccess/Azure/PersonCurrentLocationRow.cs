using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pesta.DataAccess.Azure
{
    // TODO: will this work?
    public class PersonCurrentLocationRow : PersonAddressRow
    {
        public PersonCurrentLocationRow()
        {
            
        }
        public PersonCurrentLocationRow(string partitionKey, string rowKey)
            : base(partitionKey, rowKey)
        {

        }
    }
}
