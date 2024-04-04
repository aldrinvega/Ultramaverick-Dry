using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.INVENTORY_DTOs;
public class WarehouseInventoryWIthBbd
{
    public int WarehouseId { get; set; }
    public string ItemCode { get; set; }
    public string ItemDescription { get; set; }
    public decimal ActualGood { get; set; }
    public string Uom { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public int ExpirationDays { get; set; }
}
