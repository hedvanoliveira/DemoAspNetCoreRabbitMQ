using DemoAspNetCoreRabbitMQ.SendMessage.Models;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DemoAspNetCoreRabbitMQ.SendMessage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private const string QUEUE_NAME = "messages";
        private readonly ConnectionFactory _factory;

        public MessageController()
        {
            _factory = new ConnectionFactory
            {
                HostName = "host.docker.internal", 
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
        }

        // GET: api/<MessageController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<MessageController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<MessageController>
        [HttpPost]
        public IActionResult Post([FromBody] Message message)
        {
            using(var connection = _factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(
                        queue: QUEUE_NAME,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    var json = JsonSerializer.Serialize(message);
                    var byteArray = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(
                        exchange: "",
                        routingKey: QUEUE_NAME,
                        basicProperties: null,
                        body: byteArray
                        );
                }
            }
            return Accepted();
        }

        // PUT api/<MessageController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<MessageController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
