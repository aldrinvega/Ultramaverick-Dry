using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Operation;

namespace ELIXIR.DATA.DTOs.RECEIVING_DTOs
{
    public class Checklists
    {
        public PO_Receiving PO_Receiving { get; set; }
        public AddNewChecklist.AddNewChecklistCommand Checklist { get; set; }
    }
}