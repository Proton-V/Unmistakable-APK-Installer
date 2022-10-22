namespace UnmistakableAPKInstaller.Helpers.Models.DiskCache
{
    public class DeviceCacheData
    {
        public string CustomName 
        { 
            get => customName ?? SerialNumber;
            set
            {
                if(value == null)
                {
                    return;
                }

                customName = value;
            }
        }
        private string? customName = null;

        public string SerialNumber { get; set; }
        public string IPAddressWPort { get; set; }

        public void UpdateData(DeviceCacheData deviceCacheData)
        {
            if (!string.IsNullOrEmpty(deviceCacheData.customName)
                && !deviceCacheData.customName.Equals(customName))
            {
                customName = deviceCacheData.customName;
            }

            if (!string.IsNullOrEmpty(deviceCacheData.SerialNumber)
                && !deviceCacheData.SerialNumber.Equals(SerialNumber))
            {
                SerialNumber = deviceCacheData.SerialNumber;
            }

            if (!string.IsNullOrEmpty(deviceCacheData.IPAddressWPort)
                && !deviceCacheData.IPAddressWPort.Equals(IPAddressWPort))
            {
                IPAddressWPort = deviceCacheData.IPAddressWPort;
            }
        }
    }
}
