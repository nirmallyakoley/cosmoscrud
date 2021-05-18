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

using System.Net;
using System.Net.Http.Formatting;
using Microsoft.Azure.Cosmos;

namespace DemoFunctionApp
{
    public static class CreateItem
    {
        [FunctionName("CreateItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            Customer data = JsonConvert.DeserializeObject<Customer>(requestBody);
            CosmosPOC cosmosPOC = new CosmosPOC();
            cosmosPOC.Initialize();
            await cosmosPOC.CreateDatabaseAsync();
            await cosmosPOC.CreateContainerAsync();
            try
            {
                await cosmosPOC.AddItemsToContainerAsync(data);
            }
            catch(Exception ex){

                log.LogInformation("Error processed a request.");
            }


            return new StatusCodeResult(201);
    }
}
}
    

