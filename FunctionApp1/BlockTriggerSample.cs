namespace FunctionApp1
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Table;
    using static FunctionApp1.TodosFunctions;

    public static class BlockTriggerSample
    {

        private const char splitter = '\n';
        private const char lineSplitter = ';';

        [FunctionName("BlockTriggerSample")]
        public static async Task Run([BlobTrigger("todos/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob,
            [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable cloudTable,
            string name,
            ILogger log)
        {
            var data = ReadTextFromStream(myBlob, name, log);
            if (string.IsNullOrWhiteSpace(data)) return;

            var todos = ExtractTodos(data);
            await AddTodosToTableStoreAsync(todos, cloudTable, log);
        }

        private static async Task AddTodosToTableStoreAsync(IList<Todo> todos,
            CloudTable cloudTable,
            ILogger log)
        {
            if (todos.Any())
            {
                log.LogInformation($"Todos created , found {todos.Count()}. ");
                var tableBatchOperation = new TableBatchOperation();
                foreach (var todo in todos)
                {
                    tableBatchOperation.Insert(todo);
                }
                await cloudTable.ExecuteBatchAsync(tableBatchOperation);
                log.LogInformation($"Added todos {todos.Count()}. ");
            }
        }

        private static string ReadTextFromStream(Stream stream, string name, ILogger log)
        {
            log.LogInformation($"Reading :{name} \n Size: {stream.Length} Bytes");
            var strReader = new StreamReader(stream);
            return strReader.ReadToEnd();
        }

        private static IList<Todo> ExtractTodos(string data)
        {
            var lines = data.Split(splitter);
            var todos = new List<Todo>();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var items = line.Split(lineSplitter);
                if (items.Length == 3) // last is for \r
                {
                    string todoName = items[0], description = items[1];
                    if (string.IsNullOrWhiteSpace(todoName) || string.IsNullOrWhiteSpace(description)) continue;

                    todos.Add(new Todo(todoName, description));
                }
            }
            return todos;
        }
    }
}
