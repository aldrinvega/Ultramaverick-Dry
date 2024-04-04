using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL
{
    public class MiscellaneousIssue : BaseEntity
    {
        public string Customer { get; set; }

        public string CustomerCode { get; set; }

        [Column(TypeName = "decimal(18,2)")] public decimal TotalQuantity { get; set; }

        public DateTime PreparedDate { get; set; }

        public string PreparedBy { get; set; }

        public string Details { get; set; }

        public string Remarks { get; set; }

        public bool IsActive { get; set; }

        public bool? IsTransact { get; set; }
        public string AccountTitleCode { get; set; }
        public string AccountTitles { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string AddedBy { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Reason { get; set; }
    }
}