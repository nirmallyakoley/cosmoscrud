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

namespace DemoFunctionApp
{
    public static class GetItem
    {
        [FunctionName("GetItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string id = req.Query["id"];

            if (!string.IsNullOrEmpty(id)) {

                CosmosPOC cosmosPOC = new CosmosPOC();
                cosmosPOC.Initialize();
                await cosmosPOC.CreateDatabaseAsync();
                await cosmosPOC.CreateContainerAsync();
                try
                {
                   List<Customer> customers= await cosmosPOC.QueryItemsAsync(id);
                    if (customers.Count > 0)
                    {
                        return new OkObjectResult(customers[0]);
                    }
                   
                }
                catch (Exception ex)
                {

                    log.LogInformation("Error processed a request.");
                }

                


            }
            return new StatusCodeResult(204);


        }
    }
}
