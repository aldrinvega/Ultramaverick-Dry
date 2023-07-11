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

        public string CustomerCode
        {
            get;
            set;
        }
        public string CustomerName
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public string CompanyName
        {
            get;
            set;
        }

        public string DepartmentName
        {
            get;
            set;
        }

        public string LocationName
        {
            get;
            set;
        }
        
        public string FarmType
        {
            get;
            set;
        }

        public string PreparedDate
        {
            get;
            set;
        }
    }
}
