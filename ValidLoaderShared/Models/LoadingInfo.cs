using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ValidLoaderShared.Models
{
    public class LoadingInfo
    {
        [Key]
        public int LoadingInfoId { get; set; } // Primary key

        public bool IsSuccess { get; set; } // Indicates if the loading was successful
        public DateTime Timestamp { get; set; } // The time when the attempt was made

        // Foreign keys
        public string ProxyAddress { get; set; }
        public string DomainName { get; set; }

        // Navigation properties for relationships
        [ForeignKey("ProxyAddress")]
        public virtual Proxy Proxy { get; set; }

        [ForeignKey("DomainName")]
        public virtual Domain Domain { get; set; }
    }

}
