using System;
using System.Collections.Generic;
using System.Text;

namespace Tabi.Shared.Sensors
{
    public interface ISensorManager
    {
        bool IsListening { get; }
        void StartSensorUpdates();
        void StopSensorUpdates();
    }
}
