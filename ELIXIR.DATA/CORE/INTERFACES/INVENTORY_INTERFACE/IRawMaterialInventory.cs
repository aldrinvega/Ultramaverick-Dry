using ELIXIR.DATA.DTOs.INVENTORY_DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.CORE.INTERFACES.INVENTORY_INTERFACE
{
    public interface IRawMaterialInventory
    {

        Task<IReadOnlyList<RawmaterialInventory>> GetAllAvailbleInRawmaterialInventory();

    }
}
