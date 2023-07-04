using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.SETUP_REPOSITORY
{
    public class AccountTitleRepository : IAccountTitleRepository
    {
        private readonly StoreContext _context;

        public AccountTitleRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<bool> AddNewAccountTitle(IEnumerable<AccountTitle> accountTitle)
        {
            foreach (var ac in accountTitle)
            {
                var result = await _context.AccountTitles.Upsert(ac)
                    .On(c => c.AccountTitleName)
                    .WhenMatched(c => new AccountTitle()
                    {
                        AccountTitleId = ac.AccountTitleId,
                        AccountTitleName = ac.AccountTitleName,
                        AccountTitleCode = ac.AccountTitleCode,
                        IsActive = false
                    }).RunAsync();
            }

            return true;
        }

        public async Task<bool> UpdateAccountTitle(AccountTitle accountTitle)
        {
            var validateAccountTitle = await _context.AccountTitles
                .FirstOrDefaultAsync(x => x.AccountTitleId == accountTitle.AccountTitleId);

            if (validateAccountTitle == null)
            {
                throw new Exception("Account Title Not Found");
            }
            
            validateAccountTitle.IsActive = accountTitle.IsActive;
            validateAccountTitle.UpdatedAt = DateTime.Now;
            
            await _context.SaveChangesAsync();
            return true;
        }
    }
}