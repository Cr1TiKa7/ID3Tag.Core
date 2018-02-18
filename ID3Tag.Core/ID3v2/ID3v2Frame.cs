using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ID3Tag.Core.ID3v2
{
    internal class ID3v2Frame
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
        /// Version of the frame
        /// </summary>
        public int MajorVersion { get; set; }



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
        

        public static ID3v2Frame ReadFrame(BinaryReader binaryReader, int version)
        {
            ID3v2Frame ret = new ID3v2Frame() ;

            //Verion 3 and 4 of the namesize is always 4.
            int nameSize = 4;

            if (version == 3)
                nameSize = 3;

            ret.Name = new string(binaryReader.ReadChars(nameSize));
            ret.MajorVersion = version;

            StringBuilder stringBuilder = new StringBuilder(0, nameSize);
            for (int i = 0; i < version; i++)
            {
                stringBuilder.Append(Convert.ToChar(0));
            }

            if (ret.Name.Equals(stringBuilder.ToString()))
            {
                ret.Padding = true;
                return ret;
            }

            //Get the size of the frame
            ret.FrameSize = GetFrameSize(binaryReader, version);

            //Read Tags
            if (version == 3)
            {
                bool[] bits = BitReader.GetBitArrayByByte(binaryReader.ReadByte());
                ret.TagAlterPreservation = bits[0];
                ret.FileAlterPreservation = bits[1];
                ret.ReadOnly = bits[2];

                bits = BitReader.GetBitArrayByByte(binaryReader.ReadByte());
                ret.Compression = bits[0];
                ret.Encryption = bits[1];
                ret.GroupingIdentity = bits[2];
            }
            else if (version == 4)
            {
                bool[] bits = BitReader.GetBitArrayByByte(binaryReader.ReadByte());
                ret.TagAlterPreservation = bits[1];
                ret.FileAlterPreservation = bits[2];
                ret.ReadOnly = bits[3];

                bits = BitReader.GetBitArrayByByte(binaryReader.ReadByte());
                ret.GroupingIdentity = bits[0];
                ret.Compression = bits[4];
                ret.Encryption = bits[5];
                ret.Unsynchronisation = bits[6];
                ret.DataLengthIndicator = bits[7];
            }

            if (ret.FrameSize > 0)
                ret.FrameContent = binaryReader.ReadBytes(Convert.ToInt32(ret.FrameSize));

            return ret;
        }

        private static ulong GetFrameSize(BinaryReader binaryReader, int version)
        {
            char[] frameTagSize;
            int[] bytes;
            ulong frameSize;

            int bytesToRead = 3;
            if (version > 2)
                bytesToRead = 4;
            //Version 2s size is always 3 Bytes, Everything 2 > is 4 bytes.
            frameTagSize = binaryReader.ReadChars(bytesToRead);
            bytes = new int[bytesToRead];
            frameSize = 0;

            switch (version)
            {
                case 2:
                    {
                        bytes[3] = frameTagSize[2] | ((frameTagSize[1] & 1) << 7);
                        bytes[2] = ((frameTagSize[1] >> 1) & 63) | ((frameTagSize[0] & 3) << 6);
                        bytes[1] = ((frameTagSize[0] >> 2) & 31);

                        frameSize = ((UInt64)bytes[3] |
                            ((UInt64)bytes[2] << 8) |
                            ((UInt64)bytes[1] << 16));
                        break;
                    }
                case 3:
                    {
                        bytes[3] = frameTagSize[3] | ((frameTagSize[2] & 1) << 7);
                        bytes[2] = ((frameTagSize[2] >> 1) & 63) | ((frameTagSize[1] & 3) << 6);
                        bytes[1] = ((frameTagSize[1] >> 2) & 31) | ((frameTagSize[0] & 7) << 5);
                        bytes[0] = ((frameTagSize[0] >> 3) & 15);

                        frameSize = ((UInt64)bytes[3] |
                            ((UInt64)bytes[2] << 8) |
                            ((UInt64)bytes[1] << 16) |
                            ((UInt64)bytes[0] << 24));
                        break;
                    }
                case 4:
                    {
                        bytes[3] = frameTagSize[3] | ((frameTagSize[2] & 1) << 7);
                        bytes[2] = ((frameTagSize[2] >> 1) & 63) | ((frameTagSize[1] & 3) << 6);
                        bytes[1] = ((frameTagSize[1] >> 2) & 31) | ((frameTagSize[0] & 7) << 5);
                        bytes[0] = ((frameTagSize[0] >> 3) & 15);

                        frameSize = ((UInt64)bytes[3] |
                            ((UInt64)bytes[2] << 8) |
                            ((UInt64)bytes[1] << 16) |
                            ((UInt64)bytes[0] << 24));
                        break;
                    }
            }

            return frameSize;
        }
    }
}
