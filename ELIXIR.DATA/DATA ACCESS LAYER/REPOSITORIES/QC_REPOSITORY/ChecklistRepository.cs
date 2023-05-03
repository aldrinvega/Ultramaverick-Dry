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

        public async Task<ForViewingofChecklistResult> GetPoReceivingInformation(int poSummaryId)
        {
            var poSummary = await _context.QC_Receiving
                .Where(x => x.PO_Summary_Id == poSummaryId)
                .FirstOrDefaultAsync();

            if (poSummary == null)
            {
                throw new NoResultFound();
            }

            var checklists = _context.CheckListStrings
                .Where(x => x.PO_ReceivingId == poSummaryId)
                .ToList();

            var checklistGroups = checklists
                .GroupBy(x => x.PO_ReceivingId)
                .Select(g => new ChecklistGroups
                {
                    Checklists = g.Select(c => new ChecklistStringDTO
                    {
                        Checklist_Type = c.Checlist_Type,
                        Values = JsonConvert.DeserializeObject<List<string>>(c.Value),
                        Remarks = c.Remarks
                    }).ToList()
                });

            var ckk = _context.CheckListStrings
            .Where(c => c.PO_ReceivingId == poSummaryId)
            .Select(c => new ChecklistStringDTO
            {
                Checklist_Type = c.Checlist_Type,
                Values = JsonConvert.DeserializeObject<List<string>>(c.Value),
                Remarks = c.Remarks
            })
            .ToList();

            var result = new ForViewingofChecklistResult
            {
                PO_Summary_Id = poSummary.PO_Summary_Id,
                Manufacturing_Date = poSummary.Manufacturing_Date,
                Expected_Delivery = poSummary.Expected_Delivery,
                Expiry_Date = poSummary.Expiry_Date,
                Actual_Delivered = poSummary.Actual_Delivered,
                ItemCode = poSummary.ItemCode,
                Batch_No = poSummary.Batch_No,
                TotalReject = poSummary.TotalReject,
                IsActive = poSummary.IsActive,
                CancelDate = poSummary.CancelDate,
                CancelBy = poSummary.CancelBy,
                Reason = poSummary.Reason,
                ExpiryIsApprove = poSummary.ExpiryIsApprove,
                IsNearlyExpire = poSummary.IsNearlyExpire,
                ExpiryApproveBy = poSummary.ExpiryApproveBy,
                ExpiryDateOfApprove = poSummary.ExpiryDateOfApprove,
                QC_ReceiveDate = poSummary.QC_ReceiveDate,
                ConfirmRejectByQc = poSummary.ConfirmRejectByQc,
                IsWareHouseReceive = poSummary.IsWareHouseReceive,
                CancelRemarks = poSummary.CancelRemarks,
                QcBy = poSummary.QcBy,
                MonitoredBy = poSummary.MonitoredBy,
                Checklists = ckk
            };

            return result;
        }


    }
}