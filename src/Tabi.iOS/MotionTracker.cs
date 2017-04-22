using System;
using System.Diagnostics;
using System.Threading.Tasks;
using CoreMotion;
using Foundation;
using Tabi.iOS.Helpers;
using Tabi.Model;

namespace Tabi.iOS
{
    public class MotionTracker
    {
        CMMotionManager cmManager;
        int deviceMotionInterval = 60;
        int deviceMotionPeriod = 1500;
        DateTime nextDeviceMotionPoll;

        public MotionTracker()
        {
            cmManager = new CMMotionManager();
            cmManager.DeviceMotionUpdateInterval = 1;
        }

        private void PollDeviceMotion()
        {

            if (DateTime.Now > nextDeviceMotionPoll)
            {
                SaveDeviceMotion();
                Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromMinutes(2));

                    cmManager.StopDeviceMotionUpdates();
                });

                nextDeviceMotionPoll = DateTime.Now.AddSeconds(deviceMotionInterval);
            }

        }


        private void SaveDeviceMotion()
        {
            NSOperationQueue queue = new NSOperationQueue();
            cmManager.StartDeviceMotionUpdates(queue, async (CMDeviceMotion motion, NSError error) =>
        {
            //Repository<AccelerometerData> accRepo = new Repository<AccelerometerData>(SQLiteHelper.Instance.Connection);

            Debug.WriteLine("DeviceMotionUpdate" + motion.Timestamp);

            // motion.Timestamp, set to accelerometerdata. Time since boot. Convert to time? How?

            DateTimeOffset timestamp = Utils.TimeSinceBootToDateTimeOffset(motion.Timestamp);
            Debug.WriteLine("Compare timestamps vs now: {0} vs {1}", DateTimeOffset.Now, timestamp);

            AccelerometerData userAcc = new AccelerometerData()
            {
                Type = AccelerometerType.User,
                Timestamp = timestamp,
                X = motion.UserAcceleration.X,
                Y = motion.UserAcceleration.Y,
                Z = motion.UserAcceleration.Z
            };

            AccelerometerData gravAcc = new AccelerometerData()
            {
                Type = AccelerometerType.Gravity,
                Timestamp = timestamp,
                X = motion.Gravity.X,
                Y = motion.Gravity.Y,
                Z = motion.Gravity.Z
            };
            Debug.WriteLine("Useracc " + userAcc);
            Debug.WriteLine("gravacc " + gravAcc);

            //await accRepo.Insert(userAcc);
            //await accRepo.Insert(gravAcc);
        });
        }
    }
}
