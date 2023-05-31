using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.WAREHOUSE_DTOs;

namespace ELIXIR.DATA.DTOs.ORDERING_DTOs
{
    public class GetAllAllOrdersDTO
    {
        public WarehouseReceived WarehouseReceived
        {
            get;
            set;
        }
        public Ordering OrderingReserve
        {
            get;
            set;
        }
        public MiscellaneousIssueDetails IssueOut
        {
            get;
            set;
        }
        public Customer Customer
        {
            get;
            set;
        }

    }
}
