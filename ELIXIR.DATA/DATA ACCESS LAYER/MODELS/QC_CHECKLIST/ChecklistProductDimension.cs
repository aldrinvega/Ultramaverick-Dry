namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class ChecklistProductDimension : BaseEntity
    {
        public int QcChecklistId { get; set; }
        public int ChecklistQuestionId { get; set; }
        public string Standard { get; set; }
        public string Actual { get; set; }

        public virtual QCChecklist QcChecklist { get; set; }
        public virtual ChecklistQuestions ChecklistQuestions { get; set; }
    }
}