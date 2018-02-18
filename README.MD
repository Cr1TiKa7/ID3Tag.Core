# ID3Tag.Core 

The ID3Tag.Core is a library to read ID3v1 and ID3v2 Tags from .mp3 files.
This library supports .Net 4.0, 4.5, 4.6 and .NetStandard 2.0.

# Current Status
The Library is ready to build but still needs improvements and bug fixes (i am currently working on).

# Usage
```sh
 ID3Tag.Core.TagParser.ID3v1Parser reader = new ID3Tag.Core.TagParser.ID3v1Parser();
//ID3Tag.Core.TagParser.ID3v2Parser reader = new ID3Tag.Core.TagParser.ID3v2Parser();
ID3Tag.Core.ID3TagObject resp = reader.Read("test.mp3");
if (resp != null)
{
    Console.WriteLine(resp.Artist);
    Console.WriteLine(resp.Album);
    Console.WriteLine(resp.Comment);
}
