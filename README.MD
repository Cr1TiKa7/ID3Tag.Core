# ID3Tag.Core 

The ID3Tag.Core is a library to read ID3v1 and ID3v2 Tags from .mp3 files.
This library supports .Net 4.0, 4.5, 4.6 and .NetStandard 2.0.

# Current Status
Build: [![Build Status](https://travis-ci.org/Cr1TiKa7/ID3Tag.Core.svg?branch=master)](https://travis-ci.org/Cr1TiKa7/ID3Tag.Core)

The Library is ready to build but still needs improvements(i am currently working on).
Also not every Frame is implemented. Will add them in the next update.

# Usage
To simply retrieve informations from you .mp3 file you can use this library as shown below:
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
```

If you want to read not implemented frames (e.g. the attached picture) you can call the following method:
```sh
ID3Tag.Core.TagParser.ID3v2Parser reader = new ID3Tag.Core.TagParser.ID3v2Parser();
ID3Tag.Core.ID3TagObject resp = reader.Read("test.mp3");
if (resp != null)
{
    Console.WriteLine("Test: " + resp.GetFrameValue("APIC"));
}
```
