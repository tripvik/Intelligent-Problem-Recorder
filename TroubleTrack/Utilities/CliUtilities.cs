using System.Management;

namespace TroubleTrack.Utilities
{
    public static class CliUtilities
    {
        public static string GetCommandLineByProcessId(int processId)
        {
            var searcher = new ManagementObjectSearcher("SELECT CliUtilities FROM Win32_Process WHERE ProcessId = " + processId);
            foreach (ManagementObject obj in searcher.Get())
            {
                return obj["CliUtilities"].ToString();
            }
            return null;
        }
    }
}