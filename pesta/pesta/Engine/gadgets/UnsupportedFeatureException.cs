using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pesta
{
    public class UnsupportedFeatureException : GadgetException
    {
        public UnsupportedFeatureException(String name)
            : base(GadgetException.Code.UNSUPPORTED_FEATURE,
                "Unsupported feature: " + name)
        {
            
        }
    }
}
