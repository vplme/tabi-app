using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Tabi.Droid.CollectionService
{
    public class SensorFusionServiceBinder : Binder
    {
        public SensorFusionService Service { get; }

        public SensorFusionServiceBinder(SensorFusionService service)
        {
            Service = service;
        }

    }
}