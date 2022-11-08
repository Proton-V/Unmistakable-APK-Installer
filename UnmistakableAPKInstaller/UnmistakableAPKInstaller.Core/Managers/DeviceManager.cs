using UnmistakableAPKInstaller.Tools.Android.Models;

namespace UnmistakableAPKInstaller.Core.Managers
{
    /// <summary>
    /// Class to store CurrentDevice data
    /// </summary>
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

                string? serialNumber = null;
                if (CurrentDevice is DeviceData deviceData)
                {
                    serialNumber = deviceData?.WifiDeviceData?.SerialNumber;
                }

                return serialNumber ?? CurrentDevice.SerialNumber;
            }
        }

        public void UpdateCurrentDevice(DeviceData deviceData)
        {
            CurrentDevice = deviceData;
        }
    }
}
