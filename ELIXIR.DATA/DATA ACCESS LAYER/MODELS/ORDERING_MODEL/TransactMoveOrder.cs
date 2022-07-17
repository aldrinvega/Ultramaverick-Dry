using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
   public class TransactMoveOrder : BaseEntity
    {

        public int OrderNo { get; set; }
        public string FarmType { get; set; }
        public string FarmCode { get; set; }
        public string FarmName { get; set; }

    //    public string ItemCode { get; set; }
    //    public string ItemDescription { get; set; }
   //     public string Uom { get; set; }

     //   [Column(TypeName = "decimal(18,2)")]
  //      public decimal QuantityOrdered { get; set; }
     //   public string Category { get; set; }

   //     [Column(TypeName = "Date")]
    //    public DateTime OrderDate { get; set; }

    //    [Column(TypeName = "Date")]
  //      public DateTime DateNeeded { get; set; }

   //     public int WarehouseId { get; set; }
        public bool IsActive { get; set; }
        public bool? IsApprove { get; set; }

        public string PreparedBy { get; set; }
        public DateTime? PreparedDate { get; set; }

        public int OrderNoPKey { get; set; }
     //   public DateTime? ExpirationDate { get; set; }

   //     public string PlateNumber { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public bool? IsTransact { get; set; }


    }
}
