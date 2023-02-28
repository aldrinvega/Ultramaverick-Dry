using System.ComponentModel.DataAnnotations;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
        public class ChecklistForCompliants
        {
            [Key]
            public int ChecklistCompliantId { get; set; }
            public int PO_ReceivingId { get; set; }
            public string Checklist_Type { get; set; }
            public string Value { get; set; }
            public bool IsCompliant { get; set; }
        }
}