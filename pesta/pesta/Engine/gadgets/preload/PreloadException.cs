using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pesta
{
    public class PreloadException : Exception
    {
        public PreloadException(String msg)
            : base(msg)
        {
        }

        public PreloadException(Exception t)
            : base("", t)
        {
        }

        public PreloadException(String msg, Exception t)
            : base(msg,t)
        {

        }
    }
}
