using System;
using System.Collections;
using System.Collections.Generic;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class QCChecklist : BaseEntity
    {
        public int ReceivingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual PO_Receiving PoReceiving { get; set; }
        public virtual IEnumerable<ChecklistAnswers> ChecklistAnswers { get; set; }
        public virtual IEnumerable<ChecklistOpenFieldAnswer> OpenFieldAnswers { get; set; }
        public virtual ChecklistOtherObservation ChecklistOtherObservation { get; set; }
        public virtual IEnumerable<ChecklistProductDimension> ProductDimension { get; set; }
        public virtual ChecklistCompliance ChecklistCompliance { get; set; }
        public virtual ChecklistReviewVerificationLog ChecklistReviewVerificationLog { get; set; }
    }
}