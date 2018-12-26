namespace FunctionApp1
{
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Blob;
    using static FunctionApp1.TodosFunctions;

    public static class TodoQueueListener
    {
        [FunctionName("TodoQueueListener")]
        public static async Task Run([QueueTrigger("todo", Connection = "AzureWebJobsStorage")]Todo myQueueItem, ILogger log,
            [Blob("todo",Connection = "AzureWebJobsStorage")] CloudBlobContainer cloudBlobContainer )
        {
            await cloudBlobContainer.CreateIfNotExistsAsync();
            var fileId = $"todo-{myQueueItem.Id}.txt";
            var blob = cloudBlobContainer.GetBlockBlobReference(fileId);
            await blob.UploadTextAsync($"ID:{myQueueItem.Id},Name={myQueueItem.Name},Description={myQueueItem.Description}");

            log.LogInformation($"file saved,{fileId}");
        }
    }
}
