using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXCEPTIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.RECEIVING_DTOs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY
{
    public class ChecklistRepository : IQCChecklist
    {
        private readonly StoreContext _context;

        public ChecklistRepository(StoreContext context)
        {
            _context = context;
        }


        #region Add Checklist
        public async Task<bool> AddChecklists(Checklists input)
        {
            foreach (var checklistStrings in input.ChecklistsString
                         .Select(compliance => new CheckListString
                         {
                             PO_ReceivingId = input.PO_Receiving.PO_Summary_Id,
                             Checlist_Type = compliance.Checlist_Type,
                             Values = compliance.Values,
                             Remarks = compliance.Remarks
                         }))
                
                await _context.CheckListStrings.AddAsync(checklistStrings);
            
            await _context.SaveChangesAsync();
            
            return true;
        }
        #endregion

        public async Task<IReadOnlyList<ChecklistStringDTO>> GetAllChecklist()
        {
            var checklistStrings = await _context.CheckListStrings.Select(x => new ChecklistStringDTO
            {
                Po_Summary_Id = x.PO_ReceivingId,
                Checklist_Type = x.Checlist_Type,
                Values = JsonConvert.DeserializeObject<List<string>>(x.Value),
                Remarks = x.Remarks
            }).ToListAsync();

            return checklistStrings;
        }
        public async Task<IReadOnlyList<ChecklistStringDTO>> GetChecklistByPoSummaryId(int poSummaryId)
        {
            var checklistStrings = await _context.CheckListStrings
                .Where(x => x.PO_ReceivingId == poSummaryId) 
                .Select(x => new ChecklistStringDTO
                {
                Po_Summary_Id = x.PO_ReceivingId,
                Checklist_Type = x.Checlist_Type,
                Values = JsonConvert.DeserializeObject<List<string>>(x.Value),
                Remarks = x.Remarks
                }).ToListAsync();

            return checklistStrings;
        }

        public async Task<IEnumerable<ForViewingofChecklistResult>> GetPoReceivingInformation(int poSummaryId)
        {
            var poSummary = await _context.QC_Receiving.Where(x => x.PO_Summary_Id == poSummaryId).ToListAsync();
            var checklist = _context.CheckListStrings.Where(x => x.PO_ReceivingId == poSummaryId).Select(x => new ChecklistStringDTO
            {
                Po_Summary_Id = x.PO_ReceivingId,
                Checklist_Type = x.Checlist_Type,
                Values = JsonConvert.DeserializeObject<List<string>>(x.Value)
            });

            var poReceivingInformation = from PO in poSummary
                                         join ck in checklist
                                         on PO.PO_Summary_Id equals ck.Po_Summary_Id
                                         into result
                                         from ck in result.DefaultIfEmpty()

                                         select new ForViewingofChecklistResult
                                         {
                                             PO_Summary_Id = PO.PO_Summary_Id,
                                             Manufacturing_Date = PO.Manufacturing_Date,
                                             Expected_Delivery = PO.Expected_Delivery,
                                             Expiry_Date = PO.Expiry_Date,
                                             Actual_Delivered = PO.Actual_Delivered,
                                             ItemCode = PO.ItemCode,
                                             Batch_No = PO.Batch_No,
                                             TotalReject = PO.TotalReject,
                                             IsActive = PO.IsActive,
                                             CancelDate = PO.CancelDate,
                                             CancelBy = PO.CancelBy,
                                             Reason = PO.Reason,
                                             ExpiryIsApprove = PO.ExpiryIsApprove,
                                             IsNearlyExpire = PO.IsNearlyExpire,
                                             ExpiryApproveBy = PO.ExpiryApproveBy,
                                             ExpiryDateOfApprove = PO.ExpiryDateOfApprove,
                                             QC_ReceiveDate = PO.QC_ReceiveDate,
                                             ConfirmRejectByQc = PO.ConfirmRejectByQc,
                                             IsWareHouseReceive = PO.IsWareHouseReceive,
                                             CancelRemarks = PO.CancelRemarks,
                                             QcBy = PO.QcBy,
                                             MonitoredBy = PO.MonitoredBy,
                                             ChecklistType = ck.Checklist_Type,
                                             Remarks = ck.Remarks,
                                             Values = ck.Values,
                                             ProductType = PO.ProductType
                                         };

            return poReceivingInformation ?? throw new NoResultFound();
        }

    }
}