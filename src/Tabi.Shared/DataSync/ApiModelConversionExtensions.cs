using System;
using Tabi.DataObjects;

namespace Tabi.Shared.DataSync
{
    public static class ApiModelConversionExtensions
    {
        public static TabiApiClient.Models.Stop ToApiModel(this Stop stop)
        {
            return new TabiApiClient.Models.Stop()
            {
                Longitude = stop.Longitude,
                Latitude = stop.Latitude,
                Name = stop.Name,
                Timestamp = stop.Timestamp,
                LocalId = stop.Id,
            };

        }

        public static TabiApiClient.Models.StopVisit ToApiModel(this StopVisit stopVisit)
        {
            return new TabiApiClient.Models.StopVisit()
            {
                LocalId = stopVisit.Id,
                LocalStopId = stopVisit.StopId,
                BeginTimestamp = stopVisit.BeginTimestamp,
                EndTimestamp = stopVisit.EndTimestamp
            };

        }

        public static TabiApiClient.Models.Log ToApiModel(this LogEntry log)
        {
            return new TabiApiClient.Models.Log()
            {
                Event = log.Event,
                Message = log.Message,
            };
        }


        public static TabiApiClient.Models.TrackEntry ToApiModel(this TrackEntry trackEntry)
        {
            return new TabiApiClient.Models.TrackEntry()
            {
                LocalId = trackEntry.Id,
                StartTime = trackEntry.StartTime,
                EndTime = trackEntry.EndTime
            };
        }

        public static TabiApiClient.Models.BatteryInfo ToApiModel(this BatteryEntry batteryEntry)
        {
            return new TabiApiClient.Models.BatteryInfo()
            {
                BatteryLevel = batteryEntry.BatteryLevel,
                Timestamp = batteryEntry.Timestamp,
                State = ConvertBatteryEntryStateToApiModel(batteryEntry.State)
            };
        }



        private static TabiApiClient.Models.BatteryState ConvertBatteryEntryStateToApiModel(BatteryEntryState entryState)
        {
            TabiApiClient.Models.BatteryState apiState = TabiApiClient.Models.BatteryState.Unknown;

            switch (entryState)
            {
                case BatteryEntryState.Charging:
                    apiState = TabiApiClient.Models.BatteryState.Charging;
                    break;
                case BatteryEntryState.Discharging:
                    apiState = TabiApiClient.Models.BatteryState.Discharging;
                    break;
                case BatteryEntryState.Full:
                    apiState = TabiApiClient.Models.BatteryState.Full;
                    break;
                case BatteryEntryState.NotCharging:
                    apiState = TabiApiClient.Models.BatteryState.NotCharging;
                    break;
                case BatteryEntryState.Unknown:
                    apiState = TabiApiClient.Models.BatteryState.Unknown;
                    break;
            }

            return apiState;
        }

        public static TabiApiClient.Models.UserStopMotive ToUserStopMotiveApiModel(this Motive motive)
        {
            return new TabiApiClient.Models.UserStopMotive()
            {
                LocalStopId = motive.StopId,
                Motive = motive.Text,
                Timestamp = motive.Timestamp,
            };

        }

        public static TabiApiClient.Models.TrackMotive ToTrackMotiveMotiveApiModel(this Motive motive)
        {
            return new TabiApiClient.Models.TrackMotive()
            {
                LocalTrackId = motive.TrackId,
                Motive = motive.Text,
                Timestamp = motive.Timestamp,
            };

        }

        public static TabiApiClient.Models.TransportationMode ToApiModel(this TransportationModeEntry entry)
        {
            return new TabiApiClient.Models.TransportationMode()
            {
                LocalTrackId = entry.TrackId,
                DeviceId = Settings.Current.Device,
                Timestamp = entry.Timestamp,
                Walk = entry.Walk,
                Run = entry.Run,
                MobilityScooter = entry.MobilityScooter,
                Car = entry.Car,
                Bike = entry.Bike,
                Moped = entry.Moped,
                Scooter = entry.Scooter,
                Motorcycle = entry.Motorcycle,
                Train = entry.Train,
                Subway = entry.Subway,
                Tram = entry.Tram,
                Bus = entry.Bus,
                Other = entry.Other
            };
        }

        public static TabiApiClient.Models.PositionEntry ToApiModel(this PositionEntry position)
        {
            return new TabiApiClient.Models.PositionEntry()
            {
                Latitude = position.Latitude,
                Longitude = position.Longitude,
                Accuracy = position.Accuracy,
                DesiredAccuracy = position.DesiredAccuracy,
                Altitude = position.Altitude,
                Speed = position.Speed,
                DistanceBetweenPreviousPosition = position.DistanceBetweenPreviousPosition,
                Timestamp = position.Timestamp
            };
        }

        public static TabiApiClient.Models.MotionSensor ToApiModel(this Accelerometer accelerometer)
        {
            return new TabiApiClient.Models.MotionSensor()
            {
                X = accelerometer.X,
                Y = accelerometer.Y,
                Z = accelerometer.Z,
                Timestamp = accelerometer.Timestamp
            };
        }

        public static TabiApiClient.Models.MotionSensor ToApiModel(this Gyroscope gyroscope)
        {
            return new TabiApiClient.Models.MotionSensor()
            {
                X = gyroscope.X,
                Y = gyroscope.Y,
                Z = gyroscope.Z,
                Timestamp = gyroscope.Timestamp
            };
        }

        public static TabiApiClient.Models.MotionSensor ToApiModel(this Magnetometer magnetometer)
        {
            return new TabiApiClient.Models.MotionSensor()
            {
                X = magnetometer.X,
                Y = magnetometer.Y,
                Z = magnetometer.Z,
                Timestamp = magnetometer.Timestamp
            };
        }

        public static TabiApiClient.Models.MotionSensor ToApiModel(this LinearAcceleration linearAcceleration)
        {
            return new TabiApiClient.Models.MotionSensor()
            {
                X = linearAcceleration.X,
                Y = linearAcceleration.Y,
                Z = linearAcceleration.Z,
                Timestamp = linearAcceleration.Timestamp
            };
        }

        public static TabiApiClient.Models.MotionSensor ToApiModel(this Orientation orientation)
        {
            return new TabiApiClient.Models.MotionSensor()
            {
                X = orientation.X,
                Y = orientation.Y,
                Z = orientation.Z,
                Timestamp = orientation.Timestamp
            };
        }

        public static TabiApiClient.Models.MotionSensor ToApiModel(this Gravity gravity)
        {
            return new TabiApiClient.Models.MotionSensor()
            {
                X = gravity.X,
                Y = gravity.Y,
                Z = gravity.Z,
                Timestamp = gravity.Timestamp
            };
        }

        public static TabiApiClient.Models.Quaternion ToApiModel(this Quaternion quaternion)
        {
            return new TabiApiClient.Models.Quaternion()
            {
                X = quaternion.X,
                Y = quaternion.Y,
                Z = quaternion.Z,
                W = quaternion.W,
                Timestamp = quaternion.Timestamp
            };
        }

        public static TabiApiClient.Models.SensorMeasurementSession ToApiModel(this SensorMeasurementSession session)
        {
            return new TabiApiClient.Models.SensorMeasurementSession()
            {
                AmbientLight = session.AmbientLight,
                BatteryLevel = session.BatteryLevel,
                BatteryState = (int)session.BatteryStatus,
                Pedometer = session.Pedometer,
                Proximity = session.Proximity,
                Compass = session.Compass,
                Timestamp = session.Timestamp
            };
        }
    }
}
