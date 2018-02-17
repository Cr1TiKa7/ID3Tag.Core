using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.Core.ID3v2
{
    public class ID3v2Frame
    {
        /// <summary>
        /// Name of the frame
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The content of the current frame
        /// </summary>
        public byte[] FrameContent { get; set; }
        /// <summary>
        /// Size of the frame in bytes
        /// </summary>
        public ulong FrameSize { get; set; }
        /// <summary>
        /// False = Frame should be preserved. True = Frame Should be discarded
        /// </summary>
        public bool TagAlterPreservation { get; set; }
        /// <summary>
        /// False = Frame should be preserved. True = Frame Should be discarded
        /// </summary>
        public bool FileAlterPreservation { get; set; }
        /// <summary>
        /// If true then don't try to overwrite this frame else the file could get corrupted.
        /// </summary>
        public bool ReadOnly { get; set; }
        /// <summary>
        /// False = Does not contain any group information. True = Frame contains group informations
        /// </summary>
        public bool GroupingIdentity { get; set; }
        /// <summary>
        /// False = Frame is not compressed. True = Is compressed.
        /// Compressed using zlib's deflate method. Also requires the 'Data Length Indicator' Property to be set.
        /// </summary>
        public bool Compression { get; set; }
        /// <summary>
        /// False = Frame is not encrypted. True = Frame is encrypted.
        /// Should be encrypted after the compression is done.
        /// </summary>
        public bool Encryption { get; set; }
        /// <summary>
        /// False = Frame is not unsynchronised. True = Frame is unsynchronised.
        /// </summary>
        public bool Unsynchronisation { get; set; }
        /// <summary>
        /// False = There is no Data Length Indicator. True = A data length Indicator has been added to the frame.
        /// </summary>
        public bool DataLengthIndicator { get; set; }
        /// <summary>
        /// False = Frame is not enlargres. True = Frame is enlarged.
        /// </summary>
        public bool Padding { get; set; }

        //TODO: Add a method to read the frame.
    }
}
