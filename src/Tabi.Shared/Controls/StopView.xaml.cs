using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Tabi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StopView : ContentView
    {
        public StopView()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create("Text",
                    typeof(string),
                    typeof(StopView),
                    null);

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create("Command",
                    typeof(ICommand),
                    typeof(StopView),
                    null);

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }


        public static readonly BindableProperty DetailTextProperty =
            BindableProperty.Create("DetailText",
                    typeof(string),
                    typeof(StopView),
                    null);

        public string DetailText
        {
            get
            {
                return (string)GetValue(DetailTextProperty);
            }
            set
            {
                SetValue(DetailTextProperty, value);
            }
        }
    }
}
