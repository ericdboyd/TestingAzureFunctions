using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TraceLevel = System.Diagnostics.TraceLevel;

namespace TestingAzureFunctions.Tests
{
    extern alias webapicompatshim;

    [TestClass]
    public class FunctionTest
    {
        [TestMethod]
        public async Task FunctionWithoutCreateResponseExtension_Test()
        {
            var url = "http://TestingAzureFunctions/Test";
            var content = "{\"name\":\"Azure Function Test\"}";

            var request = BuildHttpPostRequestMessage(url, content);
            var traceWriter = new TraceWriterStub(TraceLevel.Verbose);

            var response = await Functions.Functions.FunctionWithoutCreateResponseExtension(request, traceWriter);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Hello, Azure Function Test", responseContent);
        }

        [TestMethod]
        public async Task FunctionWithCreateResponseExtension_Test()
        {
            var url = "http://TestingAzureFunctions/Test";
            var content = "{\"name\":\"Azure Function Test\"}";

            var request = BuildHttpPostRequestMessage(url, content);
            var traceWriter = new TraceWriterStub(TraceLevel.Verbose);

            var response = await Functions.Functions.FunctionWithCreateResponseExtension(request, traceWriter);
            var responseContent = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Hello, Azure Function Test", responseContent);
        }

        private static HttpRequestMessage BuildHttpPostRequestMessage(string url, string content)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddOptions();
            services.AddSingleton(typeof(IContentNegotiator), typeof(DefaultContentNegotiator));
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext {RequestServices = serviceProvider};
            
            var httpConfiguration = new HttpConfiguration();

            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),
                Content = new StringContent(content, Encoding.UTF8, "application/json"),
                Properties =
                {
                    { HttpPropertyKeys.HttpConfigurationKey, httpConfiguration },
                    { nameof(HttpContext), httpContext} 
                }
            };

            return request;
        }
    }
}
