using System;
using Tabi.Shared.Helpers;
using UIKit;

namespace Tabi.iOS.PlatformImplementations
{
    public class iOSHelper : IIOSHelper
    {
        public bool IsiPhoneX => UIScreen.MainScreen.NativeBounds.Height == 2436;
    }
}
