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
    using Microsoft.WindowsAzure.Storage.Table;

    public static class TodosFunctions
    {
        public class Todo : TableEntity
        {

            public string Id { set; get; }
            public string Name { set; get; }
            public string Description { set; get; }

            public Todo(string name, string description)
            {
                Name = name;
                Description = description;
                Id = Guid.NewGuid().ToString();
                PartitionKey = "Todos";
                RowKey = Id;
            }

            public Todo()
            {

            }
        }
        public class TodoCreateViewModel
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        [FunctionName("Todo")]
        public static async Task<IActionResult> CreateTodo([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Todo")] HttpRequest req,
         [Queue("Todos", Connection = "AzureWebJobsStorage")]IAsyncCollector<Todo> todoQueue,
         [Table("Todos", Connection = "AzureWebJobsStorage")]IAsyncCollector<Todo> todoTable,
            ILogger log)
        {
            log.LogInformation("Creating a new todo");
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<TodoCreateViewModel>(requestBody);

            var item = new Todo(data.Name, data.Description)
            {
                Description = data.Description,
                Name = data.Name
            };

            await todoTable.AddAsync(item);
            await todoQueue.AddAsync(item);
            return new OkObjectResult(item);
        }

        [FunctionName("GetTodo")]
        public static async Task<IActionResult> GetTodo(
            [HttpTrigger(
            AuthorizationLevel.Anonymous,"get" , Route = "Todo/{id}"
            )]HttpRequest req,
            [Table("Todos", Connection = "AzureWebJobsStorage")] CloudTable cloudTable,
            ILogger log,
            string id
            )
        {
            log.LogInformation($"Get {id}");
            var tableOperation = TableOperation.Retrieve<Todo>("Todos", id);

            var result = await cloudTable.ExecuteAsync(tableOperation);
            if (result.Result == null)
            {
                return new NotFoundObjectResult(id);
            }

            var found = result.Result;
            return new OkObjectResult(found);
        }

        [FunctionName("DeleteTodo")]
        public static async Task<IActionResult> DeleteTodo(
            [HttpTrigger(
            AuthorizationLevel.Anonymous,"delete" , Route = "Todo/{id}"
            )]HttpRequest req,
            [Table("Todos", Connection = "AzureWebJobsStorage")] CloudTable cloudTable,
            ILogger log,
            string id
            )
        {
            log.LogInformation($"Deleting {id}");
            var entityToDelete = new Todo { RowKey = id, PartitionKey = "Todos", ETag = "*" };
            var tableOperation = TableOperation.Delete(entityToDelete);
            var result = await cloudTable.ExecuteAsync(tableOperation);
            if (result.Result == null)
            {
                return new NotFoundObjectResult(id);
            }

            var found = result.Result;
            return new OkObjectResult(found);
        }

        [FunctionName("GetAllTodos")]
        public static async Task<IActionResult> GetAllTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetAllTodos")] HttpRequest req,
            [Table("Todos", Connection = "AzureWebJobsStorage")] CloudTable cloudTable,
                                                                 ILogger log
            )
        {
            log.LogInformation("Building query to get all todos");
            var tableQueryToReadAllTodos = new TableQuery<Todo>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Todos"));

            var results = new List<Todo>();
            TableContinuationToken token = null;
            log.LogInformation("Starting running the query to get all todos");
            var queryResults = await cloudTable.ExecuteQuerySegmentedAsync(tableQueryToReadAllTodos, token);
            log.LogInformation($"Got {queryResults.Count()} items. ");

            return new OkObjectResult(queryResults);
        }
    }
}
