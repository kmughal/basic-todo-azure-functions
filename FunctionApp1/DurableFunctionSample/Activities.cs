//using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.Host;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace FunctionApp1.DurableFunctionSample
//{
//    //Activity Classes: Can't have IO operations , can't have long loops must be deterministic.
     
//    public static class Activities
//    {
//        [FunctionName("A_Step_1")]
//        public static async Task<List<object>> ActivityStep1([ActivityTrigger] string process, TraceWriter log)
//        {
//            await Task.Delay(5);
//            log.Info("in step 1");
//            return new List<object> { "khurram", "mughal" };
//        }

//        [FunctionName("A_Step_2")]
//        public static async Task<List<object>> ActivityStep2([ActivityTrigger] List<object> list, TraceWriter log)
//        {
//            await Task.Delay(5);
//            list.Reverse();
//            log.Info("in step 2");
//            return list;
//        }

        
//    }
//}
