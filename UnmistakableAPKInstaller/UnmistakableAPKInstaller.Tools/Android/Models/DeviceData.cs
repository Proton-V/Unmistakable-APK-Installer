using System.Net;
using UnmistakableAPKInstaller.Helpers;

namespace UnmistakableAPKInstaller.Tools.Android.Models
{
    public class DeviceData
    {
        public const string NULL_VALUE = "Null";

        private DeviceData() 
        {
            info = new Dictionary<string, string>();
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
                return new DeviceData()
                {
                    SerialNumber = NULL_VALUE,
                    Status = NULL_VALUE
                };
            }
        }

        public bool IsActive => Status.Contains("device");
        public bool IsActiveWifi => WifiDeviceData?.IsActive ?? false;
        public bool IsWifiDevice => IPAddress.TryParse(SerialNumber.Split(':')[0], out IPAddress iPAddress);

        public DeviceData WifiDeviceData { get; private set; }

        public string SerialNumber { get; private set; }
        public string Status { get; private set; }

        public string Model => info.GetValueOrDefault("model");

        public Dictionary<string, string> info { get; private set; }

        public void SetWifiDeviceData(DeviceData wifiDeviceData)
        {
            WifiDeviceData = wifiDeviceData;
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
                        info.Add(infoKeyValue[0], infoKeyValue[1]);
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
