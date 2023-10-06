using System.Collections.Generic;

namespace ELIXIR.DATA.DTOs.ORDERING_DTOs
{
    public class MultiplePrintingDTO
    {
        public int OrderNo { get; set; }
        public IEnumerable<Order> Orders { get; set; }

        public class Order
        {
            public string FarmCode { get; set; }
            public string FarmName { get; set; }
            public string FarmType { get; set; }
            public string Category { get; set; }
            public string Uom { get; set; }
            public string OrderDate { get; set; }
            public string DateNeeded { get; set; }
            public string PreparedDate { get; set; }
            public int BarcodeNo { get; set; }
            public string ItemCode { get; set; }
            public string ItemDescription { get; set; }
            public decimal Quantity { get; set; }
            public string Expiration { get; set; }
            public bool IsActive { get; set; }
            public string PlateNumber { get; set; }
            public bool IsReject { get; set; }
            public bool IsApprove { get; set; }
            public bool IsPrepared { get; set; }
            public string ApprovedDate { get; set; }
            public string RejectedDate { get; set; }
            public string Remarks { get; set; }
            public int OrderNoPKey { get; set; }
            public bool IsPrint { get; set; }
            public bool IsTransact { get; set; }
            public string DeliveryStatus { get; set; }
            public string PreparedBy { get; set; }

            public string CheckedBy { get; set; }

            public decimal UnitPrice { get; set; }
        }
    }
}