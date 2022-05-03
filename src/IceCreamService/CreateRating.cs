using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CreateRating.Models;
using System.Xml;
using System.Net.Http;
using IceCreamRatingService.Models;

namespace CreateRating
{
    public static class CreateRating
    {
        [FunctionName("CreateRating")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [CosmosDB(
                databaseName: "IceCreamRatings",
                collectionName: "Ratings",
                ConnectionStringSetting = "CosmosDBConnection")]IAsyncCollector<Rating> ratingItems,
            ILogger log)
        {
            

            CreateRatingRequest createRating;

            try
            {
                createRating = JsonConvert.DeserializeObject<CreateRatingRequest>(await req.ReadAsStringAsync());
            }
            catch
            {
                return new BadRequestObjectResult("Invalid Request");
            }

            log.LogInformation($"Processing user id: {createRating.UserId}");

            // we have the body, now we need to make a rest call using the userId and product id to the api endpoints

            if (createRating.UserId != null)
            {
                var client = new HttpClient();
                string url = $"https://serverlessohapi.azurewebsites.net/api/GetUser?userId={createRating.UserId}";
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new BadRequestObjectResult("UserId not valid");
                }
            }
            else
            {
                return new BadRequestObjectResult("UserId not found");
            }

            if(createRating.ProductId != null)
            {
                var client = new HttpClient();
                string url = $"https://serverlessohapi.azurewebsites.net/api/GetProduct?productId={createRating.ProductId}";
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new BadRequestObjectResult("ProductId not valid");
                }
            }
            else
            {
                return new BadRequestObjectResult("ProductId not Found");
            }

            if (createRating.rating < 0 || createRating.rating > 5)
                return new BadRequestObjectResult("Rating must be between 0 and 5");

            Rating rating = new Rating
            {
                id = Guid.NewGuid(),
                userId = createRating.UserId,
                productId = createRating.ProductId,
                timestamp = DateTime.Now,
                locationName = createRating.locationName,
                rating = createRating.rating,
                userNotes = createRating.userNotes
            };
            await ratingItems.AddAsync(rating);
            
            return new OkObjectResult(rating);
            
    }
    }
}
