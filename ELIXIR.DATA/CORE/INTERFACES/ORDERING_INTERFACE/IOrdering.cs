using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.ORDERING_INTERFACE
{
    public interface IOrdering
    {

        Task<IReadOnlyList<OrderDto>> GetAllListofOrders(string farms);

        Task<bool> EditQuantityOrder(Ordering orders);
        Task<bool> SchedulePreparedDate(Ordering orders);

        Task<IReadOnlyList<OrderDto>> GetAllListOfPreparedDate();

        Task<bool> ApprovePreparedDate(Ordering orders);
        Task<bool> RejectPreparedDate(Ordering orders);

        Task<bool> AddNewOrders(Ordering orders);

        Task<IReadOnlyList<OrderDto>> OrderSummary(string DateFrom, string DateTo);

        Task<bool> ValidateFarmType(Ordering orders);

        Task<bool> ValidateFarmCode(Ordering orders);
        Task<bool> ValidateRawMaterial(Ordering orders);
        Task<bool> ValidateUom(Ordering orders);

        Task<bool> ValidateExistingOrders(Ordering orders);


        Task<PagedList<OrderDto>> GetAllListofOrdersPagination(UserParams userParams);

        Task<bool> ValidateOrderAndDateNeeded(Ordering orders);


        Task<bool> CancelOrders(Ordering orders);

        Task<IReadOnlyList<OrderDto>> GetAllListOfCancelledOrders();

        Task<bool> ReturnCancellOrdersInList(Ordering orders);


        Task<PagedList<OrderDto>> GetAllListForMoveOrderPagination(UserParams userParams);


        Task<IReadOnlyList<OrderDto>> GetAllListOfApprovedPreparedDate(string farm);

    }
}
