using ID3Tag.Core.Exceptions;
using System;
using System.IO;
using System.Text;

namespace ID3Tag.Core.TagParser
{
    public class ID3v1Parser : ITagParser
    {
        //ID3 indeces and length of fields.
        private int _HeaderIndex = 0;
        private int _HeaderLength = 3;
        private int _TitleIndex = 3;
        private int _TitleLength = 30;
        private int _ArtistIndex = 33;
        private int _ArtistLength = 30;
        private int _AlbumIndex = 63;
        private int _AlbumLength = 30;
        private int _YearIndex = 93;
        private int _YearLength = 4;
        private int _CommentIndex = 97;
        private int _CommentLength = 4;
        private int _ZerobinaryIndex = 125;
        private int _TrackIndex = 126;
        private int _GenreIdIndex = 127;
        
        public ID3TagObject Read(string fileName)
        {
            ID3TagObject ret = null;

            if (!File.Exists(fileName))
                return ret;

            using (FileStream fs = new FileStream(fileName, FileMode.Open,FileAccess.Read))
            {
                //Delcare a buffer to read
                byte[] id3buffer = new byte[128];

                //Read the Last 128 Bytes of the file
                fs.Seek(id3buffer.Length * -1, SeekOrigin.End);
                fs.Read(id3buffer, 0, id3buffer.Length);
                
                //Read all bytes and convert them to a string
                string id3string = Encoding.ASCII.GetString(id3buffer);
                //Check if the ID3v1 Tag exists
                if (id3string.Substring(_HeaderIndex, _HeaderLength) == "TAG")
                {
                    ret = new ID3TagObject()
                    {
                        Title = id3string.Substring(_TitleIndex, _TitleLength),
                        Artist = id3string.Substring(_ArtistIndex, _ArtistLength),
                        Album = id3string.Substring(_TitleIndex, _TitleLength),
                        Year = id3string.Substring(_YearIndex, _YearLength),
                        Comment = id3string.Substring(_CommentIndex, _CommentLength),
                        Genre = (Genre)id3buffer[_GenreIdIndex]
                    };
                    if (id3buffer[_ZerobinaryIndex] == 0)
                    {
                       ret.Track = id3buffer[_TrackIndex];
                    }
                }
                else
                {
                    throw new ID3TagCouldntBeenFoundException("The ID3v1 Tag couldn't been found. Please make sure the file has an ID3v1 Tag");
                }
            }
            return ret;
        }

        private bool IsExtendedTag(byte[] id3buffer)
        {
            bool ret = false;
            if ( Convert.ToString(id3buffer[4]) == "+")
                ret = true;
            return ret;
        }

    }
}
