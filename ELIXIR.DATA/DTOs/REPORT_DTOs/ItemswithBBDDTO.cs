namespace ELIXIR.DATA.DTOs.REPORT_DTOs;
public class ItemswithBBDDTO
{
    public int WarehouseId { get; set; }
    public string ItemCode { get; set; }
    public string ItemDescription { get; set; }
    public string UOM { get; set; }
    public decimal? Receipt { get; set; }
    public decimal? Issue { get; set; }
    public decimal? MoveOrder { get; set; }
    public decimal? Warehouse { get; set; }
    public decimal SOH { get; set; }
    public string BBD { get; set; }
}
