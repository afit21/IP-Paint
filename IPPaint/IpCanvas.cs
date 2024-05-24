using IPPaint;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

public class IpCanvas
{
    public static readonly int WIDTH = 65536; // Conceptual WIDTH
    static readonly int ARRAY_MAX = 1073741824; //Maximum amount of bytes in a byte array

    // Storing Colours corresponding with each ip address
    // This is split into two byte arrays to get around the maximum byte array size
    private byte[] AIPs;
    private byte[] BIPs;

    public IpCanvas() {
        //Sets Canvas to all black
        var enumerable = Enumerable.Repeat((byte)0x0, ARRAY_MAX);
        AIPs = enumerable.ToArray();
        BIPs = enumerable.ToArray();

        addIpRangesFromSettings();
    }

    public void SetValueForIPRange(string startIp, string endIp, byte value)
    {
        string[] startParts = startIp.Split('.');
        string[] endParts = endIp.Split('.');

        // Extract start and end ranges for each octet
        int startFirstOctet = int.Parse(startParts[0]);
        int startSecondOctet = int.Parse(startParts[1]);
        int startThirdOctet = int.Parse(startParts[2]);
        int startFourthOctet = int.Parse(startParts[3]);

        int endFirstOctet = int.Parse(endParts[0]);
        int endSecondOctet = int.Parse(endParts[1]);
        int endThirdOctet = int.Parse(endParts[2]);
        int endFourthOctet = int.Parse(endParts[3]);

        // Generate IP addresses using Parallel.For for the second octet
        for (int first = startFirstOctet; first <= endFirstOctet; first++)
        {
            Parallel.For(startSecondOctet, endSecondOctet + 1, second =>
            {
                for (int third = startThirdOctet; third <= endThirdOctet; third++)
                {
                    for (int fourth = startFourthOctet; fourth <= endFourthOctet; fourth++)
                    {
                        SetByteValue(IpOperations.IPAddressToUint($"{first}.{second}.{third}.{fourth}"), value);
                    }
                }
            });
        }
    }

    public void SetValueForIPRange(uint startIp, uint endIp, byte value)
    {
        if (endIp == uint.MaxValue) endIp = uint.MaxValue - 1;
        for (uint i = startIp; i < endIp + 1; i++)
        {
            SetByteValue(i, value);
        }
    }

    public byte GetByteValue(uint index)
    {
        uint arrayIndex = index / 2; // Calculate which byte index
        bool isSecondNibble = (index % 2) != 0; // Determine if it's the first or second nibble

        byte[] targetArray = arrayIndex < ARRAY_MAX ? AIPs : BIPs;
        arrayIndex = (uint)(arrayIndex % ARRAY_MAX); // Adjust index for the target array

        byte targetByte = targetArray[arrayIndex];
        if (isSecondNibble) return (byte)((targetByte >> 4) & 0x0F); // Second nibble
        else return (byte)(targetByte & 0x0F); // First nibble
        
    }

    // Sets the nibble value in the combined nibble array
    public void SetByteValue(uint index, byte newByte)
    {
        uint arrayIndex = index / 2; // Calculate which byte index
        bool isSecondNibble = (index % 2) != 0; // Determine if it's the first or second nibble

        byte[] targetArray = arrayIndex < ARRAY_MAX ? AIPs : BIPs;
        arrayIndex = (uint)(arrayIndex % ARRAY_MAX); // Adjust index for the target array

        // Ensure that newByte is indeed a nibble
        newByte &= 0x0F;

        ref byte targetByte = ref targetArray[arrayIndex];
        if (isSecondNibble)
        {
            targetByte = (byte)((targetByte & 0x0F) | (newByte << 4)); // Set the second nibble
        }
        else
        {
            targetByte = (byte)((targetByte & 0xF0) | newByte); // Set the first nibble
        }
    }

    private void addValueToRangesInFile(string file, byte value)
    {
        using (var streamReader = File.OpenText(file))
        {
            var lines = streamReader.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Parallel.ForEach(lines, line =>
            {
                if (line.Contains('#')) return;
                var range = IpOperations.RangeToIPUints(line);
                SetValueForIPRange(range[0], range[1], value);
            });
        }
    }

    private void addValueToRangesInDirectory(string dir, byte value)
    {
        DirectoryInfo d = new DirectoryInfo(dir);
        FileInfo[] Files = d.GetFiles("*.txt"); //Getting Text files

        foreach (FileInfo file in Files)
        {
            addValueToRangesInFile(file.FullName, value);
        }
    }

    private void addIpRangesFromSettings()
    {
        foreach(IpRangeFileDirectory direct in Settings.IpRangeDirectories)
        {
            addValueToRangesInDirectory(direct.getDirectory(), direct.getValue());
        }
    }
}