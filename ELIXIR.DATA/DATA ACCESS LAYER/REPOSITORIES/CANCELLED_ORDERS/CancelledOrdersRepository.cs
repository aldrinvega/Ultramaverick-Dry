using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.INTERFACES.CANCELLED_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.CANCELLED_ORDERS
{
    public class CancelledOrdersRepository : ICancelledOrders
    {
        private readonly StoreContext _context;

        public CancelledOrdersRepository( StoreContext context)
        {
            _context = context;
        }
        public async Task<bool> VoidOrder(CancelledOrders cancelledOrder)
        {
            var existing = await _context.Orders.Where(x => x.Id == cancelledOrder.OrderNo)
                .Where(x => x.IsActive == true)
                .FirstOrDefaultAsync();

            if (existing == null)
                return false;
            existing.IsCancelledOrder = true;

            await _context.CancelledOrders.AddAsync(cancelledOrder);
            return true;
        }
        public async Task<IEnumerable<CancelledOrders>> GetCancelledOrdersAsync()
        {
        
            return await _context.CancelledOrders.Include(x => x.Customers).ToListAsync();
        }
    }
}