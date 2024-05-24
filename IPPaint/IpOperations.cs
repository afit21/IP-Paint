using System.Net;

namespace IPPaint
{
    public class IpOperations
    {
        public static uint[] RangeToIPUints(string range)
        {
            string[] parts = range.Split('/');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid CIDR notation");
            }

            string ipAddress = parts[0];
            int prefixLength = int.Parse(parts[1]);

            uint ipAsUint = IPAddressToUint(ipAddress);
            uint mask = ~(uint.MaxValue >> prefixLength);

            uint startIP = ipAsUint & mask;
            uint endIP = startIP | ~mask;

            return new uint[]
            {
                startIP,
                endIP
            };
        }

        public static uint IPAddressToUint(string ipAddress)
        {
            byte[] bytes = IPAddress.Parse(ipAddress).GetAddressBytes();
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}
