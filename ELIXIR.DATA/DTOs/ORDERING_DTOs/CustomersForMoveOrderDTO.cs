using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.ORDERING_DTOs
{
    public class CustomersForMoveOrderDTO
    {
        public string Farm
        {
            get;
            set;
        }
        public bool IsActive
        {
            get;
            set;
        }
        public bool IsApproved
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
