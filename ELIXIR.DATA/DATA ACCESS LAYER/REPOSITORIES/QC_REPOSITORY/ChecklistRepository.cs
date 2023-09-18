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
        
        /*#region Add Checklist
        /*public async Task<bool> AddChecklists(Checklists input)
        {
            foreach (var newChecklistString in input.ChecklistsString.Select(checklistString => new CheckListString
                     {
                         ReceivingId = checklistString.PO_Summary_Id,
                         Checlist_Type = checklistString.Checlist_Type,
                         Value = checklistString.Value,
                         Remarks = checklistString.Remarks
                     }))
            {
                await _context.CheckListStrings.AddAsync(newChecklistString);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion#1#*/
        public async Task<bool> UpdateReceivingId(int receivingId)
        {
            var checklistString = await _context.CheckListStrings.Where(x => x.ReceivingId == null).ToListAsync();

           foreach(var ck in checklistString)
            {
                ck.ReceivingId = receivingId;
            }

            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<IReadOnlyList<ChecklistStringDTO>> GetAllChecklist()
        {
            var checklistStrings = await _context.CheckListStrings.Select(x => new ChecklistStringDTO
            {
                ReceivingId = x.ReceivingId,
                Checklist_Type = x.Checlist_Type,
                Values = x.Value,
                Remarks = x.Remarks
            }).ToListAsync();
        
            return checklistStrings;
        }
        public async Task<IReadOnlyList<ChecklistStringDTO>> GetChecklistByPoSummaryId(int receivingId)
        {
            var checklistStrings = await _context.CheckListStrings
                .Where(x => x.ReceivingId == receivingId) 
                .Select(x => new ChecklistStringDTO
                {
                ReceivingId = x.ReceivingId,
                Checklist_Type = x.Checlist_Type,
                Values = x.Value,
                Remarks = x.Remarks
                }).ToListAsync();
        
            return checklistStrings;
        }
        
        public async Task<ForViewingofChecklistResult> GetPoReceivingInformation(int receivingId)
        {
            var poSummary = await _context.QC_Receiving
                .Where(x => x.Id == receivingId)
                .FirstOrDefaultAsync();
        
            if (poSummary == null)
            {
                throw new NoResultFound();
            }
        
            var checklists = _context.CheckListStrings
                .Where(x => x.ReceivingId == receivingId)
                .ToList();
        
            var checklistGroups = checklists
                .GroupBy(x => x.ReceivingId)
                .Select(g => new ChecklistGroups
                {
                    Checklists = g.Select(c => new ChecklistStringDTO
                    {
                        Checklist_Type = c.Checlist_Type,
                        Values = c.Value,
                        Remarks = c.Remarks
                    }).ToList()
                });
        
            var ckk = _context.CheckListStrings
            .Where(c => c.ReceivingId == receivingId)
            .Select(c => new ChecklistStringDTO
            {
                Checklist_Type = c.Checlist_Type,
                Values = c.Value,
                Remarks = c.Remarks
            }).ToList();

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