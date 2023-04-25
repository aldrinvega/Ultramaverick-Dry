using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
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
            foreach (var compliance in input.ChecklistsString)
            {
                CheckListString checklistStrings = new CheckListString
                {
                    PO_ReceivingId = input.PO_Receiving.PO_Summary_Id,
                    Checlist_Type = compliance.Checlist_Type,
                    Values = compliance.Values
                };

                await _context.CheckListStrings.AddAsync(checklistStrings);

            }
            await _context.SaveChangesAsync();
            
            return true;
        }
        #endregion

        //public async Task<List<ChecklistParent>> GetAllChecklist()
        //{
        //    var checklistStrings = await _context.CheckListStrings.ToListAsync();
        //    var checklistInputs = await _context.CheckListInput.ToListAsync();
        //    var checklistComplaints = await _context.ChecklistForCompliant.ToListAsync();

        //    var checklists = from cls in checklistStrings
        //        join cli in checklistInputs on cls.PO_ReceivingId equals cli.PO_ReceivingId into cliGroup
        //        join clc in checklistComplaints on cls.PO_ReceivingId equals clc.PO_ReceivingId into clcGroup
        //        from cli in cliGroup.DefaultIfEmpty()
        //        from clc in clcGroup.DefaultIfEmpty()
        //        group new { cls, cli, clc } by cls.PO_ReceivingId into g
        //        select new ChecklistParent
        //        {
        //            PO_Summary_Id = g.Key,
        //            ChecklistString = g.Select(x => new ChecklistStringDTO
        //            {
        //                Checklist_Type = x.cls.Checlist_Type,
        //                Values = JsonConvert.DeserializeObject<List<string>>(x.cls.Value)
        //            }).ToList(),
        //            // ChecklistInput = g.Select(x => new CheclistInputDTO
        //            // {
        //            //     Checklist_Type = x.cli?.Checlist_Type,
        //            //     Parameter = x.cli?.Parameter,
        //            //     Value = x.cli?.Value
        //            // }).Where(x => x != null).ToList(),
        //            // ChecklistCompliants = g.Select(x => new ChecklistCompliantsDTO
        //            // {
        //            //     Checklist_Type = x.clc?.Checklist_Type,
        //            //     Values = x.clc?.Value,
        //            //     IsCompliant = x.clc.IsCompliant
        //            // }).Where(x => x != null).ToList()
        //        };

        //    return checklists.ToList();
        //}
        //
        //
        //public async Task<List<ChecklistParent>> GetAllChecklistbyPOSummaryId(int po_SummaryId)
        //{
        //    var query =
        //        from cfc in _context.ChecklistForCompliant
        //        join ci in _context.CheckListInput on cfc.PO_ReceivingId equals ci.PO_ReceivingId into ciGroup
        //        from ci in ciGroup.DefaultIfEmpty()
        //        join cs in _context.CheckListStrings on cfc.PO_ReceivingId equals cs.PO_ReceivingId into csGroup
        //        from cs in csGroup.DefaultIfEmpty()
        //        where cfc.PO_ReceivingId == po_SummaryId && ci != null && cs != null
        //        group new { cfc, ci, cs } by cfc.PO_ReceivingId into g
        //        select new ChecklistParent
        //        {
        //            PO_Summary_Id = g.Key,
        //            ChecklistInput = g.Select(x => x.ci)
        //                .Where(ci => ci != null)
        //                .Select(ci => new CheclistInputDTO
        //                {
        //                    Checklist_Type = ci.Checlist_Type,
        //                    Parameter = ci.Parameter,
        //                    Value = ci.Value
        //                }).ToList(),
        //            ChecklistString = g.Where(x => x.cs != null)
        //                .GroupBy(x => x.cs.Checlist_Type)
        //                .Select(gcs => new ChecklistStringDTO
        //                {
        //                    Checklist_Type = gcs.Key,
        //                    Values = gcs.Where(x => x.cs.Value != null)
        //                        .Select(x => x.cs.Value)
        //                        .ToList()
        //                })
        //                .ToList(),
        //            ChecklistCompliants = g.Select(x => new ChecklistCompliantsDTO
        //            {
        //                Checklist_Type = x.cfc.Checklist_Type,
        //                Value = x.cfc.Value
        //            }).ToList()
        //        };

        //    return await query.ToListAsync();
        //}


    }
}