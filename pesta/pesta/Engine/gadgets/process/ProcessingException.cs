using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pesta
{
    public class ProcessingException : Exception
    {
        public ProcessingException(Exception t)
            : base("",t)
        {
            
        }

        public ProcessingException(String message)
            : base(message)
        {
            
        }

        public ProcessingException(String message, Exception t)
            : base(message,t)
        {
           
        }
    }
}
