namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class ChecklistOpenFieldAnswer : BaseEntity
    {
        public int QcChecklistId { get; set; }
        public int ChecklistQuestionId { get; set; }
        public string Remarks { get; set; }

        public virtual QCChecklist QcChecklist { get; set; }
        public virtual ChecklistQuestions ChecklistQuestions { get; set; }
    }
}