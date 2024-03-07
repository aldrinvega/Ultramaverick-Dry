using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;

namespace ELIXIR.DATA.CORE.INTERFACES.ORDER_HUB
{
    public interface IOrderHub
    {
        Task SetBeingPrepared(Ordering moveList);
    }
}