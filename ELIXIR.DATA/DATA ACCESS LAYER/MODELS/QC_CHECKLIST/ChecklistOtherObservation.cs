namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class ChecklistOtherObservation : BaseEntity
    {
        public int QCChecklistId { get; set; }
        public string Observation { get; set; }

        public virtual QCChecklist QCChecklist { get; set; }
    }
}