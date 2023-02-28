using System.ComponentModel.DataAnnotations;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class CheckListString
    {
        [Key]
        public int ChecklistStringId { get; set; }
        public int PO_ReceivingId { get; set; }
        public string Checlist_Type { get; set; }
        public string Value { get; set; }
        
    }
}