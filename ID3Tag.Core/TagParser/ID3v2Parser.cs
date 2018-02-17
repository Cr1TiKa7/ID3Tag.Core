using System;
using System.IO;

namespace ID3Tag.Core.TagParser
{
    //I took all the informations about how ID3v2 works from this website: http://id3.org/id3v2.4.0-structure

    public class ID3v2Parser : ITagParser
    {
        //Field sizes
        private ulong _HeaderSize;
        private ulong _ExtendedHeaderSize;

        //Length of the fields
        private int _TagLength = 3;
        
        //Version of ID3v2
        private int _MajorVersion;
        private int _MinorVersion;

        //Header Flags
        private bool _UnsynchronisationFlag;
        private bool _ExtendedHeaderFlag;
        private bool _ExperimentalIndicatorFlag;
        private bool _FooterPresentTag;

        //ExtendedHeader Flags
        private bool _UpdatedTagFlag;
        private bool _CrcDataPresentFlag;
        private bool _TagRestrictionFlag;

        //ExtendedHeader Fields
        private byte _TagRestrictionField;


        public ID3TagObject Read(string fileName)
        {
            ID3TagObject ret = null;

            if (!File.Exists(fileName))
                return ret;

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                //Delcare a buffer to read
                byte[] id3buffer = new byte[128];
                BinaryReader binaryReader = new BinaryReader(fs);

                ReadHeader(binaryReader);
                
            }
            return ret;
        }

        private void ReadHeader(BinaryReader binaryReader)
        {
            string id3v2tag = new string(binaryReader.ReadChars(_TagLength));

            if (id3v2tag == "ID3")
            {
                //Read the version number of id3v2
                _MajorVersion = Convert.ToInt32(binaryReader.ReadByte());
                _MinorVersion = Convert.ToInt32(binaryReader.ReadByte());
                
                //Read the next byte to get the 4 flags
                bool[] flagByte = BitReader.GetBitArrayByByte(binaryReader.ReadByte());

                _UnsynchronisationFlag = flagByte[0];
                _ExtendedHeaderFlag = flagByte[1];
                _ExperimentalIndicatorFlag = flagByte[2];
                _FooterPresentTag = flagByte[3];
            }
            else
            {
                //TODO: Throw exception that the ID3 Tag couldn't been found.
            }
        }

        private void ReadExtendedHeader(BinaryReader binaryReader)
        {
            int numberOfFlags = 0;

            //Read the first 4 chars to get the size of the extended header
            char[] extendedHeaderSize = binaryReader.ReadChars(4);
            //Needed for bitshifting
            int[] bytes = new int[4];
            //The actual size of the header
            ulong extHeaderSize = 0;

            //Do some magic (Code by Daniel White)
            bytes[3] = extendedHeaderSize[3] | ((extendedHeaderSize[2] & 1) << 7);
            bytes[2] = ((extendedHeaderSize[2] >> 1) & 63) | ((extendedHeaderSize[1] & 3) << 6);
            bytes[1] = ((extendedHeaderSize[1] >> 2) & 31) | ((extendedHeaderSize[0] & 7) << 5);
            bytes[0] = ((extendedHeaderSize[0] >> 3) & 15);

            extHeaderSize = ((UInt64)10 + (UInt64)bytes[3] |
                ((UInt64)bytes[2] << 8) |
                ((UInt64)bytes[1] << 16) |
                ((UInt64)bytes[0] << 24));

            //Save the actual size of the header to our variable we declared at the top of the class.
            _ExtendedHeaderSize = extHeaderSize;

            //Read the number of flag bytes
            int flagByteCount = Convert.ToInt32(binaryReader.ReadByte());

            bool[] extendedHeaderFlags = BitReader.GetBitArrayByBytes(binaryReader.ReadBytes(flagByteCount));

            _UpdatedTagFlag = extendedHeaderFlags[1];
            _CrcDataPresentFlag = extendedHeaderFlags[2];
            _TagRestrictionFlag = extendedHeaderFlags[3];

            //Has no data but is required to read.
            if (_UpdatedTagFlag)
                binaryReader.ReadByte();
            
            if(_CrcDataPresentFlag)
            {
                //Read the length of the flag field. Must be a length of 5 bytes else it's corrupted
                int crcDataPresentLength = Convert.ToInt32(binaryReader.ReadByte());
                if (crcDataPresentLength == 5)
                {
                    char[] crcDataChars = binaryReader.ReadChars(5);
                    bytes = new int[4];
                    extHeaderSize = 0;

                    bytes[4] = crcDataChars[4] | ((crcDataChars[3] & 1) << 7);
                    bytes[3] = ((crcDataChars[3] >> 1) & 63) | ((crcDataChars[2] & 3) << 6);
                    bytes[2] = ((crcDataChars[2] >> 2) & 31) | ((crcDataChars[1] & 7) << 5);
                    bytes[1] = ((crcDataChars[1] >> 3) & 15) | ((crcDataChars[0] & 15) << 4);
                    bytes[0] = ((crcDataChars[0] >> 4) & 7);

                    extHeaderSize = ((UInt64)10 + (UInt64)bytes[4] |
                        ((UInt64)bytes[3] << 8) |
                        ((UInt64)bytes[2] << 16) |
                        ((UInt64)bytes[1] << 24) |
                        ((UInt64)bytes[0] << 32));

                    _ExtendedHeaderSize = extHeaderSize;
                }
                else
                {
                    //TODO: Throw corrupted header exception
                }
            }
            if (_TagRestrictionFlag)
            {
                //Read the length of the flag (always 1 byte)
                binaryReader.ReadByte();
                _TagRestrictionField = binaryReader.ReadByte();
            }
        }

        private void ReadFrames(BinaryReader binaryReader)
        {
            //Read the frames.
        }
    }
}
