namespace IPPaint
{
    public class HilbertCurve
    {
        public static uint XYToIndex(int x, int y, int width)
        {
            uint index = XYToD(width, x, y);

            return index;
        }

        // Convert (x, y) to d
        private static uint XYToD(int n, int x, int y)
        {
            int rx, ry;
            long s, d = 0;
            for (s = n / 2; s > 0; s /= 2)
            {
                rx = (x & s) > 0 ? 1 : 0;
                ry = (y & s) > 0 ? 1 : 0;
                d += s * s * ((3 * rx) ^ ry);
                Rotate(s, ref x, ref y, rx, ry);
            }
            return (uint)d;
        }


        // Rotate/flip a quadrant appropriately
        private static void Rotate(long n, ref int x, ref int y, int rx, int ry)
        {
            if (ry == 0)
            {
                if (rx == 1)
                {
                    x = (int)(n - 1 - x);
                    y = (int)(n - 1 - y);
                }

                // Swap x and y
                int t = x;
                x = y;
                y = t;
            }
        }
        
    }
}
