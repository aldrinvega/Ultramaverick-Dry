using ELIXIR.DATA.CORE.INTERFACES.INVENTORY_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.INVENTORY_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.INVENTORY_REPOSITORY
{
    public class MiscellaneousRepository : IMiscellaneous
    {
        private readonly StoreContext _context;

        public MiscellaneousRepository(StoreContext context)
        {
            _context = context;
        }
        public async Task<bool> AddMiscellaneousReceipt(MiscellaneousReceipt receipt)
        {
            await _context.MiscellaneousReceipts.AddAsync(receipt);
            return true;
        }

        public async Task<bool> GenerateReceiptNumber(GenerateMReceipt receipt)
        {
            await _context.GenerateReceiptNos.AddAsync(receipt);
            return true;
        }

        public async Task<bool> AddMiscellaneousIssue(MiscellaneousIssue issue)
        {
            await _context.MiscellaneousIssues.AddAsync(issue);
            return true;
        }

        public async Task<bool> GenerateIssueNumber(GenerateMIssue issue)
        {
            await _context.GenerateIssueNos.AddAsync(issue);
            return true;
        }


    }
}
