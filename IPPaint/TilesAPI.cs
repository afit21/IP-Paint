using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace IPPaint
{
    [ApiController]
    [Route("api/tiles")]
    public class TilesAPI : ControllerBase
    {
        [HttpGet("{z}-{x}-{y}.png")]
        public IActionResult GetTile(int z, int x, int y)
        {
            Bitmap tileBitmap = IpCanvasRenderer.GetTile(z, x, y); // Your method to generate a bitmap
            using var ms = new MemoryStream();
            tileBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png); // Save bitmap as PNG
            ms.Position = 0; // Reset stream position to the beginning
            return File(ms.ToArray(), "image/png");
        }
    }
}
