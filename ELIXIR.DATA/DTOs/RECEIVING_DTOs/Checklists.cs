using System.Collections.Generic;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class Checklists
    {
        public PO_Receiving PO_Receiving { get; set; }
        public List<CheckListString> ChecklistsString { get; set; }
    }
}