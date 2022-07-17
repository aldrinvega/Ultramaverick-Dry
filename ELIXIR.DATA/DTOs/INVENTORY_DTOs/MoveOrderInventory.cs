using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.INVENTORY_DTOs
{
    public class MoveOrderInventory
    {
        public string ItemCode { get; set; }
        public decimal QuantityOrdered { get; set; }

    }
}
