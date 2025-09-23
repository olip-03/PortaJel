using CommunityToolkit.Maui.Behaviors;
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
                propertyChanged: OnImageSourceChanged);
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        public new static readonly BindableProperty BackgroundColorProperty =
            BindableProperty.Create(
                nameof(BackgroundColor),
                typeof(Color),
                typeof(CircleButton),
                Colors.Transparent);
        public new Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }
        
        public static readonly BindableProperty ImageColorProperty =
            BindableProperty.Create(
                nameof(ImageColor),
                typeof(Color),
                typeof(CircleButton),
                Colors.White);
        public Color ImageColor
        {
            get => (Color)GetValue(ImageColorProperty);
            set => SetValue(ImageColorProperty, value);
        }
        
        public static readonly BindableProperty TextProperty = 
            BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(CircleButton), propertyChanged: TextPropertyChanged);

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        
        public event EventHandler? Clicked;

        private readonly Border _border;
        private readonly ImageButton _button;
        private readonly Image _image;

        public CircleButton()
        {
            _border = new Border
            {
                StrokeThickness = 0,
                StrokeShape = new RoundRectangle()
            };

            _button = new ImageButton()
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
            Render();
            
            // Bind the Button's BackgroundColor to the property
            _button.SetBinding(Button.BackgroundColorProperty, new Binding(nameof(BackgroundColor), source: this));
            _button.Clicked += OnButtonClicked;
            
            UpdateSize();
        }

        public void Render()
        {
            var grid = new Grid();
            grid.Children.Add(_button);

            if (string.IsNullOrWhiteSpace(Text))
            {
                grid.Children.Add(_image);
            }
            else
            {
                var margin = 12;
                HorizontalStackLayout stack = new();
                Label text = new()
                {
                    VerticalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(margin * 0.7, 0, margin, 0),
                };
                _image.Margin = new Thickness(margin, 0, 0, 0);
                text.SetBinding(Label.TextProperty, new Binding(nameof(Text), source: this));
                
                stack.Children.Add(_image);
                stack.Children.Add(text);
                
                grid.Children.Add(stack);
            }
            _border.Content = grid;    
            Content = _border;
        }
        
        private static void OnSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CircleButton circleButton)
            {
                circleButton.UpdateSize();
                circleButton.UpdateSize();
            }
        }
        
        private static void TextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CircleButton circleButton)
            {
                circleButton.Render();
            }
        }
        
        private static void OnImageSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CircleButton circleButton && newValue is ImageSource newSource)
            {
                circleButton._image.Source = newSource;
            }
        }

        private void OnButtonClicked(object? sender, EventArgs e)
        {
            Clicked?.Invoke(this, e);
        }
        
        private void UpdateSize()
        {
            _border.MinimumWidthRequest = Size;
            _border.MinimumHeightRequest = Size;
            _image.WidthRequest = ImageSize;
            _image.HeightRequest = ImageSize;
            if (_border.StrokeShape is not RoundRectangle roundRect) return;
            roundRect.CornerRadius = new CornerRadius(Size);
            _border.InvalidateMeasure();
        }
    }
}