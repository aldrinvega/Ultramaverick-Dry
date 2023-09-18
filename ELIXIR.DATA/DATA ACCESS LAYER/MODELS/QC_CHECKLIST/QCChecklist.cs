using System;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class QCChecklist : BaseEntity
    {
        public int ReceivingId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual PO_Receiving PoReceiving { get; set; }
    }
}