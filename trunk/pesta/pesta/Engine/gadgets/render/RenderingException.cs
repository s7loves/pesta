using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/**
 * Exceptions thrown during gadget rendering.
 *
 * These execeptions will usually translate directly into an end-user error message, so they should
 * be easily localizable.
 */
namespace Pesta
{
    public class RenderingException : Exception
    {
        public RenderingException(Exception t)
            : base("",t)
        {
            
        }

        public RenderingException(String message)
            : base(message)
        {
            
        }

        public RenderingException(String message, Exception t)
            : base(message,t)
        {
            
        }
    }
}
