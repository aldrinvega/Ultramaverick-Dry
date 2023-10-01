using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class QCChecklist : BaseEntity
    {
        public int ReceivingId { get; set; }
        public int? ProductTypeId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual PO_Receiving PoReceiving { get; set; }
        public virtual IEnumerable<ChecklistAnswers> ChecklistAnswers { get; set; }
        public virtual IEnumerable<ChecklistOpenFieldAnswer> OpenFieldAnswers { get; set; }
        public virtual IEnumerable<ChecklistProductDimension> ProductDimension { get; set; }
        public virtual ProductType ProductType { get; set; }
    }
}