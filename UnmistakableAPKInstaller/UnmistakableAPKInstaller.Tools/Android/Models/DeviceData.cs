using System.Net;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Helpers.Models.DiskCache;

namespace UnmistakableAPKInstaller.Tools.Android.Models
{
    public class DeviceData
    {
        public const string NULL_VALUE = "Null";

        private DeviceData() 
        {
            Info = new Dictionary<string, string>();
            CachedData = new DeviceCacheData();
        }

        public DeviceData(string input, DeviceData wifiDeviceData = default) : this()
        {
            Update(input);
            WifiDeviceData = wifiDeviceData;
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

        public bool IsActive => Status.Contains("device");
        public bool IsActiveWifi => (IsWifiDevice && IsActive) || (WifiDeviceData?.IsActive ?? false);
        public bool IsWifiDevice => IPAddress.TryParse(SerialNumber.Split(':')[0], out IPAddress iPAddress);

        public DeviceData WifiDeviceData { get; private set; }

        public string SerialNumber { get; private set; }
        public string Status { get; private set; }

        public string Model => Info.GetValueOrDefault("model");

        public Dictionary<string, string> Info { get; private set; }

        public DeviceCacheData CachedData { get; private set; }

        public void SetWifiDeviceData(DeviceData wifiDeviceData)
        {
            WifiDeviceData = wifiDeviceData;
        }

        public void SetCachedData(DeviceCacheData cacheData)
        {
            CachedData = cacheData;
        }

        private void Update(string input)
        {
            try
            {
                var inputArray = input
                    .Split(null)
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray();
                SerialNumber = inputArray[0];
                Status = inputArray[1];

                for (int i = 2; i < inputArray.Length; i++)
                {
                    var infoKeyValue = inputArray[i].Split(':');
                    if (infoKeyValue.Length == 2)
                    {
                        Info.Add(infoKeyValue[0], infoKeyValue[1]);
                    }
                    else
                    {
                        CustomLogger.Log($"Error reading part of android device data ({inputArray[i]})");
                    }
                }
            }
            catch (Exception e)
            {
                CustomLogger.Log($"{e}");
            }
        }
    }
}
