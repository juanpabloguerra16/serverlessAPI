using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrderBatchService.Models;
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
namespace OrderBatchService
{
    public static class EventGridTrigger
    {
        [FunctionName("EventGridTrigger")]
        public static async Task RunEventGridTrigger(
            [EventGridTrigger] EventGridEvent eventGridEvent,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            log.LogInformation($"Received event grid blob created event: {Environment.NewLine}{eventGridEvent.Data.ToString()}");

            try
            {
                JObject json = JObject.Parse(eventGridEvent.Data.ToString());
                if (json.TryGetValue("url", out JToken urlToken))
                {
                    string url = urlToken.ToString();
                    log.LogInformation($"New file created at url={url}");

                    // Use the file name as the batchId and gate for processing
                    // The prefix is the batchId
                    // The files are the gates
                    string fileName = url.Substring(url.LastIndexOf('/') + 1);
                    string[] fileNameTokens = fileName.Split('-');
                    string batchId = fileNameTokens[0];
                    string file = fileNameTokens[1];

                    // Check if an instance with the specified ID already exists or an existing one stopped running(completed/failed/terminated).
                    var existingInstance = await client.GetStatusAsync(batchId);
                    if (existingInstance == null
                    || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Completed
                    || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Failed
                    || existingInstance.RuntimeStatus == OrchestrationRuntimeStatus.Terminated)
                    {
                        // The client can only start a new instance once
                        // Use instance id when events come from an external source or when implementing singleton orchestrator
                        // https://github.com/Azure/azure-functions-durable-extension/issues/1347
                        // https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-singletons?tabs=csharp
                        // https://docs.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-instance-management?tabs=csharp
                        log.LogInformation($"Starting orchestration with instance id={batchId}");
                        string instanceId = await client.StartNewAsync("MonitorOrderEvents", batchId, batchId);
                    }

                    log.LogInformation($"Raising orchestration event with batchId={batchId}, file={file}");
                    await client.RaiseEventAsync(batchId, file, batchId);
                }
                else
                {
                    log.LogWarning("No url found in the event grid payload");
                }

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error processing event grid event");
            }

        }

        [FunctionName("MonitorOrderEvents")]
        public static async Task RunMonitorBatch(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            string batchId = context.GetInput<string>();
            log.LogInformation($"Monitor called for batchId={batchId}");

            var gate1 = context.WaitForExternalEvent($"OrderHeaderDetails.csv");
            var gate2 = context.WaitForExternalEvent($"OrderLineItems.csv");
            var gate3 = context.WaitForExternalEvent($"ProductInformation.csv");

            // All three files must be created before the batch can be processed
            await Task.WhenAll(gate1, gate2, gate3);

            log.LogInformation($"Received all files for batchId={batchId}");
            await context.CallActivityAsync("ProcessBatch", batchId);
        }

        [FunctionName("ProcessBatch")]
        public static async Task RunProcessBatch(
            [ActivityTrigger] string batchId,
            [CosmosDB(
                databaseName: "IceCreamRatings",
                collectionName: "batch2",
                ConnectionStringSetting = "CosmosDBConnection")]IAsyncCollector<dynamic> combinedOrderCosmos,
            ILogger log)
        {
            // TODO: Call the API
            // https://petstore.swagger.io/?url=https://serverlessohmanagementapi.trafficmanager.net/api/definition#/Register%20Storage%20Account/combineOrderContent
            log.LogInformation($"Processing batchId={batchId}");

            var client = new HttpClient();
            string url = "https://serverlessohmanagementapi.trafficmanager.net/api/order/combineOrderContent";
            string baseUrl = "https://platformdevjpguerra.blob.core.windows.net/batch2/";
            
            combineOrder order = new combineOrder
            {
                orderHeaderDetailsCSVUrl =  $"{baseUrl}{batchId}-OrderHeaderDetails.csv",
                orderLineItemsCSVUrl = $"{baseUrl}{batchId}-OrderLineItems.csv",
                productInformationCSVUrl = $"{baseUrl}{batchId}-ProductInformation.csv",
            };

            //request
            var json = JsonConvert.SerializeObject(order);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync(url, content).Result;

            // convert response to dynamic object ot be passed as input to cosmosDB
            var combinedOrder = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
            
            

            if (!response.IsSuccessStatusCode)
                log.LogInformation("Fetching combined csv was unsuccessful");
            else
                await combinedOrderCosmos.AddAsync(combinedOrder);


        }

    }
}