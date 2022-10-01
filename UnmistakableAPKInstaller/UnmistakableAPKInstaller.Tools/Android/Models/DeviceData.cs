using UnmistakableAPKInstaller.Helpers;

namespace UnmistakableAPKInstaller.Tools.Android.Models
{
    public class DeviceData
    {
        public DeviceData(string input)
        {
            info = new Dictionary<string, string>();
            Update(input);
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
