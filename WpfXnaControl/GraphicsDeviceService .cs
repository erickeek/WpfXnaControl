using System;
using System.Threading;
using Microsoft.Xna.Framework.Graphics;

namespace WpfXnaControl
{
    public class GraphicsDeviceService : IGraphicsDeviceService
    {
        private static GraphicsDeviceService _singletonInstance;

        private static int _referenceCount;

        public static GraphicsDeviceService Instance
        {
            get { return _singletonInstance ?? (_singletonInstance = new GraphicsDeviceService()); }
        }

        private PresentationParameters _parameters;

        public GraphicsDevice GraphicsDevice { get; private set; }

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;

        GraphicsDeviceService() { }

        private void CreateDevice(IntPtr windowHandle)
        {
            _parameters = new PresentationParameters();
            _parameters.BackBufferWidth = 480;
            _parameters.BackBufferHeight = 320;
            _parameters.BackBufferFormat = SurfaceFormat.Color;
            _parameters.DeviceWindowHandle = windowHandle;
            _parameters.DepthStencilFormat = DepthFormat.Depth24Stencil8;
            _parameters.IsFullScreen = false;

            GraphicsDevice = new GraphicsDevice(
                GraphicsAdapter.DefaultAdapter,
                GraphicsProfile.HiDef,
                _parameters);

            if (DeviceCreated != null)
                DeviceCreated(this, EventArgs.Empty);
        }

        public static GraphicsDeviceService AddRef(IntPtr windowHandle)
        {
            if (Interlocked.Increment(ref _referenceCount) == 1)
            {
                Instance.CreateDevice(windowHandle);
            }

            return _singletonInstance;
        }

        public void Release()
        {
            if (Interlocked.Decrement(ref _referenceCount) != 0)
                return;

            if (DeviceDisposing != null)
                DeviceDisposing(this, EventArgs.Empty);

            GraphicsDevice.Dispose();

            GraphicsDevice = null;
        }
    }
}