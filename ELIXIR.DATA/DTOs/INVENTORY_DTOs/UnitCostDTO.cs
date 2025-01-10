using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.INVENTORY_DTOs;
public class UnitCostDTO
{
    public int WarehouseId { get; set; }
    public string ItemCode { get; set; }
    public decimal ActualGood { get; set; }
    public decimal? TotalUnitPrice { get; set; }

    public string ExpirationDate { get; set; }
    public int ExpirationDays { get; set; }
    public decimal UnitCost
    {
        get;
        set;
    }
}
