using System;
using Autofac;
using CoreLocation;
using CoreMotion;
using Tabi.DataObjects.CollectionProfile;
using Tabi.DataSync;
using Tabi.Helpers;
using Tabi.iOS.Localization;
using Tabi.Localization;
using Tabi.Sensors;

namespace Tabi.iOS.PlatformImplementations
{
    public class PlatformContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<CLLocationManager>();
            builder.RegisterType<CMMotionActivityManager>();
            builder.RegisterType<Localize>().As<ILocalize>().SingleInstance();
            builder.RegisterType<LocationManagerImplementation>().As<ILocationManager>().SingleInstance();
            builder.RegisterType<SensorManagerImplementation>().As<ISensorManager>().SingleInstance();
            builder.RegisterType<ExtraPermission>().As<IExtraPermission>().SingleInstance();
            builder.RegisterType<CMMotionManager>();
            builder.RegisterType<CMPedometer>();
            builder.RegisterInstance(CollectionProfile.GetDefaultProfile().iOSProfile).As<ProfileiOS>();
            builder.RegisterType<DataUploadTask>().As<IDataUploadTask>();
            builder.RegisterType<iOSHelper>().As<IIOSHelper>();
        }
    }
}
