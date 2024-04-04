using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Configuration;

namespace ELIXIR.DATA.DTOs.REPORT_DTOs;
public class ItemswithBBDDTO
{
    public int WarehouseId { get; set; }
    public string ItemCode { get; set; }
    public string ItemDescription { get; set; }
    public string UOM { get; set; }
    public string BBD { get; set; }
}
