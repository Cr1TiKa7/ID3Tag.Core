namespace ID3Tag.Core.ID3v1
{
    public class ID3v1ResponseObject
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Year { get; set; }
        public string Comment { get; set; }
        public int GenreID { get; set; }
        public int Track { get; set; }
        /// <summary>
        /// ID3v1+ Tags only
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// ID3v1+ Tags only
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// ID3v1+ Tags only
        /// </summary>
        public string EndTime { get; set; }
    }
}
