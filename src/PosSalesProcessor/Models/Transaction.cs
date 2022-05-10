using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosSalesProcessor.Models
{

    public class Transaction
    {
        public Header header { get; set; }
        public Detail[] details { get; set; }
    }

    public class Header
    {
        public string salesNumber { get; set; }
        public DateTime dateTime { get; set; }
        public string locationId { get; set; }
        public string locationName { get; set; }
        public string locationAddress { get; set; }
        public string locationPostcode { get; set; }
        public decimal totalCost { get; set; }
        public string totalTax { get; set; }
        public string receiptUrl { get; set; }
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
