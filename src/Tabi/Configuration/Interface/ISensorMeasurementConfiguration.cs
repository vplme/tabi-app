using System;
namespace Tabi.Configuration
{
    public interface ISensorMeasurementsConfiguration
    {
        bool Enabled { get; set; }
        bool UserAdjustable { get; set; }
    }
}
