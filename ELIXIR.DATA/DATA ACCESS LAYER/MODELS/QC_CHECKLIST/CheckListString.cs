using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class CheckListString
    {
        [Key]
        public int ChecklistStringId { get; set; }
        public int? PO_Summary_Id { get; set; }
        public int? ReceivingId { get; set; }
        public string Checlist_Type { get; set; }
        public List<string> Value { get; set; }
        public string Remarks { get; set; }
    }
}