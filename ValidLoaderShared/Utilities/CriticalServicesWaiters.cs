using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValidLoaderShared.Utilities
{
    using Microsoft.Data.SqlClient;
    using RabbitMQ.Client;
    using System;

    using System.Threading.Tasks;

    public static class CriticalServicesWaiters
    {
        public static async Task WaitForSqlServer(string connectionString, int retryIntervalSeconds = 5)
        {
            while (true)
            {
                try
                {
                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            Console.WriteLine("Connected to SQL Server.");
                            break;
                        }
                    }
                }
                catch (SqlException ex)
                {
                    Console.WriteLine($"Failed to connect to SQL Server: {ex.Message}. Retrying in {retryIntervalSeconds} seconds...");
                }

                await Task.Delay(TimeSpan.FromSeconds(retryIntervalSeconds));
            }
        }
            public static async Task WaitForRabbitMQAndQueue(string hostName, string queueName, int retryIntervalSeconds = 5)
        {
            while (true)
            {
                try
                {
                    var factory = new ConnectionFactory { HostName = hostName };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: queueName,
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        if (connection.IsOpen && channel.IsOpen)
                        {
                            Console.WriteLine($"Connected to RabbitMQ and verified existence of queue '{queueName}'.");
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect to RabbitMQ or verify queue '{queueName}': {ex.Message}. Retrying in {retryIntervalSeconds} seconds...");
                }

                await Task.Delay(TimeSpan.FromSeconds(retryIntervalSeconds));
            }
        }
    }

}
