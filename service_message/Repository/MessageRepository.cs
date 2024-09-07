using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using service_message.Models;
using service_message.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MessageService.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly string _connectionString;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(IConfiguration configuration, ILogger<MessageRepository> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;


            InitializeDatabase();
        }

        private IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public void InitializeDatabase()
        {
            const string createTableSql = @"
            CREATE TABLE IF NOT EXISTS messages (
                id SERIAL PRIMARY KEY,
                text TEXT NOT NULL,
                time TIMESTAMP NOT NULL,
                order_number INT NOT NULL
            );";

            try
            {
                using (var connection = CreateConnection())
                {
                    connection.Open();
                    connection.Execute(createTableSql);
                    _logger.LogInformation("Database and table created successfully (if they did not exist).");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create database or table.");
                throw;
            }
        }

        public async Task CreateAsync(Message message)
        {
            const string query = "INSERT INTO messages (text, time, order_number) VALUES (@Text, @Time, @Order)";
            try
            {
                using (var connection = CreateConnection())
                {
                    await connection.ExecuteAsync(query, message);
                    _logger.LogInformation("Message created successfully: {Text}", message.Text);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create message: {Text}", message.Text);
                throw;
            }
        }

        public async Task<IEnumerable<Message>> GetMessagesSinceAsync(DateTime since)
        {
            const string query = "SELECT id, text, time, order_number AS Order FROM messages WHERE time >= @Since";
            try
            {
                using (var connection = CreateConnection())
                {
                    var messages = await connection.QueryAsync<Message>(query, new { Since = since });
                    _logger.LogInformation("Successfully fetched messages since {Since}", since);
                    return messages;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch messages since {Since}", since);
                throw;
            }
        }
    }
}
