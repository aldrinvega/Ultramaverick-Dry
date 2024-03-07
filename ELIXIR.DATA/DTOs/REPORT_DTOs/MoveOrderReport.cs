namespace ELIXIR.DATA.DTOs.REPORT_DTOs
{
    public class MoveOrderReport
    {
        public int? MoveOrderId { get; set; }
        public int? OrderNo { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string FarmCode { get; set; }
        public string FarmName { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public string Category { get; set; }
        public decimal? Quantity { get; set; }
        public string ExpirationDate { get; set; }
        public string TransactionType { get; set; }
        public string MoveOrderBy { get; set; }
        public string MoveOrderDate { get; set; }
        public string TransactedDate { get; set; }
        public string TransactedBy { get; set; }
        public string DeliveryDate { get; set; }
        public string PreparedDate { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal WeightedAverageUnitCost { get; set; }
        public string DepartmentName { get; set; }
        public int WarehouseId { get; set; }
        public string Location { get; set; }
        public string AccountTitle { get; set; }
        public string DeliveryStatus { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string Status { get; set; }
    }
}