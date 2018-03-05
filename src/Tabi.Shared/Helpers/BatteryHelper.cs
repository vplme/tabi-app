using System;
using Plugin.Battery.Abstractions;
using Tabi.DataObjects;

namespace Tabi.Shared.Helpers
{
    public class BatteryHelper
    {
        public static DateTimeOffset LastEntryTimestamp;

        public static void CheckStoreBatteryLevel(TimeSpan span)
        {
            if(DateTimeOffset.Now - LastEntryTimestamp >= span)
            {
                IBattery batteryPlugin = Plugin.Battery.CrossBattery.Current;
                LastEntryTimestamp = DateTimeOffset.Now;
                BatteryEntry entry = new BatteryEntry()
                {
                    Timestamp = DateTimeOffset.Now,
                    BatteryLevel = batteryPlugin.RemainingChargePercent,
                    State = ToBatteryEntryState(batteryPlugin.Status),
                };

                App.RepoManager.BatteryEntryRepository.Add(entry);
            }
        }

        private static BatteryEntryState ToBatteryEntryState(Plugin.Battery.Abstractions.BatteryStatus bs)
        {
            BatteryEntryState state = BatteryEntryState.Unknown;
            switch (bs)
            {
                case Plugin.Battery.Abstractions.BatteryStatus.Full:
                    state = BatteryEntryState.Full;
                        break;
                case Plugin.Battery.Abstractions.BatteryStatus.Charging:
                    state = BatteryEntryState.Charging;
                    break;
                case Plugin.Battery.Abstractions.BatteryStatus.Discharging:
                    state = BatteryEntryState.Discharging;
                    break;
                case Plugin.Battery.Abstractions.BatteryStatus.NotCharging:
                    state = BatteryEntryState.NotCharging;
                    break;
                case Plugin.Battery.Abstractions.BatteryStatus.Unknown:
                    state = BatteryEntryState.Unknown;
                    break;   
            }
            return state;
        }
    }
}
