using System.Collections.Generic;

namespace Tabi
{
    public interface IMotiveConfiguration
    {
        /// <summary>
        /// Determines if motives are shown for stops
        /// </summary>
        /// <value><c>true</c> if stops should have motives; otherwise, <c>false</c>.</value>
        bool Stops { get; set; }

        /// <summary>
        /// Determines if motives are shown for tracks
        /// </summary>
        /// <value><c>true</c> if tracks should have motives; otherwise, <c>false</c>.</value>
        bool Tracks { get; set; }

        int ShowAmount { get; set; }
        List<MotiveOption> Options { get; set; }
        List<MotiveOption> OtherOptions { get; set; }
    }
}