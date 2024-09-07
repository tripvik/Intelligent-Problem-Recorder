using TroubleTrack.Models;

namespace TroubleTrack.Services.Exploration;
public class AppExploreService
{
    private readonly List<ApplicationDetail> _applications = new List<ApplicationDetail>();

    public void AddApplication(ApplicationDetail appDetail)
    {
        _applications.Add(appDetail);
    }

    public ApplicationDetail GetApplication(int id)
    {
        return _applications.FirstOrDefault(app => app.Id == id);
    }

    public IEnumerable<ApplicationDetail> GetAllApplications()
    {
        return _applications;
    }

    public IEnumerable<ApplicationDetail> GetApplicationsByType(ApplicationType type)
    {
        return _applications.Where(app => app.Type == type);
    }

    public bool UpdateApplication(ApplicationDetail updatedApp)
    {
        var existingApp = _applications.FirstOrDefault(app => app.Id == updatedApp.Id);
        if (existingApp == null) return false;

        existingApp.Name = updatedApp.Name;
        existingApp.Type = updatedApp.Type;
        existingApp.Version = updatedApp.Version;
        existingApp.Path = updatedApp.Path;
        existingApp.IsActive = updatedApp.IsActive;

        return true;
    }

    public bool RemoveApplication(int id)
    {
        var app = _applications.FirstOrDefault(a => a.Id == id);
        if (app == null) return false;

        _applications.Remove(app);
        return true;
    }

    public bool RemoveApplicationByType(ApplicationType type)
    {
        _applications.RemoveAll(app => app.Type == type);
        return true;
    }
}
