using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.INTERFACES.ORDER_HUB;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using Microsoft.AspNetCore.SignalR;

namespace ELIXIR.DATA.SERVICES
{
    public class OrderHub : Hub<IOrderHub>
    {
        public async Task SetBeingPrepared(List<Ordering> moveList)
        {
            await Clients.Others.SetBeingPrepared(moveList);
        }
      
    }
} 