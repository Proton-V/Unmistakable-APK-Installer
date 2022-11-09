using Serilog;
using System.Net;
using UnmistakableAPKInstaller.Helpers.Models.DiskCache;

namespace UnmistakableAPKInstaller.Tools.Android.Models
{
    /// <summary>
    /// Base class for all devices
    /// </summary>
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

        /// <summary>
        /// Device is active
        /// </summary>
        public bool IsActive => Status.Contains("device");

        /// <summary>
        /// Device has IP address in <see cref="SerialNumber"/>
        /// </summary>
        public bool IsWifiDevice => IPAddress.TryParse(SerialNumber.Split(':')[0], out IPAddress iPAddress);

        /// <summary>
        /// Model string
        /// </summary>
        public string Model => Info.GetValueOrDefault("model");

        /// <summary>
        /// Serial number
        /// </summary>
        public string SerialNumber { get; protected set; }

        /// <summary>
        /// Status
        /// </summary>
        public string Status { get; protected set; }

        /// <summary>
        /// Extra device info
        /// </summary>
        public Dictionary<string, string> Info { get; protected set; }

        /// <summary>
        /// Cached device data.
        /// </summary>
        public DeviceCacheData CustomCachedData { get; protected set; }

        public void SetCachedData(DeviceCacheData cacheData)
        {
            CustomCachedData = cacheData;
        }

        /// <summary>
        /// Update device data with input str
        /// </summary>
        /// <param name="input"></param>
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
                        Log.Warning($"Error reading part of android device data ({inputArray[i]})");
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e}");
            }
        }
    }
}
