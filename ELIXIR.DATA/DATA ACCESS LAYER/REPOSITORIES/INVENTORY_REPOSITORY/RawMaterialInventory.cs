using ELIXIR.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.INVENTORY_DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.INVENTORY_REPOSITORY
{
    public class RawMaterialInventory : IRawMaterialInventory
    {
        private readonly StoreContext _context;
        public RawMaterialInventory(StoreContext context)
        {
            _context = context;
        }


        public async Task<IReadOnlyList<RawmaterialInventory>> GetAllAvailbleInRawmaterialInventory()
        {

            return await _context.WarehouseReceived
                                                        .GroupBy(x => new
                                                        {
                                                            x.ItemCode,
                                                            x.ItemDescription,
                                                            x.LotCategory,
                                                            x.Uom,
                                                            x.IsWarehouseReceive,

                                                        })
                                                         .Select(inventory => new RawmaterialInventory
                                                         {
                                                             ItemCode = inventory.Key.ItemCode,
                                                             ItemDescription = inventory.Key.ItemDescription,
                                                             LotCategory = inventory.Key.LotCategory,
                                                             Uom = inventory.Key.Uom,
                                                             SOH = inventory.Sum(x => x.ActualGood),
                                                             ReceiveIn = inventory.Sum(x => x.ActualGood),
                                                             IsWarehouseReceived = inventory.Key.IsWarehouseReceive


                                                         })
                                                          .OrderBy(x => x.ItemCode)
                                                          .Where(x => x.IsWarehouseReceived == true)
                                                          .ToListAsync();

        }
    }
}
