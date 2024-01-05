using System.ComponentModel.DataAnnotations;

namespace ValidLoaderShared.Models
{
    public class Domain
    {
        [Key]
        public string DomainName { get; set; } // Unique identifier for a domain

        // Navigation property to relate with LoadingInfo
        public virtual ICollection<LoadingInfo> LoadingInfos { get; set; }
    }

}
