using ID3Tag.Core.Exceptions;
using ID3Tag.Core.ID3v2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

        #region "Frame headers"
        //Frame Header MajorVersion 2
        private const string TITLE_HEADER_V2 = "TT2";
        private const string ARTIST_HEADER_V2 = "TP1";
        private const string ALBUM_HEADER_V2 = "TAL";
        private const string YEAR_HEADER_V2 = "TYE";
        private const string TRACK_HEADER_V2 = "TRK";
        private const string GENRE_HEADER_V2 = "TCO";
        private const string COMMENT_HEADER_V2 = "COM";

        //Frame Header MajorVersion 3/4
        //URL to all frame headers: http://id3.org/id3v2.4.0-frames

        private const string COMMENT_HEADER_V34 = "COMM";

        private const string ALBUM_HEADER_V34 = "TALB";
        private const string BPM_HEADER_V34 = "TBPM";
        private const string GENRE_HEADER_V34 = "TCON";
        private const string COMPOSER_HEADER_V34 = "TCON";
        private const string COPYRIGHT_HEADER_V34 = "TCOP";
        private const string TEXTWRITER_HEADER_V34 = "TEXT";
        private const string TITLE_HEADER_V34 = "TIT2";
        private const string LANGUAGE_HEADER_V34 = "TLAN";
        private const string LENGTH_HEADER_V34 = "TLEN";
        private const string ARTIST_HEADER_V34 = "TPE1";
        private const string TRACK_HEADER_V34 = "TRCK";
        private const string YEAR_HEADER_V34 = "TYER";

        #endregion

        //List of Frames and FrameHashes that were found
        private List<ID3v2Frame> _Frames = new List<ID3v2Frame>();
        private Hashtable _FrameHashes = new Hashtable();

        /// <summary>
        /// Reads in the given file at the filepath and returns an object to get all the ID3 Content.
        /// </summary>
        /// <param name="fileName">Filepath of the file to parse</param>
        /// <returns>Returns an ID3TagObject to work with.</returns>
        public ID3TagObject Read(string fileName)
        {
            ID3TagObject ret = null;

            if (!File.Exists(fileName))
                return ret;

            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fs))
                {
                    ret = new ID3TagObject()
                    {
                         ID3v2Parser = this,
                         Type = ID3Type.ID3v2
                    };
                    ReadHeader(binaryReader);

                    if (_ExtendedHeaderFlag)
                        ReadExtendedHeader(binaryReader);

                    ReadFrames(binaryReader);

                    if (_FooterPresentTag)
                        ReadFooter(binaryReader);

                    ReadFramesByHeader(ret);
                }
        }
            return ret;
        }

        private void ReadFooter(BinaryReader binaryReader)
        {
            string id3v2FooterTag = new string(binaryReader.ReadChars(_TagLength));

            //If a footer exists the tag is 3DI instead of ID3
            if (id3v2FooterTag == "3DI")
            {
                //Read the version number of id3v2
                _MajorVersion = Convert.ToInt32(binaryReader.ReadByte());
                _MinorVersion = Convert.ToInt32(binaryReader.ReadByte());

                if (_MajorVersion <= 4)
                {
                    bool[] flagByte = BitReader.GetBitArrayByByte(binaryReader.ReadByte());
                    switch (_MajorVersion)
                    {
                        case 2:
                            {
                                _UnsynchronisationFlag = flagByte[0];
                                _ExtendedHeaderFlag = flagByte[1];
                                break;
                            }
                        case 3:
                            {
                                _UnsynchronisationFlag = flagByte[0];
                                _ExtendedHeaderFlag = flagByte[1];
                                _ExperimentalIndicatorFlag = flagByte[2];
                                break;
                            }
                        case 4:
                            {
                                _UnsynchronisationFlag = flagByte[0];
                                _ExtendedHeaderFlag = flagByte[1];
                                _ExperimentalIndicatorFlag = flagByte[2];
                                _FooterPresentTag = flagByte[3];
                                break;
                            }
                    }

                    char[] tagSize = binaryReader.ReadChars(4);
                    int[] bytes = new int[4];
                    ulong newSize = 0;

                    bytes[3] = tagSize[3] | ((tagSize[2] & 1) << 7);
                    bytes[2] = ((tagSize[2] >> 1) & 63) | ((tagSize[1] & 3) << 6);
                    bytes[1] = ((tagSize[1] >> 2) & 31) | ((tagSize[0] & 7) << 5);
                    bytes[0] = ((tagSize[0] >> 3) & 15);

                    newSize = ((UInt64)10 + (UInt64)bytes[3] |
                        ((UInt64)bytes[2] << 8) |
                        ((UInt64)bytes[1] << 16) |
                        ((UInt64)bytes[0] << 24));

                    _HeaderSize = newSize;
                }
            }
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

                char[] tagSize = binaryReader.ReadChars(4);  
                int[] bytes = new int[4]; 
                ulong newSize = 0;

                bytes[3] = tagSize[3] | ((tagSize[2] & 1) << 7);
                bytes[2] = ((tagSize[2] >> 1) & 63) | ((tagSize[1] & 3) << 6);
                bytes[1] = ((tagSize[1] >> 2) & 31) | ((tagSize[0] & 7) << 5);
                bytes[0] = ((tagSize[0] >> 3) & 15);

                newSize = ((UInt64)10 + (UInt64)bytes[3] |
                    ((UInt64)bytes[2] << 8) |
                    ((UInt64)bytes[1] << 16) |
                    ((UInt64)bytes[0] << 24));

                _HeaderSize = newSize;
            }
            else
            {
                throw new ID3TagCouldntBeenFoundException("The ID3v2 Tag couldn't been found. Please make sure the file has an ID3v2 Tag");
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
                    throw new ID3v2HeaderCorruptedException("The Header of the file is corrupted. Please try another file.");
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
            ID3v2Frame frame;
            do
            {
                frame = ID3v2Frame.ReadFrame(binaryReader, _MajorVersion);
                if (frame.Padding)
                {
                    binaryReader.BaseStream.Position = Convert.ToInt64(_HeaderSize);
                    break;
                }
                _Frames.Add(frame);
                _FrameHashes.Add(frame.Name, frame);

            } while (binaryReader.BaseStream.Position <= Convert.ToInt64(_HeaderSize));
        }

        private void ReadFramesByHeader(ID3TagObject id3TagObject)
        {
            if (_MajorVersion == 2)
            {
                id3TagObject.Title = GetFrameDataByHeaderName(TITLE_HEADER_V2, false);
                id3TagObject.Artist = GetFrameDataByHeaderName(ARTIST_HEADER_V2, false);
                id3TagObject.Album = GetFrameDataByHeaderName(ALBUM_HEADER_V2, false);
                id3TagObject.Year = GetFrameDataByHeaderName(YEAR_HEADER_V2, false);
                string[] tracks = GetFrameDataByHeaderName(TRACK_HEADER_V2, false).Split('/');
                if (tracks.Length > 0 && !string.IsNullOrEmpty(tracks[0]))
                    id3TagObject.Track = Convert.ToInt32(tracks[0]);
                if (tracks.Length > 1 && !string.IsNullOrEmpty(tracks[1]))
                    id3TagObject.TotalTracks = Convert.ToInt32(tracks[1]);
                //if (!string.IsNullOrEmpty(GetFrameDataByHeaderName(GENRE_HEADER_V2, false)))
                    id3TagObject.Genre = GetFrameDataByHeaderName(GENRE_HEADER_V2, false);
                id3TagObject.Comment = GetFrameDataByHeaderName(COMMENT_HEADER_V2, false);
            }
            else if (_MajorVersion > 2)
            {

                id3TagObject.BeatsPerMinute = GetFrameDataByHeaderName(BPM_HEADER_V34, false);
                id3TagObject.Length = GetFrameDataByHeaderName(LENGTH_HEADER_V34, false);

                id3TagObject.Artist = GetFrameDataByHeaderName(ARTIST_HEADER_V34, false);
                id3TagObject.Album = GetFrameDataByHeaderName(ALBUM_HEADER_V34, false);
                id3TagObject.Comment = GetFrameDataByHeaderName(COMMENT_HEADER_V34, false);
                id3TagObject.Composer = GetFrameDataByHeaderName(COMPOSER_HEADER_V34, false);
                id3TagObject.Copyright = GetFrameDataByHeaderName(COPYRIGHT_HEADER_V34, false);
                id3TagObject.Language = GetFrameDataByHeaderName(LANGUAGE_HEADER_V34, false);
                id3TagObject.Textwriter = GetFrameDataByHeaderName(TEXTWRITER_HEADER_V34, false);
                id3TagObject.Title = GetFrameDataByHeaderName(TITLE_HEADER_V34, false);
                id3TagObject.Year = GetFrameDataByHeaderName(YEAR_HEADER_V34, false);

                string[] tracks = GetFrameDataByHeaderName(TRACK_HEADER_V34, true).Split('/');
               if (tracks.Length > 0 && !string.IsNullOrEmpty(tracks[0]))
                    id3TagObject.Track = Convert.ToInt32(tracks[0]);
                if (tracks.Length > 1 && !string.IsNullOrEmpty(tracks[1]))
                    id3TagObject.TotalTracks = Convert.ToInt32(tracks[1]);
                if (!string.IsNullOrEmpty(GetFrameDataByHeaderName(GENRE_HEADER_V34, false)))
                    id3TagObject.Genre = GetFrameDataByHeaderName(GENRE_HEADER_V34, false);
            }
        }

        /// <summary>
        /// Allows you to get the content of a frame by yourself
        /// </summary>
        /// <param name="frameKey">Framekeys: http://id3.org/id3v2.4.0-frames </param>
        /// <param name="useEncoding">If the framekey has an encoding set this value to true. Default value is false</param>
        /// <returns></returns>

        internal string GetFrameDataByHeaderName(string frameKey, bool useEncoding)
        {
            int i = 0;
            Encoding charEncoding = Encoding.Unicode;
            if (!useEncoding)
                i = 3;
                
            if (_FrameHashes.Contains(frameKey))
            {
                byte[] bytes = ((ID3v2Frame)_FrameHashes[frameKey]).FrameContent;
                StringBuilder stringBuilder = new StringBuilder();
                byte encoding;

                for (int j = i; j < bytes.Length; j++)
                {
                    //If byte value is 0 skip it else you get string like 'T E S T ' instea of 'TEST'
                    if (bytes[j] == 0)
                        continue;

                    if (j == 0)
                        encoding = bytes[j];
                    else
                        stringBuilder.Append(Convert.ToChar(bytes[j]));
                }
               return stringBuilder.ToString();
                //return stringBuilder.ToString();
            }
            return "";
        }
    }
}
