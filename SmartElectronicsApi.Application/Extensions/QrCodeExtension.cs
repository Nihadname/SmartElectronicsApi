


using System.Drawing;
using System.Drawing.Imaging;

namespace SmartElectronicsApi.Application.Extensions
{
    public static class QrCodeExtension
    {
        public static byte[] BitmapToByteArray(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
}
