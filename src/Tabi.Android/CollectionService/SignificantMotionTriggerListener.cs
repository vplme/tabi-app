using System;
using Android.Hardware;
using Tabi.Logging;

namespace Tabi.Droid.CollectionService
{
    public class SignificantMotionTriggerListener : TriggerEventListener
    {
        readonly SensorManager sensorManager;
        readonly Sensor sensor;
        private readonly Action action;

        public SignificantMotionTriggerListener()
        {
        }

        public SignificantMotionTriggerListener(SensorManager sensorManager, Sensor sensor, Action action)
        {
            this.sensorManager = sensorManager;
            this.sensor = sensor;
            this.action = action;
        }

        public override void OnTrigger(TriggerEvent e)
        {
            Log.Info("Significant Motion detected");
            try{
                action?.Invoke();
            }
            catch (Exception exc)
            {
                Log.Error($"{exc.Message}");
            }

            // Trigger only works once. Request trigger sensor again.
            sensorManager.RequestTriggerSensor(new SignificantMotionTriggerListener(sensorManager, sensor, action), sensor);
        }
    }
}
