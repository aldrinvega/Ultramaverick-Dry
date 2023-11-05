using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
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
        Task<IReadOnlyList<MRPDto>> GetAllItemForInventory();


        Task<PagedList<MRPDto>> GetAllItemForInventoryPagination(UserParams userParams, string SortColumn,
            string SortOrder);

        Task<PagedList<MRPDto>> GetAllItemForInventoryPaginationOrig(UserParams userParams, string search,
            string SortColumn, string SortOrder);


        //MRP
        Task<PoSummaryInventory> GetPOSummary();

        Task<IReadOnlyList<MRPDto>> GetSample(string itemCode);
    }
}