using Swashbuckle.AspNetCore.Annotations;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{ 
    public class ChecklistOpenFieldAnswer : BaseEntity
    {
        public int QCChecklistId { get; set; }
        public int ChecklistQuestionId { get; set; }
        public string Remarks { get; set; }

        public virtual QCChecklist QCChecklist { get; set; }
        public virtual ChecklistQuestions ChecklistQuestion { get; set; }
    }
}