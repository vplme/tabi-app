using Tabi.Configuration;

namespace Tabi
{
    public interface ITabiConfiguration
    {
        IAppConfiguration App { get; set; }
        IMobileCenterConfiguration MobileCenter { get; set; }
        INotificationsConfiguration Notifications { get; set; }
        ISupportConfiguration Support { get; set; }
        IStopResolverConfiguration StopResolver { get; set; }
        ISensorMeasurementsConfiguration SensorMeasurements { get; set; }
        IUserInterfaceConfiguration UserInterface { get; set; }
        ILoggingConfiguration Logging { get; set; }
        IApiConfiguration Api { get; set; }
        IMotiveConfiguration Motive { get; set; }
        ITransportationModeConfiguration TransportMode { get; set; }
    }
}