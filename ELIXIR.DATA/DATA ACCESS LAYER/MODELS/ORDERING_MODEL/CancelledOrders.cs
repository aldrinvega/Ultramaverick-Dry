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
        public DateTime CancellationDate
        {
            get;
            set;
        }
        public string Reason
        {
            get;
            set;
        }
        
        public Ordering Orders
        {
            get;
            set;
        }
        public int OrdersId
        {

            get;
            set;
        }


    }
}