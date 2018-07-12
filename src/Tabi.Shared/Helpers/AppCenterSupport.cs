using System;
namespace Tabi.Shared.Helpers
{
    public class AppCenterSupport
    {
        private MobileCenterConfiguration _configuration;

        public AppCenterSupport(MobileCenterConfiguration mobileCenterConfiguration)
        {
            _configuration = mobileCenterConfiguration ?? throw new ArgumentNullException(nameof(mobileCenterConfiguration));
        }


    }
}
