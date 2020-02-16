using System.Drawing;
using TryFreetype.Model;

namespace TryFreetype
{
    public class OutlineMask
    {
        internal const byte OutlineBit = 0x80;

        private Figure _figure;
        private byte[,] _buffer;

        public OutlineMask(Figure figure, byte[,] maskBuf)
        {
            _figure = figure;
            _buffer = maskBuf;
        }

        public Bitmap RenderBitmap()
        {
            var color = Color.Red;
            var bitmap = new Bitmap(_figure.Width, _figure.Height);

            for (int y = 0; y < _figure.Height; y++)
            {
                for (int x = 0; x < _figure.Width; x++)
                {
                    if ((_buffer[y, x] & OutlineBit) != 0)
                        bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }
    }
}
