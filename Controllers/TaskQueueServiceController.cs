namespace TaskQueueController.Controllers
{
    using Microsoft.AspNetCore.Connections;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using RabbitMQ.Client;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using ValidLoaderShared.Consts;
    using ValidLoaderShared.Context;
    using ValidLoaderShared.Models;
    using ValidLoaderShared.Structs;
    using ValidLoaderShared.Utilities;
    using ValidLoaderShared.Utilities.Logging;

    [Route("api/[controller]")]
    [ApiController]
    public class TaskQueueServiceController : ControllerBase
    {
        private readonly PageLoaderServiceContext _context; // Replace with your actual DbContext
        private readonly ConnectionFactory _factory;
        private readonly ISimplifiedLogger _logger;

     
        //public TaskQueueServiceController(PageLoaderServiceContext context)
        //{
        //    _context = context;
        //    _factory = new ConnectionFactory()
        //    {
        //        HostName = "localhost",
        //    };
        //}

        public TaskQueueServiceController(PageLoaderServiceContext context, ConnectionFactory factory, ISimplifiedLogger logger)
        {
            _context = context;
            _factory = factory;
            _logger = logger;
        }

        // POST: api/TaskQueueService
        [HttpPost]
        public async Task<ActionResult<int>> CreateLoadTask(string url)
        {
            _logger.Log($"CreateLoadTask: {url}");
            // Generate unique ID (consider using a more robust method)
            int loadTaskId = new Random().Next();
            // Create a new LoadTask object
            var loadTask = new NewLoadTask
            {
                Url = url,
                TaskId = loadTaskId
            };
            RabbitMQUtilities.PublishTaskToQueue(loadTask,_factory);

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
                _logger.Log(ex.Message, LogLevel.Error);
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
                _logger.Log($"CheckTaskStatus: {taskId}");
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
                _logger.Log(ex.Message, LogLevel.Error);
                return StatusCode(500, "An error occurred while checking the task status.");
            }
        }

    }

}
