namespace IPPaint
{
    public class Settings
    {
        public static string ipRangesParentDirectory = @"IP Ranges\";
        public static List<IpRangeFileDirectory> IpRangeDirectories = new List<IpRangeFileDirectory>()
        {
            new IpRangeFileDirectory("PrivateInternet", 0x3),
            new IpRangeFileDirectory("Multicast", 0x7),
            new IpRangeFileDirectory("LoopbackAndSoftware", 0x1),
            new IpRangeFileDirectory("FutureUse", 0x8),
            new IpRangeFileDirectory("CGNat", 0x9),
            new IpRangeFileDirectory("Faang", 0xA),
            new IpRangeFileDirectory("Goverment", 0x4),
            new IpRangeFileDirectory("PrivateBusiness", 0x6)
        };
    }
}
