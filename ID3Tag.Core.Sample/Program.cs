using ID3TagReader.ID3v1;
using System;

namespace ID3TagReader_Sample
{
    class Program
    {

        static void Main(string[] args)
        {
            ID3v1Reader reader = new ID3v1Reader();
            ID3v1Object resp = reader.Read("test.mp3");
            if (resp != null)
                Console.WriteLine(resp.Artist);
            Console.ReadLine();
        }
    }
}
