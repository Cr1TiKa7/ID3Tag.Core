using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.Core.Exceptions
{
    public class ID3v2HeaderCorruptedException : Exception
    {
        public ID3v2HeaderCorruptedException(string msg) : base(msg)
        {
        }
    }
}
