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

        private static BatteryEntryState ToBatteryEntryState(BatteryStatus bs)
        {
            BatteryEntryState state = BatteryEntryState.Unknown;
            switch (bs)
            {
                case BatteryStatus.Full:
                    state = BatteryEntryState.Full;
                        break;
                case BatteryStatus.Charging:
                    state = BatteryEntryState.Charging;
                    break;
                case BatteryStatus.Discharging:
                    state = BatteryEntryState.Discharging;
                    break;
                case BatteryStatus.NotCharging:
                    state = BatteryEntryState.NotCharging;
                    break;
                case BatteryStatus.Unknown:
                    state = BatteryEntryState.Unknown;
                    break;   
            }
            return state;
        }
    }
}
