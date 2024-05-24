using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.InteropServices;

namespace IPPaint
{
    public class IpCanvasRenderer
    {
        public static IpCanvas ipCanvas { set; get;}
        static Dictionary<byte, Color> colorDict = new Dictionary<byte, Color>()
        {
            [0x1] = Color.White,
            [0x0] = Color.Black,
            [0x2] = Color.Red,
            [0x3] = Color.Green,
            [0x4] = Color.DarkBlue,
            [0x5] = Color.LightBlue,
            [0x6] = Color.Yellow,
            [0x7] = Color.Pink,
            [0x8] = Color.Brown,
            [0x9] = Color.Purple,
            [0xA] = Color.Coral
        };

        //Returns a tile mapping the IP range accross a hilbert curve
        public static Bitmap GetTile(int zoom, int tileX, int tileY)
        {
            //Sets TileSize and reductionFactor based on zoom
            int tileSize = (int)(32 * Math.Pow(2, zoom));
            int reductionFactor = (int)(256 / Math.Pow(4, zoom));

            //Defines new bitmap
            Bitmap tile = new Bitmap(tileSize, tileSize, PixelFormat.Format32bppArgb);
            BitmapData data = tile.LockBits(new Rectangle(0, 0, tileSize, tileSize), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            // Bytes per pixel for the specified pixel format
            int bytesPerPixel = Image.GetPixelFormatSize(data.PixelFormat) / 8;
            int byteCount = data.Stride * tileSize;
            byte[] pixels = new byte[byteCount];

            //adds each pixel in the requested tile
            Parallel.For(0, tileSize, j =>
            {
                for (int i = 0; i < tileSize; i++)
                {
                    int sourceX = (i * reductionFactor) + (tileX * tileSize * reductionFactor);
                    int sourceY = (j * reductionFactor) + (tileY * tileSize * reductionFactor);

                    Color color = GetColorFromIndex(HilbertCurve.XYToIndex(sourceX, sourceY, IpCanvas.WIDTH));
                    int colorIndex = j * data.Stride + i * bytesPerPixel;
                    pixels[colorIndex] = color.B;       // Blue
                    pixels[colorIndex + 1] = color.G;  // Green
                    pixels[colorIndex + 2] = color.R;  // Red
                    pixels[colorIndex + 3] = color.A;  // Alpha
                }
            });

            // Copy the RGB values back to the bitmap
            Marshal.Copy(pixels, 0, data.Scan0, pixels.Length);
            tile.UnlockBits(data);

            return tile;
        }

        private static Color GetColorFromIndex(uint index)
        {
            return colorDict[ipCanvas.GetByteValue(index)];
        }
    }
}
