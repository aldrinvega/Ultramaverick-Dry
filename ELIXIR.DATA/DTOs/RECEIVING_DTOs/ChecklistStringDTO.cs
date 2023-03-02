using System.Collections.Generic;

namespace ELIXIR.DATA.DTOs.RECEIVING_DTOs
{
    public class ChecklistStringDTO
    {
        public string Checklist_Type { get; set; }
        public List<string> Values { get; set; }
    }
}