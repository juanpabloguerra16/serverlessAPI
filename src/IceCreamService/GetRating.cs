using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using IceCreamRatingService.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace IceCreamRatingService
{
    public static class GetRating
    {
        [FunctionName("GetRating")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rating/{id}")] HttpRequest req, string id,
            [CosmosDB(
                databaseName: "IceCreamRatings",
                collectionName: "Ratings",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM Ratings r where r.id = {id}")]
                IEnumerable<Rating> rating,
                ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request for id: {id}");

            if(!Guid.TryParse(id, out Guid ratingId))
            {
                return new BadRequestObjectResult("Invalid Rating ID format");
            }

            if(rating.Count() == 0)
            {
                // 404 not found
                return new NotFoundObjectResult("ID not found");
            }
            else
            {
                return new OkObjectResult(rating.FirstOrDefault());
            }




        }
    }
}
