using UnmistakableAPKInstaller.Tools.Android.Models;

namespace UnmistakableAPKInstaller.Core.Managers
{
    public class DeviceManager
    {
        public DeviceData currentDevice;
        public string CurrentDeviceSerialNumber
        {
            get
            {
                if (currentDevice == null)
                {
                    return null;
                }

                return currentDevice.WifiDeviceData?.SerialNumber ?? currentDevice.SerialNumber;
            }
        }
    }
}
