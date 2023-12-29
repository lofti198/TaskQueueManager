using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ValidLoaderShared.Models;
using Newtonsoft.Json;

namespace TaskQueueController.Pages
{
    public class ResultsViewModel : PageModel
    {
        public List<TaskProcessingResult> Results { get; set; }

        public async Task OnGetAsync()
        {
            using (var httpClient = new HttpClient())
            {
                string apiUrl = "http://localhost:5000/api/TaskQueueService/GetAllResults"; // Replace with actual API URL
                var response = await httpClient.GetStringAsync(apiUrl);
                Results = JsonConvert.DeserializeObject<List<TaskProcessingResult>>(response);
            }
        }
    }
}
