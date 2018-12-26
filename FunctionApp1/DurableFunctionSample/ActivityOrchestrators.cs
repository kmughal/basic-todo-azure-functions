//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace FunctionApp1.DurableFunctionSample
//{
//    public static class ActivityOrchestrators
//    {
//        [FunctionName("O_process_step_1")]
//        public static async Task<object> ProcessActivity(
//            [OrchestrationTrigger] DurableOrchestrationContext ctx,
//            TraceWriter log
//            )
//        {
//            var processId = ctx.GetInput<string>();
//            if (!ctx.IsReplaying) log.Info("staring step1");
//            var list = await ctx.CallActivityAsync<List<object>>("A_Step_1", processId);

//            if (!ctx.IsReplaying) log.Info("staring step2");
//            var mapValues = await ctx.CallActivityAsync<List<object>>("A_Step_2", list);

//            return new
//            {
//                list , mapValues
//            };
//        }
//    }
//}
