namespace IPPaint
{
    public class IpRangeFileDirectory
    {
        string folderName { get; set; }
        byte value { get; set; }
        public IpRangeFileDirectory(string folderName, byte value)
        {
            this.folderName = folderName;
            this.value = value;
        }

        public string getDirectory()
        {
            return Settings.ipRangesParentDirectory + folderName;
        }
        public byte getValue()
        {
            return value;
        }
    }
}
