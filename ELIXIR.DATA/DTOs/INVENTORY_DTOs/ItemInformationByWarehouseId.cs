using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.INVENTORY_DTOs;
public class ItemInformationByWarehouseId
{
    public int ReceivingId { get; set; }
    public string ItemCode { get; set; }
    public string ItemDescription { get; set; }
    public string UOM { get; set; }
    public string Supplier { get; set; }
    public decimal QuantityGood { get; set; }
    public string ReceivedDate { get; set; }
    public string ExpirationDate { get; set; }
    public string LotCategory { get; set; }
}
