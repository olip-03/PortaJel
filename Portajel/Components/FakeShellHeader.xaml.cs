    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Portajel.Structures.ViewModels.Components;

    namespace Portajel.Components;

    public partial class FakeShellHeader : ContentView
    {
        public event EventHandler? BackButtonClicked;

        public static readonly BindableProperty IgnoreSystemHeaderProperty
            = BindableProperty.Create(
                nameof(IgnoreSystemHeader),
                typeof(bool),
                typeof(FakeShellHeader),
                false,
                propertyChanged: OnIgnoreSystemHeaderChanged);
        public bool IgnoreSystemHeader
        {
            get => (bool)GetValue(IgnoreSystemHeaderProperty);
            set => SetValue(IgnoreSystemHeaderProperty, value);
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text),
                typeof(string),
                typeof(FakeShellHeader),
                "",
                propertyChanged: OnTextChanged);
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        readonly double _defaultHeaderHeight;
        private FakeShellHeaderViewModel _viewModel;
    
        public FakeShellHeader()
        {
            _defaultHeaderHeight = (double)Application.Current.Resources["SystemHeaderHeight"];
            _viewModel = new()
            {
                SystemHeaderPadding = _defaultHeaderHeight,
                LabelText = Text
            };
            BindingContext = _viewModel;
            InitializeComponent();
        }

        public void UpdateText(string text)
        {
        
        }
    
        public void UpdateBackgroundOpacity(double opacity)
        {
            HeaderBackground.Opacity = opacity;
        }

        private async void CircleButton_OnClicked(object? sender, EventArgs e)
        {
            BackButtonClicked?.Invoke(sender, e);
        }

        static void OnIgnoreSystemHeaderChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            if (bindable is FakeShellHeader fh)
                fh.UpdateHeaderPadding();
        }

        static void OnTextChanged(
            BindableObject bindable,
            object oldValue,
            object newValue)
        {
            if (bindable is FakeShellHeader fh)
                fh._viewModel.LabelText = newValue.ToString() ?? "";
        }


        void UpdateHeaderPadding()
        {
            _viewModel.SystemHeaderPadding
                = IgnoreSystemHeader
                    ? 0
                    : _defaultHeaderHeight;
        }
    }