using CliWrap;
using CliWrap.Buffered;
using TroubleTrack.Models;

namespace TroubleTrack.Services.Exploration;
public class DotnetExplorerService
{
    private readonly AppExploreService _applicationService;

    public DotnetExplorerService(AppExploreService applicationService)
    {
        _applicationService = applicationService;
    }

    public async Task<List<ApplicationDetail>> DetectAndAddRunningDotNetApplicationsAsync()
    {
        var cmdResult = await Cli.Wrap("powershell")
            .WithArguments(new[] { "-Command", @"Get-Process -ErrorAction Ignore | Where-Object { $_.Modules.ModuleName -contains 'mscoree.dll' -or $_.Modules.ModuleName -contains 'coreclr.dll' } | Select-Object ProcessName, Id, Path | Format-Table -AutoSize | Out-String -Width 4096" })
            .ExecuteBufferedAsync();

        if (cmdResult.ExitCode != 0)
        {
            throw new Exception("Failed to fetch the list of processes.");
        }

        var dotNetApps = ParsePowershellOutputToApplicationDetails(cmdResult.StandardOutput);

        _applicationService.RemoveApplicationByType(ApplicationType.DotNet);

        foreach (var app in dotNetApps)
        {
            _applicationService.AddApplication(app);
        }

        return dotNetApps;
    }

    private List<ApplicationDetail> ParsePowershellOutputToApplicationDetails(string output)
    {
        var lines = output.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l) && !l.Contains("ProcessName")).ToList();
        var dotNetApps = new List<ApplicationDetail>();

        foreach (var line in lines)
        {
            var details = line.Trim().Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
            if (details.Length < 3) continue;  // This ensures we skip lines with incomplete data

            var processName = details[0];

            if (!int.TryParse(details[1], out var processId))
                continue; // Skip if the ID is not a valid integer

            var processPath = details[2];

            dotNetApps.Add(new ApplicationDetail
            {
                Name = processName,
                Type = ApplicationType.DotNet,
                Id = processId,
                Path = processPath,
                IsActive = true
            });
        }

        return FilteredProcesses(dotNetApps);
    }



    List<ApplicationDetail> FilteredProcesses(List<ApplicationDetail> processes)
    {
        var unwantedProcesses = new List<string>
        {
            "devenv",
            "ServiceHub.*",
            "VBCSCompiler",
            "WebViewHost",
            "MSBuild",
            "powershell",
            "powershell_ise",
            // Add any other unwanted process names or patterns here.
        };

        var unwantedPaths = new List<string>
        {
            @"C:\Program Files\Microsoft Visual Studio\",
            @"C:\Program Files\Microsoft SQL Server",
            @"Microsoft.NET\Framework64",
            @"TroubleTrack.exe",
            @"wbem\wmiprvse.exe",
            @"C:\Program Files\AppDynamics",
            // Add other unwanted path patterns here.
        };

        return processes.Where(p =>
              !unwantedProcesses.Contains(p.Name) &&
              !unwantedPaths.Any(up => p.Path.Contains(up, StringComparison.OrdinalIgnoreCase))
          ).ToList();
    }
}