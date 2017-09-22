﻿using System;
using Android.Hardware;

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
            action?.Invoke();
            
            // Trigger only works once. Request trigger sensor again.
            sensorManager.RequestTriggerSensor(new SignificantMotionTriggerListener(sensorManager, sensor, action), sensor);
        }
    }
}
