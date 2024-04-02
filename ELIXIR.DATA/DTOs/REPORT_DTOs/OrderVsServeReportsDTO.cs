using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.REPORT_DTOs;
public class OrderVsServeReportsDTO
{
    public int? OrderNo { get; set; }
    public string CustomerCode { get; set; }
    public string CustomerName { get; set; }
    public string ItemCode { get; set; }
    public string ItemDescription { get; set; }
    public string Uom { get; set; }
    public string Category { get; set; }
    public decimal? QuantityOrdered { get; set; }
    public decimal QuantityServed { get; set; }
    public decimal Variance { get; set; }
    public decimal Percentage { get; set; }
}
