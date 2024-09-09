using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace TroubleTrack.Services
{
    public class StepRecorderService
    {
        #region Fields
        private const string PsrExe = "psr.exe";
        private const int SaveDelay = 5000;
        private string _outputFilePath;
        private readonly ILogger<StepRecorderService> _logger;
        private readonly AppStateService _appStateService;

        #endregion

        #region Constructor

        public StepRecorderService(ILogger<StepRecorderService> logger, AppStateService appStateService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appStateService = appStateService ?? throw new ArgumentNullException(nameof(appStateService));
            _outputFilePath = Path.GetTempPath();
        }

        #endregion

        #region Public Methods

        public async Task StartRecordingAsync()
        {
            _outputFilePath = Path.Combine(Path.GetTempPath(), $"{DateTime.Now.Ticks}.zip");
            string parameters = $"/start /output \"{_outputFilePath}\" /gui 0 /sc 1 /sketch 1 /maxsc 100";

            _logger.LogInformation("Starting PSR recording.");
            TryKillProcess(PsrExe);
            var process = InvokeProcess(PsrExe, parameters);
            await Task.Delay(100);
            if (process == null)
            {
                _logger.LogError("Failed to start PSR recording.");
                throw new InvalidOperationException("Failed to start PSR recording.");
            }
        }

        public async Task StopRecordingAsync()
        {
            try
            {
                var process = InvokeProcess(PsrExe, @"/stop");

                if (process != null)
                {
                    await process.WaitForExitAsync().ConfigureAwait(false);
                }

                await Task.Delay(SaveDelay).ConfigureAwait(false);

                if (File.Exists(_outputFilePath))
                {
                    _appStateService.SaveValue("Recording", _outputFilePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unexpected exception while trying to stop PSR process: {ex.Message}");
                TryKillProcess(PsrExe);
            }
        }

        #endregion

        #region Private Methods

        private Process InvokeProcess(string processName, string parameters)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = processName,
                    Arguments = parameters,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    UseShellExecute = true,
                    Verb = "runas",
                    CreateNoWindow = true
                };

                _logger.LogInformation($"Executing {processName} with arguments: {parameters}");
                return Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error executing {processName}.");
                throw;
            }
        }

        private void TryKillProcess(string processName)
        {
            try
            {
                var processes = Process.GetProcessesByName(processName);
                foreach (var proc in processes)
                {
                    proc.Kill();
                }
                _logger.LogInformation($"Successfully killed all instances of {processName}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error trying to kill process {processName}.");
                throw;
            }
        }

        #endregion
    }
}
