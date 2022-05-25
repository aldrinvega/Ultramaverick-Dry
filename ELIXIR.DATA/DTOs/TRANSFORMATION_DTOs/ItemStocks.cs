using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.TRANSFORMATION_DTOs
{
    public class ItemStocks
    {

        public int WarehouseId { get; set; }
        public string ItemCode { get; set; }
        public int ExpirationDays  { get; set; }
        public decimal In { get; set; }
        public decimal Out { get; set; }
        public decimal Remaining { get; set; }

    }
}
