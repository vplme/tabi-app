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

        public static BindableProperty TitleProperty = BindableProperty.Create(
                                                propertyName: "Title",
                                                returnType: typeof(string),
                                                declaringType: typeof(StopView),
                                                defaultValue: "",
                                                defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: titlePropertyChanged);

        public string Title
        {
            get { return base.GetValue(TitleProperty).ToString(); }
            set { base.SetValue(TitleProperty, value); }
        }

        private static void titlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (StopView)bindable;
            control.textLabel.Text = newValue?.ToString();
        }

        public static BindableProperty DetailProperty = BindableProperty.Create(
                                           propertyName: "Detail",
                                           returnType: typeof(string),
                                           declaringType: typeof(StopView),
                                           defaultValue: "",
                                           defaultBindingMode: BindingMode.TwoWay,
                                        propertyChanged: detailPropertyChanged);

        private static void detailPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (StopView)bindable;
            control.detailLabel.Text = newValue?.ToString();
        }

        public string Detail
        {
            get { return base.GetValue(DetailProperty).ToString(); }
            set { base.SetValue(DetailProperty, value); }
        }

        public static BindableProperty CommandProperty = BindableProperty.Create(
                                           propertyName: "Command",
                                           returnType: typeof(ICommand),
                                           declaringType: typeof(StopView),
                                           defaultValue: null,
                                           defaultBindingMode: BindingMode.TwoWay
                                        );

        private static void commandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (StopView)bindable;
            control.stackLayout.GestureRecognizers.Clear();
            control.stackLayout.GestureRecognizers.Add(new TapGestureRecognizer());
        }

        public ICommand Command
        {
            get { return base.GetValue(CommandProperty) as ICommand; }
            set { base.SetValue(CommandProperty, value); }
        }

       
    }
}
