﻿using System;
using Plugin.Battery.Abstractions;
using Tabi.DataObjects;
using Tabi.DataStorage;

namespace Tabi.Helpers
{
    public class BatteryHelper
    {
        public static DateTimeOffset LastEntryTimestamp;
        private readonly IRepoManager _repoManager;

        public BatteryHelper(IRepoManager repoManager)
        {
            _repoManager = repoManager ?? throw new ArgumentNullException(nameof(repoManager));
        }

        public void CheckStoreBatteryLevel(TimeSpan span)
        {
            if (DateTimeOffset.Now - LastEntryTimestamp >= span)
            {
                IBattery batteryPlugin = Plugin.Battery.CrossBattery.Current;
                LastEntryTimestamp = DateTimeOffset.Now;
                BatteryEntry entry = new BatteryEntry()
                {
                    Timestamp = DateTimeOffset.Now,
                    BatteryLevel = batteryPlugin.RemainingChargePercent,
                    State = ToBatteryEntryState(batteryPlugin.Status),
                };

                _repoManager.BatteryEntryRepository.Add(entry);
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
