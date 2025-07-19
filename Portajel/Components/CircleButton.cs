using System.Windows.Input;
using CommunityToolkit.Maui.Behaviors;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;

namespace Portajel.Components
{
    public class CircleButton : ContentView
    {
        private static readonly BindableProperty SizeProperty =
            BindableProperty.Create(
                nameof(Size),
                typeof(int),
                typeof(CircleButton),
                64,
                propertyChanged: OnSizeChanged);
        public int Size
        {
            get => (int)GetValue(SizeProperty);
            set => SetValue(SizeProperty, value);
        }

        private static readonly BindableProperty ImageSizeProperty =
            BindableProperty.Create(
                nameof(ImageSize),
                typeof(int),
                typeof(CircleButton),
                64, 
                propertyChanged: OnSizeChanged);
        public int ImageSize
        {
            get => (int)GetValue(ImageSizeProperty);
            set => SetValue(ImageSizeProperty, value);
        }

        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create(
                nameof(ImageSource),
                typeof(ImageSource),
                typeof(CircleButton),
                default(ImageSource),
                propertyChanged: OnImageSourceChanged);
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public static readonly BindableProperty BackgroundColorProperty =
            BindableProperty.Create(
                nameof(BackgroundColor),
                typeof(Color),
                typeof(CircleButton),
                Colors.Transparent,
                propertyChanged: OnButtonBackgroundColorChanged);
        public Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }
        public static readonly BindableProperty ImageColorProperty =
            BindableProperty.Create(
                nameof(ImageColor),
                typeof(Color),
                typeof(CircleButton),
                Colors.White,
                propertyChanged: OnImageColorChanged);

        public Color ImageColor
        {
            get => (Color)GetValue(ImageColorProperty);
            set => SetValue(ImageColorProperty, value);
        }
        
        public event EventHandler Clicked;

        private Border _border;
        private Button _button;
        private Image _image;

        public CircleButton()
        {
            _border = new Border
            {
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle()
            };

            _button = new Button
            {
                ZIndex = 1
            };

            _image = new Image
            {
                InputTransparent = true,
                ZIndex = 2,
                Behaviors =
                {
                    new IconTintColorBehavior()
                    {
                        BindingContext = this
                    }
                }
            };

            var tintBehavior = (IconTintColorBehavior)_image.Behaviors[0];
            tintBehavior.SetBinding(IconTintColorBehavior.TintColorProperty, new Binding(nameof(ImageColor), source: this));

            var grid = new Grid();
            grid.Children.Add(_button);
            grid.Children.Add(_image);

            _border.Content = grid;
            Content = _border;

            // Bind the Button's BackgroundColor to the property
            _button.SetBinding(Button.BackgroundColorProperty, new Binding(nameof(BackgroundColor), source: this));
            _button.Clicked += OnButtonClicked;
            
            UpdateSize();
        }
        private static void OnSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CircleButton circleButton)
            {
                circleButton.UpdateSize();
                circleButton.UpdateSize();
            }
        }
        private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CircleButton circleButton && newValue is ImageSource newSource)
            {
                circleButton._image.Source = newSource;
            }
        }
        private static void OnButtonBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            
        }
        private static void OnImageColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CircleButton circleButton && newValue is string newSource)
            {
                
            }
        }
        private void OnButtonClicked(object sender, EventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
        private void UpdateSize()
        {
            WidthRequest = Size;
            HeightRequest = Size;
            _image.WidthRequest = ImageSize;
            _image.HeightRequest = ImageSize;
            if (_border?.StrokeShape is RoundRectangle roundRect)
            {
                roundRect.CornerRadius = new CornerRadius(Size / 2.0);
                _border.InvalidateMeasure();
            }
        }
    }
}