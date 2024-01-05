using System.ComponentModel.DataAnnotations;

namespace ValidLoaderShared.Models
{
    public class Proxy
    {
        [Key]
        public string ProxyAddress { get; set; } // Assuming proxy is identified by a string like an IP address or a URL

        // Navigation property to relate with LoadingInfo
        public virtual ICollection<LoadingInfo> LoadingInfos { get; set; }
    }
}
