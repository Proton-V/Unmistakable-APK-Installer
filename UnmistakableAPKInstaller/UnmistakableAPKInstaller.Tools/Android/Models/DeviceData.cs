using UnmistakableAPKInstaller.Helpers.Models.DiskCache;

namespace UnmistakableAPKInstaller.Tools.Android.Models
{
    /// <summary>
    /// Default DeviceData.
    /// E.g. USB device
    /// </summary>
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

        /// <summary>
        /// Get Null/Default instance of <see cref="DeviceData"/>
        /// </summary>
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

        /// <summary>
        /// Check if Wi-Fi mode is active
        /// </summary>
        public bool IsActiveWifi => (IsWifiDevice && IsActive) || (WifiDeviceData?.IsActive ?? false);

        /// <summary>
        /// Wi-Fi device data related to this data
        /// </summary>
        public WifiDeviceData WifiDeviceData { get; protected set; }

        /// <summary>
        /// Set new Wi-Fi data related to this device
        /// </summary>
        /// <param name="wifiDeviceData"></param>
        public void SetWifiDeviceData(WifiDeviceData wifiDeviceData)
        {
            WifiDeviceData = wifiDeviceData;
        }
    }
}
