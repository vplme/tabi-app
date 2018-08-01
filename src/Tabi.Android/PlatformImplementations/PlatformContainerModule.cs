using Autofac;
using SQLite;
using Tabi.DataSync;
using Tabi.Droid.Helpers;
using Tabi.Droid.Localization;
using Tabi.Localization;
using Tabi.Sensors;

namespace Tabi.Droid.PlatformImplementations
{
    public class PlatformContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterInstance(SQLiteCreatorHelper.Connection).As<SQLiteConnection>();
            builder.RegisterType<Localize>().As<ILocalize>().SingleInstance();
            builder.RegisterType<LocationManagerImplementation>().As<ILocationManager>().SingleInstance();
            builder.RegisterType<SensorManagerImplementation>().As<ISensorManager>().SingleInstance();
            builder.RegisterType<DataUploadTask>().As<IDataUploadTask>();
        }
    }
}
