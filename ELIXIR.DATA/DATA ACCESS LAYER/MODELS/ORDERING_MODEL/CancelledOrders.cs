using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
    public class CancelledOrders : BaseEntity
    {
        public int OrderNo
        {
            get;
            set;
        }
        
        public Customer Customers { get; set; }
        
        [Column(TypeName = "Date")]
        public DateTime OrderDate
        {
            get;
            set;
        }
        [Column(TypeName = "Date")]
        public DateTime DateNeeded
        {
            get;
            set;
        }
        public string ItemCode
        {
            get;
            set;
        }
        public string ItemDescription
        {
            get;
            set;
        }
        public string Uom
        {
            get;
            set;
        }
        public int QuantityOrdered
        {
        
         get;
         set;
        }
        public string Category
        {
            get;
            set;
        }
    }
}