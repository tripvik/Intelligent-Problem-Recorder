namespace TroubleTrack.Models
{
    public class ApplicationDetail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ApplicationType Type { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }
        public string CommandLine { get; set; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
        public bool IsInstrumented { get; set; }
    }
}