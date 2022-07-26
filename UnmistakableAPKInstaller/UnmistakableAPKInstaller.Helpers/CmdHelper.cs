﻿using Serilog;
using System.Diagnostics;
using System.Text;

namespace UnmistakableAPKInstaller.Helpers
{
    /// <summary>
    /// Cmd helper class for start/close process
    /// </summary>
    public class CmdHelper
    {
        /// <summary>
        /// Start default hidden process
        /// </summary>
        /// <param name="path">program file path</param>
        /// <param name="arguments">args params single line</param>
        /// <param name="timeoutInSec">default: infinity</param>
        /// <returns></returns>
        public static async Task<(string data, string error)> StartProcessAsync(string path, string arguments, int timeoutInSec = -1)
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
     
                    CancellationToken token = default;
                    try
                    {
                        if(timeoutInSec > 0)
                        {
                            var timeoutSignal = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutInSec));
                            token = timeoutSignal.Token;
                        }
                        await p.WaitForExitAsync(token);
                    }
                    catch (OperationCanceledException e)
                    {
                        Log.Error($"Process {p.ProcessName} exit with: {e}");
                        p.Kill();
                    }

                    var outStr = outData.ToString();
                    var errorStr = outErrorData.ToString();

                    if (!string.IsNullOrEmpty(errorStr))
                    {
                        Log.Warning("CmdHelper: {0}", errorStr);
                    }

                    return (outStr, errorStr);
                }
            }
            catch(Exception e)
            {
                Log.Error("GD Download Helper: {0}", e.ToString());
                return (null, e.Message);
            }
        }

        /// <summary>
        /// Stop all processes by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Task StopAllProcessesByName(string name)
        {
            foreach (var process in Process.GetProcessesByName(name))
            {
                process.Kill();
            }

            return Task.CompletedTask;
        }
    }
}
