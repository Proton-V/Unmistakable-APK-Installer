using System.Text.Json;
using UnmistakableAPKInstaller.Helpers.Models.DiskCache;

namespace UnmistakableAPKInstaller.Helpers
{
    /// <summary>
    /// DiskCache controller for <see cref="DeviceCacheData"/>
    /// </summary>
    public class DiskCache
    {
        public static readonly string AppDataDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(UnmistakableAPKInstaller));

        private static Dictionary<string, DeviceCacheData> deviceDict = new Dictionary<string, DeviceCacheData>();

        /// <summary>
        /// Disk cache file path
        /// </summary>
        private static string path;

        public static void Init(string diskCacheFilePath)
        {
            path = diskCacheFilePath;
            LoadFromDisk();
        }

        /// <summary>
        /// Upsert method
        /// </summary>
        /// <param name="deviceCacheData"></param>
        /// <returns></returns>
        public static DeviceCacheData AddOrUpdateValue(DeviceCacheData deviceCacheData)
        {
            var deviceData = FindDataByIP(deviceCacheData.IPAddressWPort);

            if (deviceData is null)
            {
                deviceData = FindDataBySerialNumber(deviceCacheData.SerialNumber);
            }

            if (deviceData is null)
            {
                if(deviceCacheData.SerialNumber != null)
                {
                    deviceData = deviceCacheData;
                    deviceDict.Add(deviceData.SerialNumber, deviceData);
                }
            }
            else
            {
                deviceData.UpdateData(deviceCacheData);
                deviceDict[deviceData.SerialNumber] = deviceData;
            }

            return deviceData;
        }

        /// <summary>
        /// Find data with <paramref name="ipAddress"/>
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private static DeviceCacheData FindDataByIP(string ipAddress)
        {
            return deviceDict.FirstOrDefault(x => x.Value.IPAddressWPort == ipAddress).Value;
        }

        /// <summary>
        /// Find data with <paramref name="serialNumber"/>
        /// </summary>
        /// <param name="serialNumber"></param>
        /// <returns></returns>
        private static DeviceCacheData FindDataBySerialNumber(string serialNumber)
        {
            return deviceDict.FirstOrDefault(x => x.Value.SerialNumber == serialNumber).Value;
        }

        /// <summary>
        /// Load disk cache from saved file
        /// </summary>
        public static void LoadFromDisk()
        {
            if (!File.Exists(path))
            {
                return;
            }

            var json = File.ReadAllText(path);
            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            var datas = JsonSerializer.Deserialize<DeviceCacheData[]>(json)?
                // To reduce errors
                .DistinctBy(x => x.SerialNumber)
                .ToArray();
            foreach (var data in datas)
            {
                deviceDict.Add(data.SerialNumber, data);
            }
        }

        /// <summary>
        /// Save current cache to disk
        /// </summary>
        public static void SaveToDisk()
        {
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            var datas = deviceDict.Values.Cast<DeviceCacheData>()
                .DistinctBy(x => x.SerialNumber)
                .ToArray();
            var text = JsonSerializer.Serialize(datas, options);
            File.WriteAllText(path, text);
        }
    }
}
