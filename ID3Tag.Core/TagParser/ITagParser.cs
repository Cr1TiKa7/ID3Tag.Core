using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.Core.TagParser
{
    public interface ITagParser
    {
        ID3TagObject Read(string fileName);
    }
}
