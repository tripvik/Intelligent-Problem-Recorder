using Blazored.LocalStorage;
using TroubleTrack.Models;

namespace TroubleTrack.Services.Presentation
{
    public class AppStateService
    {
        private readonly ILocalStorageService _localStorage;

        private ConnectionInfo _connectionInfo;
        private List<ApplicationDetail> _instrumentedApps;
        private List<Agent> _installedAgents;

        private const string CONNECTION_INFO_KEY = "ConnectionInfo";
        private const string INSTRUMENTED_APPS_KEY = "InstrumentedApps";
        private const string INSTALLED_AGENTS_KEY = "InstalledAgents";

        public AppStateService(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<ConnectionInfo> GetConnectionInfoAsync()
        {
            if (_connectionInfo == null && await _localStorage.ContainKeyAsync(CONNECTION_INFO_KEY))
            {
                _connectionInfo = await _localStorage.GetItemAsync<ConnectionInfo>(CONNECTION_INFO_KEY);
            }

            return _connectionInfo;
        }

        public async Task SetConnectionInfoAsync(ConnectionInfo connectionInfo)
        {
            await _localStorage.SetItemAsync(CONNECTION_INFO_KEY, connectionInfo);
            _connectionInfo = connectionInfo;
        }

        public async Task<List<ApplicationDetail>> GetInstrumentedAppsAsync()
        {
            if (_instrumentedApps == null && await _localStorage.ContainKeyAsync(INSTRUMENTED_APPS_KEY))
            {
                _instrumentedApps = await _localStorage.GetItemAsync<List<ApplicationDetail>>(INSTRUMENTED_APPS_KEY);
            }

            return _instrumentedApps;
        }

        public async Task SetInstrumentedAppsAsync(List<ApplicationDetail> instrumentedApps)
        {
            await _localStorage.SetItemAsync(INSTRUMENTED_APPS_KEY, instrumentedApps);
            _instrumentedApps = instrumentedApps;
        }

        public async Task<List<Agent>> GetInstalledAgentsAsync()
        {
            if (_installedAgents == null && await _localStorage.ContainKeyAsync(INSTALLED_AGENTS_KEY))
            {
                _installedAgents = await _localStorage.GetItemAsync<List<Agent>>(INSTALLED_AGENTS_KEY);
            }

            return _installedAgents;
        }

        public async Task SetInstalledAgentsAsync(Agent agent)
        {
            var agents = await GetInstalledAgentsAsync() ?? new();
            agents.RemoveAll(x => x.Type == agent.Type);
            agents.Add(agent);
            await _localStorage.SetItemAsync(INSTALLED_AGENTS_KEY, agents);
            _installedAgents = agents;
        }
    }
}