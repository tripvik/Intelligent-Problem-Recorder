using System.ComponentModel.DataAnnotations;

namespace TroubleTrack.Models
{
    public class ConnectionInfo
    {
        [Required]
        public string Host { get; set; }

        [Required]
        public int Port { get; set; }

        public bool UseSsl { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string ApplicationName { get; set; }
    }
}
