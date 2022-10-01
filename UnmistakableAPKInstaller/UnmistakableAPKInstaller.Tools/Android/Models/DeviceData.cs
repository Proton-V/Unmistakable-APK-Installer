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

        public DeviceData(string input) : this()
        {
            Update(input);
        }

        public static DeviceData Default
        {
            get
            {
                return new DeviceData()
                {
                    serialNumber = NULL_VALUE,
                    status = NULL_VALUE
                };
            }
        }

        public bool IsActive => status.Contains("device");

        public string serialNumber;
        public string status;
        public Dictionary<string, string> info;

        private void Update(string input)
        {
            try
            {
                var inputArray = input.Split(null);
                serialNumber = inputArray[0];
                status = inputArray[1];

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
