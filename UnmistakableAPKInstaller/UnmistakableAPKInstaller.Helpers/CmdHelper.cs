using System.Diagnostics;
using System.Text;

namespace UnmistakableAPKInstaller.Helpers
{
    public class CmdHelper
    {
        public static async Task<(string data, string error)> StartProcess(string path, string arguments)
        {
            try
            {
                var outData = new StringBuilder();
                var outErrorData = new StringBuilder();

                using (var p = new Process())
                {
                    p.StartInfo.FileName = path;
                    p.StartInfo.Arguments = arguments;

                    p.EnableRaisingEvents = true;
                    p.StartInfo.UseShellExecute = false;

                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.RedirectStandardError = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.RedirectStandardInput = false;
                    p.OutputDataReceived += (a, b) => outData.AppendLine(b.Data);
                    p.ErrorDataReceived += (a, b) => outData.AppendLine(b.Data);
                    p.Start();
                    p.BeginErrorReadLine();
                    p.BeginOutputReadLine();

                    await p.WaitForExitAsync();

                    var outStr = outData.ToString();
                    var errorStr = outErrorData.ToString();

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        CustomLogger.WriteToLog("CmdHelper: {0}", errorStr);
                    }

                    return (outStr, errorStr);
                }
            }
            catch(Exception e)
            {
                CustomLogger.WriteToLog("GD Download Helper: {0}", e.ToString());
                return (null, e.Message);
            }
        }
    }
}
