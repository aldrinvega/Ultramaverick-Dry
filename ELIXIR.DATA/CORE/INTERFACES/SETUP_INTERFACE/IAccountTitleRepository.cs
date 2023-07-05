using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;

namespace ELIXIR.DATA.CORE.INTERFACES.SETUP_INTERFACE
{
    public interface IAccountTitleRepository
    {
        Task<bool> AddNewAccountTitle(IEnumerable<AccountTitle> accountTitle);
        Task<bool> UpdateAccountTitle(AccountTitle accountTitle);
        Task<IEnumerable<AccountTitle>> GetAllAccountTitleAsync();
        Task<PagedList<AccountTitle>> GetAllAccountTitleAsyncPagination(bool status, UserParams userParams);
    }
}