using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.INTERFACES.LABTEST_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.LABORATORYTEST_DTO;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.LABTEST_REPOSITORY
{
    public class LabTestRepository : ILabTestInterface
    {
        private readonly StoreContext _context;

        public LabTestRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NearlyExpiryitemsDTO>> GetAllNearlyExpiryItems()
        {
            var today = DateTime.Now.Date;
            var targetDate = today.AddDays(10);
            var items = await _context.WarehouseReceived
                .Where(x => x.Expiration <= targetDate)
                .Select(items => new NearlyExpiryitemsDTO
                {
                    Id = items.Id,
                    ItemCode = items.ItemCode,
                    ItemDescription = items.ItemDescription,
                    ExpirationDate = items.Expiration.ToString(),
                    WarehouseId = items.Id,
                }).ToListAsync();

            return items;
        }

        // public async Task<IEnumerable<RequestedItemsForLabtest>> GetAllRequestedItemsForLabTest()
        // {
        //     
        // }
        // public async Task<bool> UpdateLabTestStatus(){}
        // public async Task<bool> ReceiveItemForLabtest(){}
    }
}