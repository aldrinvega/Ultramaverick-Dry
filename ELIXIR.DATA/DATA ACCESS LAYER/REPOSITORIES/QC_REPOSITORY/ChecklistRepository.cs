using System.Threading.Tasks;
using ELIXIR.DATA.CORE.INTERFACES.QC_INTERFACE;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY
{
    public class ChecklistRepository : IQCChecklist
    {
        private readonly StoreContext _context;

        public ChecklistRepository(StoreContext context)
        {
            _context = context;
        }
        
        public async Task<bool> AddChecklists(Checklists input)
        {
            
            var checklistForCompliance = new ChecklistForCompliants();
            var checklistString = new CheckListString();
            var checklistInput = new CheckListInputs();
            
            
            foreach (var compliance in input.ChecklistForCompliants)
            {
                switch (compliance.Value)
                {
                    case "Certificate of Analysis":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "Documentation Requirements";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Certificate of Product Registration":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "Documentation Requirements";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Food Grade Certificate":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "Documentation Requirements";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Meat Inspection Certificate":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "Documentation Requirements";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Purchase Order":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "Documentation Requirements";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Material Safety Data Sheet":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "Documentation Requirements";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Migration Test":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "Documentation Requirements";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Veterinary Health Certificate":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "Documentation Requirements";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Shipping Permit":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "Documentation Requirements";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "No rust, torn/detached parts, etc":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "OTHER CONFORMANCE PARAMETERS ON PRODUCT CONDITION/S";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Delivered in freezer/refeer van":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "OTHER CONFORMANCE PARAMETERS ON PRODUCT CONDITION/S";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Properly packed in clean plastic packaging materials/ containers/crates/ sack/boxes etc":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "OTHER CONFORMANCE PARAMETERS ON PRODUCT CONDITION/S";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Durable / elastic (if plastic/ packaging material":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "OTHER CONFORMANCE PARAMETERS ON PRODUCT CONDITION/S";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "No holes and/or tears":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "OTHER CONFORMANCE PARAMETERS ON PRODUCT CONDITION/S";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "With clear, correct and readable product information and label":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "OTHER CONFORMANCE PARAMETERS ON PRODUCT CONDITION/S";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "No spillages / leaks/wet portions":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "OTHER CONFORMANCE PARAMETERS ON PRODUCT CONDITION/S";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Stored/ delivered in clean and in good conditioned container (crates and/or pallets)":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "OTHER CONFORMANCE PARAMETERS ON PRODUCT CONDITION/S";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Each product type is segregated to avoid cross contamination":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "OTHER CONFORMANCE PARAMETERS ON PRODUCT CONDITION/S";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Absence of unnecessary things/ products inside the delivery truck that may contaminate the products":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "OTHER CONFORMANCE PARAMETERS ON PRODUCT CONDITION/S";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "No dirt, food debris, pest and signs of pest, etc.":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "OTHER CONFORMANCE PARAMETERS ON PRODUCT CONDITION/S";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "No off odor, detached/ disintegrated parts.":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "DELIVERY VEHICLE CONDITION";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "No rust, retained dirt, food debris or any sign of pest/pest infestation":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "DELIVERY VEHICLE CONDITION";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Cooling system is in good working condition and without leaks (if ref/ freezer van)":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "DELIVERY VEHICLE CONDITION";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Plastic curtains are available, complete and in good condition":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "DELIVERY VEHICLE CONDITION";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Clean and trimmed fingernails. No nail polish and false nails":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "HYGIENE PRACTICES";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Wearing clean, complete and appropriate uniform and/or working attire":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "HYGIENE PRACTICES";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Proper  and short haircut":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "HYGIENE PRACTICES";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Delivery personnel is apparently healthy":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "HYGIENE PRACTICES";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Cleanly shaven face":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "HYGIENE PRACTICES";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "Absence of loose items":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "HYGIENE PRACTICES";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "FIT FOR HUMAN CONSUMPTION":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "EVALUATION / DISPOSITION";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "NOT FIT FOR HUMAN CONSUMPTION":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "EVALUATION / DISPOSITION";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    case "ACCEPT":
                        checklistForCompliance.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistForCompliance.Checklist_Type = "EVALUATION / DISPOSITION";
                        checklistForCompliance.Value = compliance.Value;
                        checklistForCompliance.IsCompliant = compliance.IsCompliant;
                        break;
                    default:
                        checklistForCompliance.Checklist_Type = "Not Available";
                        checklistForCompliance.Value = "Not Available";
                        checklistForCompliance.IsCompliant = false;
                        break;
                }
            }
            
            foreach (var checklistStrings in input.ChecklistsString)
            {
                switch (checklistStrings.Checlist_Type)
                {
                    case "Color":
                        checklistString.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistString.Checlist_Type = checklistStrings.Checlist_Type;
                        checklistString.Value = checklistStrings.Value;
                        break;
                    case "Odor":
                        checklistString.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistString.Checlist_Type = checklistStrings.Checlist_Type;
                        checklistString.Value = checklistStrings.Value;
                        break;
                    case "Appearance":
                        checklistString.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistString.Checlist_Type = checklistStrings.Checlist_Type;
                        checklistString.Value = checklistStrings.Value;
                        break;
                    case "Texture":
                        checklistString.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistString.Checlist_Type = checklistStrings.Checlist_Type;
                        checklistString.Value = checklistStrings.Value;
                        break;
                    case "Absence Of Contaminants":
                        checklistString.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistString.Checlist_Type = checklistStrings.Checlist_Type;
                        checklistString.Value = checklistStrings.Value;
                        break;
                    case "Product Condition":
                        checklistString.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistString.Checlist_Type = checklistStrings.Checlist_Type;
                        checklistString.Value = checklistStrings.Value;
                        break;
                    case "Product / Commodity Type":
                        checklistString.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistStrings.Checlist_Type = checklistStrings.Checlist_Type;
                        checklistString.Value = checklistStrings.Value;
                        break;
                    default:
                        checklistStrings.Checlist_Type = null;
                        checklistString.Value = null;
                        break;
                }
            }
            
            foreach (var checklistInputs in input.CheckListInput)
            {
                switch (checklistInputs.Parameter)
                {
                    case "Width":
                        checklistInput.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistInput.Checlist_Type = "CONFORMANCE TO SET STANDARD SPECIFICATIONS";
                        checklistInput.Parameter = checklistInputs.Parameter;
                        checklistInput.Value = checklistInputs.Value;
                        break;
                    case "HEIGHT":
                        checklistInput.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistInput.Checlist_Type = "CONFORMANCE TO SET STANDARD SPECIFICATIONS";
                        checklistInput.Parameter = checklistInputs.Parameter;
                        checklistInput.Value = checklistInputs.Value;
                        break;
                    case "LENGTH":
                        checklistInput.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistInput.Checlist_Type = "CONFORMANCE TO SET STANDARD SPECIFICATIONS";
                        checklistInput.Parameter = checklistInputs.Parameter;
                        checklistInput.Value = checklistInputs.Value;
                        break;
                    case "THICKNESS":
                        checklistInput.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistInput.Checlist_Type = "CONFORMANCE TO SET STANDARD SPECIFICATIONS";
                        checklistInput.Parameter = checklistInputs.Parameter;
                        checklistInput.Value = checklistInputs.Value;
                        break;
                    case "DIAMETER":
                        checklistInput.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistInput.Checlist_Type = "CONFORMANCE TO SET STANDARD SPECIFICATIONS";
                        checklistInput.Parameter = checklistInputs.Parameter;
                        checklistInput.Value = checklistInputs.Value;
                        break;
                    case "RADIUS":
                        checklistInput.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistInput.Checlist_Type = "CONFORMANCE TO SET STANDARD SPECIFICATIONS";
                        checklistInput.Parameter = checklistInputs.Parameter;
                        checklistInput.Value = checklistInputs.Value;
                        break;
                    case "INTERNAL / SURFACE TEMPERATURE (if cold products)":
                        checklistInput.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistInput.Checlist_Type = "CONFORMANCE TO SET STANDARD SPECIFICATIONS";
                        checklistInput.Parameter = checklistInputs.Parameter;
                        checklistInput.Value = checklistInputs.Value;
                        break;
                    case "Delivery vehicle temperature (if product is delivered using freezer/reefer van)":
                        checklistInput.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistInput.Checlist_Type = "DELIVERY VEHICLE CONDITION";
                        checklistInput.Parameter = checklistInputs.Parameter;
                        checklistInput.Value = checklistInputs.Value;
                        break;
                    case "Delivery vehicle's plate number":
                        checklistInput.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistInput.Checlist_Type = "DELIVERY VEHICLE CONDITION";
                        checklistInput.Parameter = checklistInputs.Parameter;
                        checklistInput.Value = checklistInputs.Value;
                        break;
                    case "Name of delivery personnel":
                        checklistInput.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistInput.Checlist_Type = "DELIVERY VEHICLE CONDITION";
                        checklistInput.Parameter = checklistInputs.Parameter;
                        checklistInput.Value = checklistInputs.Value;
                        break;
                    case "QUANTITY REJECT":
                        checklistInput.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistInput.Checlist_Type = "EVALUATION / DISPOSITION";
                        checklistInput.Parameter = checklistInputs.Parameter;
                        checklistInput.Value = checklistInputs.Value;
                        break;
                    case "QUANTITY ACCEPT":
                        checklistInput.PO_ReceivingId = input.PO_Receiving.PO_Summary_Id;
                        checklistInput.Checlist_Type = "EVALUATION / DISPOSITION";
                        checklistInput.Parameter = checklistInputs.Parameter;
                        checklistInput.Value = checklistInputs.Value;
                        break;
                    default:
                        checklistInput.Checlist_Type = "Not Specified";
                        checklistInput.Parameter = "Not Specified";
                        checklistInput.Value = "Not Available";
                        break;
                }
            }
            
            await _context.QC_Receiving.AddAsync(input.PO_Receiving);
            await _context.ChecklistForCompliant.AddRangeAsync(checklistForCompliance);
            await _context.CheckListStrings.AddAsync(checklistString);
            await _context.CheckListInput.AddAsync(checklistInput);
            return true;
        }
    }
}