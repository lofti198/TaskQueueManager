using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidLoaderShared.Models
{

    public class TaskProcessingResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResultId { get; set; } // Primary key, auto-generated
        public int LoadTaskId { get; set; } // Loaded Task Iв
        public string Content { get; set; } // Content of the loaded webpage

        public string Error { get; set; } // Error of loading
       
    }
}
