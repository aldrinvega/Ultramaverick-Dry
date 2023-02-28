using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY;

namespace ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE
{
    public interface IQCChecklist
    {
        Task<bool> AddChecklists(Checklists input);
    }
}