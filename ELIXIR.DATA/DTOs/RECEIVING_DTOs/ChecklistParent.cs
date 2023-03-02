using System.Collections.Generic;

namespace ELIXIR.DATA.DTOs.RECEIVING_DTOs
{
    public class ChecklistParent
    {
        public int PO_Summary_Id { get; set; }
        public List<ChecklistStringDTO> ChecklistString { get; set; }
        public List<CheclistInputDTO> ChecklistInput { get; set; }
        public List<ChecklistCompliantsDTO> ChecklistCompliants { get; set; }
    }
}