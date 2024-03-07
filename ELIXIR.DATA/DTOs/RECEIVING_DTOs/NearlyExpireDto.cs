using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.RECEIVING_DTOs
{
    public class NearlyExpireDto
    {
        public int Id { get; set; }
        public int PO_Number { get; set; }
        public string PO_Date { get; set; }
        public int PR_Number { get; set; }
        public string PR_Date { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Supplier { get; set; }
        public string Uom { get; set; }
        public decimal QuantityOrdered { get; set; }
        public decimal ActualGood { get; set; }
        public decimal ActualRemaining { get; set; }
        public string ManufacturingDate { get; set; }
        public string? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public int Days { get; set; }
        public bool? IsNearlyExpire { get; set; }
        public bool? ExpiryIsApprove { get; set; }
        public int ReceivingId { get; set; }
        public string DateOfChecking { get; set; }
        public decimal TotalReject { get; set; }
        public decimal ExpectedDelivery { get; set; }
        public string MonitoredBy { get; set; }
        public string QcBy { get; set; }
    }
}
