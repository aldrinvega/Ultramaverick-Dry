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
        public int PO_ReceivingId { get; set; }
        public string Checlist_Type { get; set; }

        [NotMapped]
        public List<string> Values
        {
            get => JsonSerializer.Deserialize<List<string>>(Value);
            set => Value = JsonSerializer.Serialize(value);
        }
        public string Value { get; set; }

        public string Remarks { get; set; }
        public PO_Receiving PoReceiving { get; set; }
    }
}