using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Graphics;

namespace WpfXnaControl
{
    public class XnaImageSource : IDisposable
    {
        public RenderTarget2D RenderTarget { get; private set; }
        public WriteableBitmap WriteableBitmap { get; private set; }
        private byte[] buffer;

        public XnaImageSource(GraphicsDevice graphics, int width, int height)
        {
            RenderTarget = new RenderTarget2D(graphics, width, height, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
            buffer = new byte[width * height * 4];
            WriteableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
        }

        ~XnaImageSource()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            RenderTarget.Dispose();

            if (disposing)
                GC.SuppressFinalize(this);
        }

        public void Commit()
        {
            RenderTarget.GetData(buffer);

            for (int i = 0; i < buffer.Length - 2; i += 4)
            {
                byte r = buffer[i];
                buffer[i] = buffer[i + 2];
                buffer[i + 2] = r;
            }

            WriteableBitmap.Lock();
            Marshal.Copy(buffer, 0, WriteableBitmap.BackBuffer, buffer.Length);
            WriteableBitmap.AddDirtyRect(
                new Int32Rect(0, 0, RenderTarget.Width, RenderTarget.Height));
            WriteableBitmap.Unlock();
        }
    }
}