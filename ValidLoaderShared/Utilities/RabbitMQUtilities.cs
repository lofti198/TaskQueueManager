using RabbitMQ.Client;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValidLoaderShared.Consts;
using ValidLoaderShared.Structs;

namespace ValidLoaderShared.Utilities
{
    public class RabbitMQUtilities
    {
        public static void PublishTaskToQueue(NewLoadTask loadTask, ConnectionFactory factory)
        {
            using (var connection = factory.CreateConnection())
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
