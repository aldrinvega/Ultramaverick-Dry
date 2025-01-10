using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.INVENTORY_DTOs;
public class ADDMISCDTO
{
    public int WarehouseId { get; set; }
    public string ItemCode { get; set; }
    public string ItemDescription { get; set; }
    public string UOM { get; set; }
    public string Customer { get; set; }
    public string CustomerCode { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string Remarks { get; set; }
    public string Details { get; set; }
    public string PreparedBy { get; set; }
}
