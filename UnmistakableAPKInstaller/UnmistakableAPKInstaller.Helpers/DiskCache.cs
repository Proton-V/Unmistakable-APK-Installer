using System.Text.Json;
using UnmistakableAPKInstaller.Helpers.Models.DiskCache;

namespace UnmistakableAPKInstaller.Helpers
{
    public class DiskCache
    {
        public static readonly string AppDataDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), nameof(UnmistakableAPKInstaller));

        private static Dictionary<string, DeviceCacheData> deviceDict = new Dictionary<string, DeviceCacheData>();

        private static string path;

        public static void Init(string diskCacheFilePath)
        {
            path = diskCacheFilePath;
            LoadFromDisk();
        }

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

        private static DeviceCacheData FindDataByIP(string ipAddress)
        {
            return deviceDict.FirstOrDefault(x => x.Value.IPAddressWPort == ipAddress).Value;
        }

        private static DeviceCacheData FindDataBySerialNumber(string serialNumber)
        {
            return deviceDict.FirstOrDefault(x => x.Value.SerialNumber == serialNumber).Value;
        }

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
