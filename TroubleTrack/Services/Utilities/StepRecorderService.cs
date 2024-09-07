using CliWrap;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TroubleTrack.Services.Utilities
{
    public class StepRecorderService
    {
        private readonly ILogger<StepRecorderService> _logger;
        private const string _psrExe = "psr.exe";
        private const string _outputFilePath = @"c:\temp\";

        public StepRecorderService(ILogger<StepRecorderService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void StartRecording(bool captureScreenshots = true)
        {
            InvokeProcess(_psrExe, $"/start /output \"{_outputFilePath}{DateTime.Now.Ticks}.zip\" /gui 0 /sc 1 /sketch 1 /maxsc 100");
        }

        public void StopRecording()
        {
            InvokeProcess(_psrExe, @"/stop").WaitForExit(60000);
        }

        #region Helpers

        private Process InvokeProcess(string processName, string parameters)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = _psrExe,
                    Arguments = parameters,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true
                };

                _logger.LogInformation($"Successfully executed {_psrExe} with arguments: {parameters}");

                return Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing {_psrExe}.");
                throw;
            }
        }

        private void TryKillProcess(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process proc in processes)
            {
                try { proc.Kill(); }
                catch { throw; }
            }
        }

        #endregion Helpers
    }
}
