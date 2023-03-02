using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY;
using ELIXIR.DATA.DTOs.RECEIVING_DTOs;

namespace ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE
{
    public interface IQCChecklist
    {
        Task<bool> AddChecklists(Checklists input);
        Task<List<ChecklistParent>> GetAllChecklist();
        Task<List<ChecklistParent>> GetAllChecklistbyPOSummaryId(int po_SummaryId);
    }
}