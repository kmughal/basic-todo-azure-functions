//namespace FunctionApp1
//{
//    using System.Threading.Tasks;
//    using Microsoft.Azure.WebJobs;
//    using Microsoft.Azure.WebJobs.Extensions.Http;
//    using Microsoft.Extensions.Logging;
//    using System.Net.Http;

//    public static class WorkFlowSample
//    {
//        [FunctionName("WorkFlowSample")]
//        public static async Task<HttpResponseMessage> Run(
//            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
//            [OrchestrationClient] DurableOrchestrationClient durableOrchestrationClient,
//            ILogger log)
//        {
//            log.LogInformation("C# HTTP trigger function processed a request.");

//            string process = req.RequestUri.ParseQueryString().Get("process");

//            dynamic data = await req.Content.ReadAsAsync<object>();
//            process = process ?? data?.process;
//            if (process == null)
//            {
//                return req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
//            }

//            var id = await durableOrchestrationClient.StartNewAsync("O_process_step_1", process);
//            return durableOrchestrationClient.CreateCheckStatusResponse(req, id);
//        }
//    }
//}
