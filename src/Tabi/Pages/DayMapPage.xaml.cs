﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Tabi.Controls;
using Tabi.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Tabi.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DayMapPage : ContentPage
    {
        DayMapViewModel ViewModel => vm ?? (vm = BindingContext as DayMapViewModel);
        DayMapViewModel vm;


        public DayMapPage()
        {
            InitializeComponent();
            BindingContext = App.Container.Resolve<DayMapViewModel>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ShowPositions();
            ShowMap();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            routeMap.ClearMap();
        }

        async Task ShowPositions()
        {
            List<Line> lines = await ViewModel.GetLinesAsync();
            routeMap.Lines.Clear();
            routeMap.Lines = lines;

            MapSpan mapSpan = await ViewModel.GetMapSpanAsync();
            routeMap.MoveToRegion(mapSpan);

            routeMap.ClearMap();
            routeMap.DrawRoute();
        }

        void DaySwitcher(object sender, EventArgs e)
        {

        }

        async void ShowMap()
        {
            routeMap.Pins.Clear();
            List<Pin> pins = await Task.Run(() => ViewModel.GetPins());

            pins.ForEach(p => routeMap.Pins.Add(p));
        }
    }
}
