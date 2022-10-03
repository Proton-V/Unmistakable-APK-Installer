using UnmistakableAPKInstaller.Tools.Android.Models;

namespace UnmistakableAPKInstaller.Core.Managers
{
    public class DeviceManager
    {
        public DeviceData CurrentDevice { get; private set; }
        public string CurrentDeviceSerialNumber
        {
            get
            {
                if (CurrentDevice == null)
                {
                    return null;
                }

                return CurrentDevice.WifiDeviceData?.SerialNumber ?? CurrentDevice.SerialNumber;
            }
        }

        public void UpdateCurrentDevice(DeviceData deviceData)
        {
            CurrentDevice = deviceData;
        }
    }
}
