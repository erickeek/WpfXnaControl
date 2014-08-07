WpfXnaControl
=============

XNA Control for WPF

[Nuget Package](https://www.nuget.org/packages/WpfXnaControl/)

How to use:

```xaml
<Window x:Class="NewProject.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:xna="clr-namespace:WpfXnaControl;assembly=WpfXnaControl"
        Title="MainWindow" Height="350" Width="525">
    <Grid>
        <xna:XnaControl x:Name="Control"/>
    </Grid>
</Window>
```
```cs
private SpriteBatch _spriteBatch;
private GraphicsDevice _graphicsDevice;

public MainWindow()
{
    InitializeComponent();

    Control.Initialize += Initialize;
    Control.Update += Update;
    Control.LoadContent += LoadContent;
    Control.Draw += Draw;
}

private Initialize() {
  _graphicsDevice = Control.GraphicsDevice;
  _spriteBatch = new SpriteBatch(_graphicsDevice);
}

private void LoadContent(ContentManager contentManager) { }

private void Update(GameTime gameTime) { }

private void Draw() { }
```

