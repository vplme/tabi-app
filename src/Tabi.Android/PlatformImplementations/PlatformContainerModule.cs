using Autofac;
using Tabi.Droid.Localization;
using Tabi.Shared;
using Tabi.Shared.Sensors;

namespace Tabi.Droid.PlatformImplementations
{
    public class PlatformContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<Localize>().As<ILocalize>().SingleInstance();
            builder.RegisterType<LocationManagerImplementation>().As<ILocationManager>().SingleInstance();
            builder.RegisterType<SensorManagerImplementation>().As<ISensorManager>().SingleInstance();
        }
    }
}
