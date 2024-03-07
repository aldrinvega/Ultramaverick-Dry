namespace ELIXIR.DATA.DTOs.ORDERING_DTOs
{
    public class PreparationScheduleDto
    {
        public int Id { get; set; }
        public string OrderDate { get; set; }
        public int OrderNo { get; set; }
        public string DateNeeded { get; set; }
        public string Farm { get; set; }
        public string FarmCode { get; set; }
        public string FarmType { get; set; }
        public string Category { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Uom { get; set; }
        public decimal QuantityOrder { get; set; }
        public bool IsActive { get; set; }
        public decimal StockOnHand { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public bool IsPrepared { get; set; }
    }
}