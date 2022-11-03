namespace UnmistakableAPKInstaller.Tools.Android.Models
{
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
