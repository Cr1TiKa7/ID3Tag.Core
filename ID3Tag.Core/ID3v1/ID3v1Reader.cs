using System.IO;
using System.Text;

namespace ID3TagReader.ID3v1
{
    public class ID3v1Reader
    {
        //ID3 Index and length of fields
        private const int HEADER_INDEX = 0;
        private const int HEADER_LENGTH = 3;
        private const int TITLE_INDEX = 3;
        private const int TITLE_LENGTH = 30;
        private const int ARTIST_INDEX = 33;
        private const int ARTIST_LENGTH = 30;
        private const int ALBUM_INDEX = 63;
        private const int ALBUM_LENGTH = 30;
        private const int YEAR_INDEX = 93;
        private const int YEAR_LENGTH = 4;
        private const int COMMENT_INDEX = 97;
        private const int COMMENT_LENGTH = 4;
        private const int ZEROBINARY_INDEX = 125;
        private const int TRACK_INDEX = 126;
        private const int GENREID_INDEX = 127;

        //ID3+ Index and length of fields

        public ID3v1Object Read(string fileName)
        {
            ID3v1Object ret = null;
            if (!File.Exists(fileName))
                return ret;
            using (FileStream fs = new FileStream(fileName, FileMode.Open,FileAccess.Read))
            {
                byte[] id3buffer = new byte[128];
                string id3string = Encoding.ASCII.GetString(id3buffer);
                if (id3string.Substring(HEADER_INDEX,HEADER_LENGTH) == "TAG")
                {
                    ret = new ID3v1Object()
                    {
                        Title = id3string.Substring(TITLE_INDEX, TITLE_LENGTH),
                        Artist = id3string.Substring(ARTIST_INDEX, ARTIST_LENGTH),
                        Album = id3string.Substring(TITLE_INDEX, TITLE_LENGTH),
                        Year = id3string.Substring(YEAR_INDEX, YEAR_LENGTH),
                        Comment = id3string.Substring(COMMENT_INDEX, COMMENT_LENGTH),
                        GenreID = id3buffer[GENREID_INDEX]
                    };
                    if (id3buffer[ZEROBINARY_INDEX] == 0)
                    {
                       ret.Track = id3buffer[TRACK_INDEX];
                    } 
                }
            }
            return ret;
        }

    }
}
