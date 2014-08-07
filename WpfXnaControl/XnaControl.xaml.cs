using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace WpfXnaControl
{
    public partial class XnaControl : UserControl
    {
        private TimeSpan _totalTime;
        private TimeSpan _elapsedTime;

        private GraphicsDeviceService _graphicsDeviceService;
        private XnaImageSource _imageSource;

        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDeviceService.GraphicsDevice; }
        }

        public ServiceContainer Services
        {
            get { return _services; }
        }

        readonly ServiceContainer _services = new ServiceContainer();

        public Action Initialize;
        public Action<ContentManager> LoadContent;
        public Action<GameTime> Update;
        public Action Draw;
        public ContentManager ContentManager { get; private set; }

        public XnaControl()
        {
            InitializeComponent();
            Loaded += XnaControl_Loaded;
        }

        ~XnaControl()
        {
            _imageSource.Dispose();
            if (_graphicsDeviceService != null)
                _graphicsDeviceService.Release();
        }

        private void XnaControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this) == false)
            {
                InitializeGraphicsDevice();

                if (Initialize != null)
                    Initialize();

                if (LoadContent != null)
                {
                    ContentManager = new ContentManager(_services, "Content");
                    LoadContent(ContentManager);
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (!DesignerProperties.GetIsInDesignMode(this) && _graphicsDeviceService != null)
            {
                _imageSource.Dispose();
                _imageSource = new XnaImageSource(GraphicsDevice, (int)ActualWidth, (int)ActualHeight);
                RootImage.Source = _imageSource.WriteableBitmap;
            }

            base.OnRenderSizeChanged(sizeInfo);
        }

        private void InitializeGraphicsDevice()
        {
            if (_graphicsDeviceService != null)
                return;

            // Adiciona refefência do graphics device
            _graphicsDeviceService = GraphicsDeviceService.AddRef((PresentationSource.FromVisual(this) as HwndSource).Handle);

            // cria um image source
            _imageSource = new XnaImageSource(GraphicsDevice, (int)ActualWidth, (int)ActualHeight);
            RootImage.Source = _imageSource.WriteableBitmap;

            _services.AddService(typeof(IGraphicsDeviceService), _graphicsDeviceService);

            _totalTime = new TimeSpan(DateTime.Now.Ticks);
            _elapsedTime = new TimeSpan(DateTime.Now.Ticks);

            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }

        protected virtual void Render()
        {
            if (Update != null)
            {
                var now = new TimeSpan(DateTime.Now.Ticks);
                Update(new GameTime(now - _totalTime, now - _elapsedTime));
                _elapsedTime = new TimeSpan(DateTime.Now.Ticks);
            }

            if (Draw != null)
                Draw();
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            GraphicsDevice.SetRenderTarget(_imageSource.RenderTarget);
            Render();
            GraphicsDevice.SetRenderTarget(null);
            _imageSource.Commit();
        }
    }
}