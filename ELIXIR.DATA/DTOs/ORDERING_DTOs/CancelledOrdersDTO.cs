using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;

namespace ELIXIR.DATA.DTOs.ORDERING_DTOs
{
    public class CancelledOrderDTO
    {
        public int CustomerId
        {
            get; set;
        }
        public string CustomerCode
        {
            get; set;
        }
        public string CustomerName
        {
            get; set;
        }
        public int FarmTypeId
        {
            get; set;
        }
        public string CompanyName
        {
            get; set;
        }
        public string CompanyCode
        {
            get; set;
        }
        public string MobileNumber
        {
            get; set;
        }
        public string LeadMan
        {
            get; set;
        }
        public string Address
        {
            get; set;
        }

        public string LocationName
        {
            get; set;
        }
        public string DepartmentName
        {
            get; set;
        }
        public string AccountTitle
        {
            get; set;
        }
        public List<OrdersforCancelledPaginationDTO> CancelledOrders
        {
            get; set;
        }
        //public List<Ordering> Orders
        //{
        //    get; set;
        //} = new List<Ordering>();
    }
}
