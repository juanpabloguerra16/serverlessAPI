using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosSalesProcessor.Models
{

    public class FilteredReceipt
    {
        public string Store { get; set; }
        public string SalesNumber { get; set; }
        public decimal TotalCost { get; set; }
        public int Items { get; set; }
        public DateTime SalesDate { get; set; }
        public string ReceiptImageBase64 { get; set; }
    }

}
