namespace FunctionApp1
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Linq;

    public static class Function1
    {
        public class Todo
        {
            public string Id { get; }
            public string Name { get;  }
            public string Description { get; }

            public Todo(string name , string description)
            {
                Name = name;
                Description = description;
                Id = Guid.NewGuid().ToString();
            }
        }
        public class TodoCreateViewModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        private static List<Todo> _todos = new List<Todo>();

        [FunctionName("Todo")]
        public static async Task<IActionResult> CreateTodo([HttpTrigger(AuthorizationLevel.Anonymous,"post",Route ="Todo")] HttpRequest req,
         [Queue("todo",Connection ="AzureWebJobsStorage")]IAsyncCollector<Todo> todoQueue,
            ILogger log)
        {
            log.LogInformation("Creating a new todo");
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<TodoCreateViewModel>(requestBody);

            var item = new Todo(data.Name, data.Description);
            _todos.Add(item);
            await todoQueue.AddAsync(item);
            return new OkObjectResult(item);

        }

        [FunctionName("GetTodo")]
        public static IActionResult GetTodo(
            [HttpTrigger(
            AuthorizationLevel.Anonymous,"get" , Route = "Todo/{name}"
            )]HttpRequest req,
            ILogger log,
            string name
            )
        {
            log.LogInformation($"Get {name}");
            var result = _todos.Where(x => x.Name.Contains(name)).ToList();
            return new OkObjectResult(result);
        }
    }
}
