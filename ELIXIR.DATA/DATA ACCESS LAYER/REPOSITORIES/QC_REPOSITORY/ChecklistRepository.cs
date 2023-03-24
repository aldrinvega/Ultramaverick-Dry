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


        #region Add Checklist Inputs
        public async Task<bool> AddChecklists(Checklists input)
        {
            
            // var checklistForCompliance = new ChecklistForCompliants();
            // var checklistInput = new CheckListInputs();
            // var checklistString = new CheckListString();

            foreach (var compliance in input.ChecklistForCompliants)
            {
                ChecklistForCompliants checklistForCompliants = new ChecklistForCompliants
                {
                    PO_ReceivingId = input.PO_Receiving.PO_Summary_Id,
                    Checklist_Type = compliance.Checklist_Type,
                    Value = compliance.Value
                };
                switch (compliance.Value)
                {
                    case "Certificate of Analysis":
                    case "Certificate of Product Registration":
                    case "Food Grade Certificate":
                    case "Meat Inspection Certificate":
                    case "Purchase Order":
                    case "Material Safety Data Sheet":
                    case "Migration Test":
                    case "Veterinary Health Certificate":
                    case "Shipping Permit":
                    case "No rust, torn/detached parts, etc":
                    case "Delivered in freezer/refeer van":
                    case "Properly packed in clean plastic packaging materials/ containers/crates/ sack/boxes etc":
                    case "Durable / elastic (if plastic/ packaging material":
                    case "No holes and/or tears":
                    case "With clear, correct and readable product information and label":
                    case "No spillages / leaks/wet portions":
                    case "Stored/ delivered in clean and in good conditioned container (crates and/or pallets)":
                    case "Each product type is segregated to avoid cross contamination":
                    case "Absence of unnecessary things/ products inside the delivery truck that may contaminate the products":
                    case "No dirt, food debris, pest and signs of pest, etc.":
                    case "No off odor, detached/ disintegrated parts.":
                    case "No rust, retained dirt, food debris or any sign of pest/pest infestation":
                    case "Cooling system is in good working condition and without leaks (if ref/ freezer van)":
                    case "Plastic curtains are available, complete and in good condition":
                    case "Clean and trimmed fingernails. No nail polish and false nails":
                    case "Proper  and short haircut":
                    case "Delivery personnel is apparently healthy":
                    case "Cleanly shaven face":
                    case "Absence of loose items":
                    case "FIT FOR HUMAN CONSUMPTION":
                    case "NOT FIT FOR HUMAN CONSUMPTION":
                    case "ACCEPT":
                        await _context.ChecklistForCompliant.AddAsync(checklistForCompliants);
                        break;
                }
               
            }
            
            foreach (var checklistStrings in input.ChecklistsString)
            {
                CheckListString checklistString = new CheckListString
                {
                    PO_ReceivingId = input.PO_Receiving.PO_Summary_Id,
                    Checlist_Type = checklistStrings.Checlist_Type,
                    Value = checklistStrings.Value
                };
    
                switch (checklistStrings.Checlist_Type)
                {
                    case "Color":
                    case "Odor":
                    case "Appearance":
                    case "Texture":
                    case "Absence Of Contaminants":
                    case "Product Condition":
                    case "Product / Commodity Type":
                        await _context.CheckListStrings.AddAsync(checklistString);
                        break;
                }
            }

            foreach (var checklistInputs in input.CheckListInput)
            {
                CheckListInputs checklistString = new CheckListInputs
                {
                    PO_ReceivingId = input.PO_Receiving.PO_Summary_Id,
                    Checlist_Type = checklistInputs.Checlist_Type,
                    Value = checklistInputs.Value
                };
                
                switch (checklistInputs.Parameter)
                {
                    case "Width":
                    case "HEIGHT":
                    case "LENGTH":
                    case "THICKNESS":
                    case "DIAMETER":
                    case "RADIUS":
                    case "INTERNAL / SURFACE TEMPERATURE (if cold products)":
                    case "Delivery vehicle temperature (if product is delivered using freezer/reefer van)":
                    case "Delivery vehicle's plate number":
                    case "Name of delivery personnel":
                    case "QUANTITY REJECT":
                    case "QUANTITY ACCEPT":
                        await _context.CheckListInput.AddAsync(checklistString);
                        break;
                }
            }
            await _context.QC_Receiving.AddAsync(input.PO_Receiving);
            
            return true;
        }
        #endregion
        
        
        public async Task<List<ChecklistParent>> GetAllChecklist()
        {
            var checklistStrings = await _context.CheckListStrings.ToListAsync();
            var checklistInputs = await _context.CheckListInput.ToListAsync();
            var checklistComplaints = await _context.ChecklistForCompliant.ToListAsync();
        
            var checklists = from cls in checklistStrings
                join cli in checklistInputs on cls.PO_ReceivingId equals cli.PO_ReceivingId into cliGroup
                join clc in checklistComplaints on cls.PO_ReceivingId equals clc.PO_ReceivingId into clcGroup
                from cli in cliGroup.DefaultIfEmpty()
                from clc in clcGroup.DefaultIfEmpty()
                group new { cls, cli, clc } by cls.PO_ReceivingId into g
                select new ChecklistParent
                {
                    PO_Summary_Id = g.Key,
                    ChecklistString = g.Select(x => new ChecklistStringDTO
                    {
                        Checklist_Type = x.cls.Checlist_Type,
                        Values = JsonConvert.DeserializeObject<List<string>>(x.cls.Value)
                    }).ToList(),
                    // ChecklistInput = g.Select(x => new CheclistInputDTO
                    // {
                    //     Checklist_Type = x.cli?.Checlist_Type,
                    //     Parameter = x.cli?.Parameter,
                    //     Value = x.cli?.Value
                    // }).Where(x => x != null).ToList(),
                    // ChecklistCompliants = g.Select(x => new ChecklistCompliantsDTO
                    // {
                    //     Checklist_Type = x.clc?.Checklist_Type,
                    //     Values = x.clc?.Value,
                    //     IsCompliant = x.clc.IsCompliant
                    // }).Where(x => x != null).ToList()
                };
        
            return checklists.ToList();
        }
        //
        //
        // public async Task<List<ChecklistParent>> GetAllChecklistbyPOSummaryId(int po_SummaryId)
        // {
        //     var checkListStrings = await _context.CheckListStrings
        //         .Where(x => x.PO_ReceivingId == po_SummaryId)
        //         .ToListAsync();
        //
        //     var checklists = new List<ChecklistParent>();
        //
        //     foreach (var checkListString in checkListStrings)
        //     {
        //         var checklistParent = checklists.FirstOrDefault(c => c.PO_Summary_Id == checkListString.PO_ReceivingId);
        //
        //         if (checklistParent == null)
        //         {
        //             checklistParent = new ChecklistParent
        //             {
        //                 PO_Summary_Id = checkListString.PO_ReceivingId,
        //                 ChecklistString = new List<ChecklistStringDTO>()
        //             };
        //             checklists.Add(checklistParent);
        //         }
        //         
        //         var values = JsonConvert.DeserializeObject<List<string>>(checkListString.Value);
        //             var checklistDTO = new ChecklistStringDTO()
        //             {
        //                 Checklist_Type = checkListString.Checlist_Type,
        //                 Values = values
        //             };
        //             checklistParent.ChecklistString.Add(checklistDTO);
        //     }
        //     return checklists;
        // }
    }
}