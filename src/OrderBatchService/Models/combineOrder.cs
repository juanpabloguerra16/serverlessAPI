using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBatchService.Models
{
    public class combineOrder
    {
        public string orderHeaderDetailsCSVUrl { get; set; }
        public string orderLineItemsCSVUrl { get; set; }
        public string productInformationCSVUrl { get; set; }
    }

}
