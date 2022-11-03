using UnmistakableAPKInstaller.Helpers.Models.DiskCache;

namespace UnmistakableAPKInstaller.Tools.Android.Models
{
    public class DeviceData : BaseDeviceData
    {
        public const string NULL_VALUE = "Null";

        private DeviceData() : base() { }

        public DeviceData(string input, WifiDeviceData wifiDeviceData = default) : base(input)
        {
            WifiDeviceData = wifiDeviceData;
        }

        public DeviceData(BaseDeviceData baseDeviceData) 
        {
            this.SerialNumber = baseDeviceData.SerialNumber;
            this.Status = baseDeviceData.Status;
            this.Info = baseDeviceData.Info;
            this.CustomCachedData = baseDeviceData.CustomCachedData;
        }

        public static DeviceData Default
        {
            get
            {
                var newData = new DeviceData()
                {
                    SerialNumber = NULL_VALUE,
                    Status = NULL_VALUE
                };

                newData.SetCachedData(new DeviceCacheData()
                {
                    CustomName = NULL_VALUE,
                    SerialNumber = NULL_VALUE,
                    IPAddressWPort = NULL_VALUE,
                });
                return newData;
            }
        }

        public bool IsActiveWifi => (IsWifiDevice && IsActive) || (WifiDeviceData?.IsActive ?? false);

        public WifiDeviceData WifiDeviceData { get; protected set; }

        public void SetWifiDeviceData(WifiDeviceData wifiDeviceData)
        {
            WifiDeviceData = wifiDeviceData;
        }
    }
}
