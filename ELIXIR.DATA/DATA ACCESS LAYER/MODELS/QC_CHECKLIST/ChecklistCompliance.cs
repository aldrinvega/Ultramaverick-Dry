namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class ChecklistCompliance : BaseEntity
    {
        public int QCChecklistId { get; set; }
        public string Compliance { get; set; }
        public string Description { get; set; }
        public string RootCause { get; set; }

        public virtual QCChecklist QCChecklist { get; set; }
    }
}