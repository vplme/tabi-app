using System;
using Android.Hardware;

namespace Tabi.Droid.CollectionService
{
    public class SignificantMotionTriggerListener : TriggerEventListener
    {
        readonly SensorManager sensorManager;
        readonly Sensor sensor;

        public SignificantMotionTriggerListener()
        {
        }

        public SignificantMotionTriggerListener(SensorManager sensorManager, Sensor sensor)
        {
            this.sensorManager = sensorManager;
            this.sensor = sensor;
        }

        public override void OnTrigger(TriggerEvent e)
        {
            System.Diagnostics.Debug.WriteLine("SignificantMotion!");

            // Trigger only works once. Request trigger sensor again.
            sensorManager.RequestTriggerSensor(new SignificantMotionTriggerListener(sensorManager, sensor), sensor);
        }
    }
}
