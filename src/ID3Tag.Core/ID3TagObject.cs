namespace ID3Tag.Core
{
    public class ID3TagObject
    {
        public string Title { get; set; }
        /// <summary>
        /// Only ID3v2
        /// </summary>
        public string BeatsPerMinute { get; set; }
        /// <summary>
        /// Only ID3v2
        /// </summary>
        public string Copyright { get; set; }
        /// <summary>
        /// Only ID3v2
        /// </summary>
        public string Length { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        /// <summary>
        /// ID3v1 Tags only
        /// </summary>
        public string Year { get; set; }
        /// <summary>
        /// ID3v1 Tags only
        /// </summary>
        public string Comment { get; set; }
        public string Genre { get; set; }
        public int TotalTracks { get; set; }
        public int Track { get; set; }
        /// <summary>
        /// ID3v1 Extendedtags only
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// ID3v1 Extendedtags only
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// ID3v1 Extendedtags only
        /// </summary>
        public string EndTime { get; set; }
    }
}
