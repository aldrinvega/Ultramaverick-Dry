using System.Collections.Generic;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY;
using ELIXIR.DATA.DTOs.RECEIVING_DTOs;

namespace ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE
{
    public interface IQCChecklist
    {
        /*ask<bool> AddChecklists(Checklists input);*/
        Task<IReadOnlyList<ChecklistStringDTO>> GetAllChecklist();
        Task<IReadOnlyList<ChecklistStringDTO>> GetChecklistByPoSummaryId(int poSummaryId);
        Task<ForViewingofChecklistResult> GetPoReceivingInformation(int poSummaryId);
        Task<bool> UpdateReceivingId(int receivingId);
    }
}