using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using PosSalesProcessor.Models;

namespace PosSalesProcessor
{
    public class ProcessReceipts
    {
        // we are filtering receipts on the code. However, we could have filtered receipts 
        // based on the subscrption
        [FunctionName("ProcessReceipts")]
        public async Task Run(
            [ServiceBusTrigger("orders", Connection = "ServiceBusConnectionString")]string myQueueItem,
            [Blob("receipts/high-value/{rand-guid}.json", FileAccess.Write, Connection = "BlobConnectionString")] CloudBlockBlob output,
            [Blob("receipts/low-value/{rand-guid}.json", FileAccess.Write, Connection = "BlobConnectionString")] CloudBlockBlob output2,
            ILogger log)
        {
            ReceiptData receipt = JsonConvert.DeserializeObject<ReceiptData>(myQueueItem);

            if(receipt.totalCost > 100)
            {
                log.LogInformation("Total cost is above 100");
                WebClient webClient = new WebClient();
                var receiptBytes = await webClient.DownloadDataTaskAsync(receipt.receiptUrl);             

                
                FilteredReceipt filteredReceipt = new FilteredReceipt
                {
                    Store = receipt.storeLocation,
                    Items = receipt.totalItems,
                    SalesDate = receipt.salesDate,
                    SalesNumber = receipt.salesNumber,
                    TotalCost = receipt.totalCost,
                    ReceiptImageBase64 = Convert.ToBase64String(receiptBytes)  
                };
                await output.UploadTextAsync(JsonConvert.SerializeObject(filteredReceipt));
            }
            else
            {
                FilteredReceipt filteredReceipt = new FilteredReceipt
                {
                    Store = receipt.storeLocation,
                    Items = receipt.totalItems,
                    SalesDate = receipt.salesDate,
                    SalesNumber = receipt.salesNumber,
                    TotalCost = receipt.totalCost
                };
                await output2.UploadTextAsync(JsonConvert.SerializeObject(filteredReceipt));

                log.LogInformation("Total cost is below 100");
            }

            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
