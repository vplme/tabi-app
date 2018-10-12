using System.Collections.Generic;

namespace Tabi
{
    public interface ITransportationModeConfiguration
    {
        bool CustomTransportModes { get; set; }
        List<TransportOption> Options { get; set; }
    }
}