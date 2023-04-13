using System.Collections.Generic;

namespace ELIXIR.DATA.DTOs.RECEIVING_DTOs
{
    public class ChecklistCompliantsDTO
    {
        public string Checklist_Type { get; set; }
        //public List<string> Values { get; set; }
        public string Value { get; set; }
        public bool IsCompliant { get; set; }
    }
}