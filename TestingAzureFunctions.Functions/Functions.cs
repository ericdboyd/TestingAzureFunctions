using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace TestingAzureFunctions.Functions
{
    public static class Functions
    {
        [FunctionName("FunctionWithCreateResponseExtension")]
        public static async Task<HttpResponseMessage> FunctionWithCreateResponseExtension(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            dynamic data = await req.Content.ReadAsAsync<object>();
            string name = data?.name;

            return name != null
                ? req.CreateResponse(HttpStatusCode.OK, $"Hello, {name}")
                : req.CreateErrorResponse(HttpStatusCode.BadRequest, "Please pass a name in the request body");
        }

        [FunctionName("FunctionWithoutCreateResponseExtension")]
        public static async Task<HttpResponseMessage> FunctionWithoutCreateResponseExtension(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req,
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            dynamic data = await req.Content.ReadAsAsync<object>();
            string name = data?.name;

            return name != null
                ? new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent($"Hello, {name}") }
                : new HttpResponseMessage(HttpStatusCode.BadRequest) { Content = new StringContent("Please pass a name in the request body") };
        }
    }
}
