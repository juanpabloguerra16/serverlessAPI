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
using IceCreamRatingService.Models;
using System.Linq;

namespace IceCreamRatingService
{
    public static class GetRatings
    {
        [FunctionName("GetRatings")]
        public static  IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "userid/{userId}")] HttpRequest req, string userId,
            [CosmosDB(
                databaseName: "IceCreamRatings",
                collectionName: "Ratings",
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM Ratings r where r.userId = {userId}")]
                IEnumerable<Rating> ratings,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request for userId: {userId}");

            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                return new BadRequestObjectResult("Invalid user ID format");
            }

            if (ratings.Count() == 0)
            {
                return new NotFoundObjectResult("Records not found");
            }
            else
            {
                return new OkObjectResult(ratings);
            }




        }
    }
}
