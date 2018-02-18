using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.Core.Exceptions
{
    internal class ID3TagCouldntBeenFoundException : Exception
    {
        public ID3TagCouldntBeenFoundException(string msg) : base(msg)
        {
        }
    }
}
