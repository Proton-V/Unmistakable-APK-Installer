using System.Net;
using UnmistakableAPKInstaller.Helpers;
using UnmistakableAPKInstaller.Helpers.Models.DiskCache;

namespace UnmistakableAPKInstaller.Tools.Android.Models
{
    public class BaseDeviceData
    {
        protected BaseDeviceData()
        {
            Info = new Dictionary<string, string>();
            CustomCachedData = new DeviceCacheData();
        }

        public BaseDeviceData(string input) : this()
        {
            Update(input);
        }

        public bool IsActive => Status.Contains("device");
        public bool IsWifiDevice => IPAddress.TryParse(SerialNumber.Split(':')[0], out IPAddress iPAddress);
        public string Model => Info.GetValueOrDefault("model");

        public string SerialNumber { get; protected set; }
        public string Status { get; protected set; }

        public Dictionary<string, string> Info { get; protected set; }

        public DeviceCacheData CustomCachedData { get; protected set; }

        public void SetCachedData(DeviceCacheData cacheData)
        {
            CustomCachedData = cacheData;
        }

        protected void Update(string input)
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
