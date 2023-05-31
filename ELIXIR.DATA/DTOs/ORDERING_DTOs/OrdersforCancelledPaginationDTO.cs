using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.ORDERING_DTOs
{
    public class OrdersforCancelledPaginationDTO
    {
        public int Id
        {
            get;
            set;
        }
        public int OrderId
        {
            get;
            set;
        }
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
        public string OrderDate
        {
            get;
            set;
        }
        public string DateNeeded
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
        public string UOM
        {
        
                   get;
                   set;
        }
        public decimal QuantityOrdered
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
