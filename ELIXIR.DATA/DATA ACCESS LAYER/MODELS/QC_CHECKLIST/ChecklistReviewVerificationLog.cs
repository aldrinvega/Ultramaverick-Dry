namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class ChecklistReviewVerificationLog : BaseEntity
    {
        public int QCChecklistId { get; set; }
        public int DispositionId { get; set; }
        public int QtyAccepted { get; set; }
        public int QtyRejected { get; set; }
        public string MonitoredBy { get; set; }
        public string ReviewedBy { get; set; }
        public string VerifiedBy { get; set; }
        public string NotedBy { get; set; }

        public virtual QCChecklist QCChecklist { get; set; }
    }
}