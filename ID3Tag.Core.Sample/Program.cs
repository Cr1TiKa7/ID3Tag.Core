using System;
namespace ID3TagReader_Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            ID3Tag.Core.TagParser.ID3v2Parser reader = new ID3Tag.Core.TagParser.ID3v2Parser();
            ID3Tag.Core.ID3TagObject resp = reader.Read("test.mp3");
            if (resp != null)
                Console.WriteLine(resp.Artist);
            Console.ReadLine();
        }
    }
}
