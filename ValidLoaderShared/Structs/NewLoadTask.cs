using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidLoaderShared.Models;

namespace ValidLoaderShared.Structs
{
    public struct NewLoadTask
    {
        public NewLoadTask(int taskId, string url)
        {
            TaskId = taskId;
            Url = url;
        }
        public string Url { get; set; } // URL to be loaded
        public int TaskId { get; set; } 
    }

}
