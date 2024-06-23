namespace ELIXIR.DATA.DTOs.REPORT_DTOs
{
    public class InventoryMovementReport
    {

        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string ItemCategory { get; set; }
        public decimal TotalMoveOrderedOut { get; set; }
        public decimal TotalMiscIssue { get; set; }
        public decimal TotalMicReceipt { get; set; }
        public decimal TotalReceived { get; set; }
        public decimal Ending { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal PurchasedOrder { get; set; }
        public decimal OthersPlus { get; set; }
        public decimal Receipt { get; set; }

    }
}
