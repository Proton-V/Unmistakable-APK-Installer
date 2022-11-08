namespace UnmistakableAPKInstaller.Helpers.Models.DiskCache
{
    /// <summary>
    /// Updatable device cache data class
    /// </summary>
    public class DeviceCacheData
    {
        /// <summary>
        /// Custom device name.
        /// Default: <see cref="SerialNumber"/>
        /// </summary>
        public string CustomName 
        { 
            get => _customName ?? SerialNumber;
            set
            {
                if(value == null)
                {
                    return;
                }

                _customName = value;
            }
        }
        private string? _customName = null;

        /// <summary>
        /// Device serial number
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Device IP address with port.
        /// </summary>
        public string IPAddressWPort { get; set; }

        /// <summary>
        /// Safe update data,
        /// exclude null && equals values
        /// </summary>
        /// <param name="deviceCacheData"></param>
        public void UpdateData(DeviceCacheData deviceCacheData)
        {
            if (!string.IsNullOrEmpty(deviceCacheData._customName)
                && !deviceCacheData._customName.Equals(_customName))
            {
                _customName = deviceCacheData._customName;
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
