using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace ID3TagReader_Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            //ID3Tag.Core.TagParser.ID3v1Parser reader = new ID3Tag.Core.TagParser.ID3v1Parser();
            ID3Tag.Core.TagParser.ID3v2Parser reader = new ID3Tag.Core.TagParser.ID3v2Parser();
            ID3Tag.Core.ID3TagObject resp = reader.Read("test.mp3");
            if (resp != null)
            {
                Console.WriteLine("Album title: " + resp.Album);
                Console.WriteLine("Artist: " + resp.Artist);
                Console.WriteLine("Comment: " + resp.Comment);
                Console.WriteLine("Composer: " + resp.Composer);
                Console.WriteLine("Copyright: " + resp.Copyright);
                Console.WriteLine("Title: " + resp.Title);
                Console.WriteLine("Tracknumber: " + resp.Track);
            }

            Console.ReadLine();
        }
    }
}
