namespace ID3Tag.Core
{
    public class ID3TagObject
    {
        public string Title { get; set; }
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
        public int GenreID { get; set; }
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
