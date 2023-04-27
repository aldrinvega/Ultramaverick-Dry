using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXCEPTIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;
using ELIXIR.DATA.DTOs.RECEIVING_DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
            foreach (var checklistStrings in input.ChecklistsString.Select(compliance => new CheckListString
                     {
                         PO_ReceivingId = input.PO_Receiving.PO_Summary_Id,
                         Checlist_Type = compliance.Checlist_Type,
                         Values = compliance.Values,
                         Remarks = compliance.Remarks
                     }))
            {
                await _context.CheckListStrings.AddAsync(checklistStrings);
            }
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
                Values = JsonConvert.DeserializeObject<List<string>>(x.Value)
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
                Values = JsonConvert.DeserializeObject<List<string>>(x.Value)
                }).ToListAsync();

            return checklistStrings;
        }

        public async Task<IEnumerable<ForViewingofChecklistResult>> GetPoReceivingInformation(int poSummaryId)
        {
            var poReceivingInformation = await _context.QC_Receiving.Where(x => x.PO_Summary_Id == poSummaryId)
                .SelectMany(
                    x => x.Checklist, (r, c) =>
                        new ForViewingofChecklistResult
                        {
                            PO_Summary_Id = r.PO_Summary_Id,
                            Manufacturing_Date = r.Manufacturing_Date,
                            Expected_Delivery = r.Expected_Delivery,
                            Expiry_Date = r.Expiry_Date,
                            Actual_Delivered = r.Actual_Delivered,
                            ItemCode = r.ItemCode,
                            Batch_No = r.Batch_No,
                            TotalReject = r.TotalReject,
                            IsActive = r.IsActive,
                            CancelDate = r.CancelDate,
                            CancelBy = r.CancelBy,
                            Reason = r.Reason,
                            ExpiryIsApprove = r.ExpiryIsApprove,
                            IsNearlyExpire = r.IsNearlyExpire,
                            ExpiryApproveBy = r.ExpiryApproveBy,
                            ExpiryDateOfApprove = r.ExpiryDateOfApprove,
                            QC_ReceiveDate = r.QC_ReceiveDate,
                            ConfirmRejectByQc = r.ConfirmRejectByQc,
                            IsWareHouseReceive = r.IsWareHouseReceive,
                            CancelRemarks = r.CancelRemarks,
                            QcBy = r.QcBy,
                            // MonitoredBy = r.MonitoredBy,
                            ChecklistType = c.Checlist_Type,
                            Values = JsonConvert.DeserializeObject<List<string>>(c.Value)
                        }).ToListAsync();

            if (poReceivingInformation.Count == 0)
                throw new NoResultFound();
            return poReceivingInformation;
        }
    
    }
}