using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL
{
        public class CancelledOrders : BaseEntity
        {
            public int OrderId
            {
                get;
                set;
            }
            public Ordering Order
            {
                get;
                set;
            }
            public DateTime CancellationDate
            {
                get; set;
            }
            public string Reason
            {
                get;
                set;
            }
            public int CustomerId
            {
        
                       get;
                       set;
            }
        }
    
}
