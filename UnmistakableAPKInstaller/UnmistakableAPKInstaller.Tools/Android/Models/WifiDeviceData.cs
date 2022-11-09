namespace UnmistakableAPKInstaller.Tools.Android.Models
{
    /// <summary>
    /// Default Wi-Fi DeviceData.
    /// </summary>
    public class WifiDeviceData : BaseDeviceData
    {
        private WifiDeviceData() : base() { }

        public WifiDeviceData(string input) : base(input) { }

        public WifiDeviceData(BaseDeviceData baseDeviceData)
        {
            this.SerialNumber = baseDeviceData.SerialNumber;
            this.Status = baseDeviceData.Status;
            this.Info = baseDeviceData.Info;
            this.CustomCachedData = baseDeviceData.CustomCachedData;
        }
    }
}
