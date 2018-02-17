using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.Core
{
    public class ID3v2TagObject
    {
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public bool Unsynchronisation { get; set; }
        public bool ExtendedHeader { get; set; }
        public bool ExperimentalIndecator { get; set; }
    }
}
