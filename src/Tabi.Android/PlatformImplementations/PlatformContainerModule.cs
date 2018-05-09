using Autofac;
using Tabi.Shared.Sensors;

namespace Tabi.Droid.PlatformImplementations
{
    public class PlatformContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<LocationManagerImplementation>().As<ILocationManager>().SingleInstance();
            builder.RegisterType<SensorManagerImplementation>().As<ISensorManager>().SingleInstance();
        }
    }
}
