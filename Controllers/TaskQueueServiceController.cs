namespace TaskQueueController.Controllers
{
    using Microsoft.AspNetCore.Connections;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using ValidLoaderShared.Consts;
    using ValidLoaderShared.Context;
    using ValidLoaderShared.Models;
    using ValidLoaderShared.Structs;

    [Route("api/[controller]")]
    [ApiController]
    public class TaskQueueServiceController : ControllerBase
    {
        private readonly PageLoaderServiceContext _context; // Replace with your actual DbContext
        private readonly ConnectionFactory _factory;
        public TaskQueueServiceController(PageLoaderServiceContext context)
        {
            _context = context;
            _factory = new ConnectionFactory()
            {
                HostName = "localhost",
            };
        }

        // POST: api/TaskQueueService
        [HttpPost]
        public async Task<ActionResult<int>> CreateLoadTask(string url)
        {
            Console.WriteLine($"CreateLoadTask: {url}");
            // Generate unique ID (consider using a more robust method)
            int loadTaskId = new Random().Next();
            // Create a new LoadTask object
            var loadTask = new NewLoadTask
            {
                Url = url,
                TaskId = loadTaskId
            };

            PublishTaskToQueue(loadTask);

            // Return the TID to the client
            return Ok(loadTaskId);
        }

        // Inside the TaskQueueServiceController class
        [HttpGet("GetAllResults")]
        public async Task<ActionResult<IEnumerable<TaskProcessingResult>>> GetAllResults()
        {
            try
            {

                var results = await _context.Results.ToListAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                // Log the exception and return an appropriate error response
                // For example, you can return a 500 Internal Server Error
                return StatusCode(500, "An error occurred while retrieving the results.");
            }
        }

        // Inside the TaskQueueServiceController class

        [HttpGet("CheckTaskStatus/{taskId}")]
        public async Task<ActionResult<TaskProcessingResult>> CheckTaskStatus(int taskId)
        {
            try
            {
                Console.WriteLine($"CheckTaskStatus: {taskId}");
                var taskResult = await _context.Results
                                              .FirstOrDefaultAsync(r => r.LoadTaskId == taskId);

                if (taskResult == null)
                {
                    return NotFound($"Task with ID {taskId} not found or not yet completed.");
                }

                return Ok(taskResult);
            }
            catch (Exception ex)
            {
                // Log the exception and return an appropriate error response
                return StatusCode(500, "An error occurred while checking the task status.");
            }
        }

        private void PublishTaskToQueue(NewLoadTask loadTask)
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: VLConstants.TaskNotificationQueue,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var jsonMessage = JsonConvert.SerializeObject(loadTask);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                //string message = JsonSerializer.Serialize(loadTask);
                //var body = System.Text.Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: VLConstants.TaskNotificationQueue,
                                     basicProperties: null,
                                     body: body);
            }
        }
    }

}
