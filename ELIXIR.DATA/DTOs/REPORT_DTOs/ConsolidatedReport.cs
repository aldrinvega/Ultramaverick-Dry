using System;

namespace ELIXIR.DATA.DTOs.REPORT_DTOs
{
    public class ConsolidatedReport
    {
        public int? Id { get; set; }

        public DateTime? TransactionDate { get; set; }

        public string TransactDate { get; set; }

        public string ItemCode { get; set; }

        public string ItemDescription { get; set; }

        public string Category { get; set; }

        public string UOM { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Amount { get; set; }
        public int Source { get; set; }
        public string Reason { get; set; }
        public string Reference { get; set; }
        public string Encoded { get; set; }

        public decimal? Quantity { get; set; }

        public int WarehouseId { get; set; }

        public string TransactionType { get; set; }

        public string CompanyCode { get; set; }

        public string CompanyName { get; set; }

        public string DepartmentCode { get; set; }

        public string DepartmentName { get; set; }

        public string LocationCode { get; set; }

        public string LocationName { get; set; }

        public string AccountTitleCode { get; set; }

        public string AccountTitle { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }
    }
}