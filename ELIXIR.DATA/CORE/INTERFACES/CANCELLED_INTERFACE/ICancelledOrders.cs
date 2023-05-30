using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.DATA.CORE.INTERFACES.CANCELLED_INTERFACE
{
    public interface ICancelledOrders
    {
        Task<bool> VoidOrder(CancelledOrders cancelledOrder);
        Task<IEnumerable<CancelledOrders>> GetCancelledOrdersAsync();
        //Task<Customer> GetAllCancelledOrdersByCustomer(int customerId);
        Task<CancelledOrderDTO> GetAllOrderandcancelledOrdersById(int customerId);
        //Task<IEnumerable<Customer>> GetAllOrderandcancelledOrders();


    }
}
