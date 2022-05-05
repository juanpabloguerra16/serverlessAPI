using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBatchService.Models
{
    public class combinedOrderCosmosDB
    {
        public Schema[] Property1 { get; set; }
    }

    public class Schema
    {
        public Headers headers { get; set; }
        public Detail[] details { get; set; }
    }

    public class Headers
    {
        public string salesNumber { get; set; }
        public string dateTime { get; set; }
        public string locationId { get; set; }
        public string locationName { get; set; }
        public string locationAddress { get; set; }
        public string locationPostcode { get; set; }
        public string totalCost { get; set; }
        public string totalTax { get; set; }
    }

    public class Detail
    {
        public string productId { get; set; }
        public string quantity { get; set; }
        public string unitCost { get; set; }
        public string totalCost { get; set; }
        public string totalTax { get; set; }
        public string productName { get; set; }
        public string productDescription { get; set; }
    }

}
