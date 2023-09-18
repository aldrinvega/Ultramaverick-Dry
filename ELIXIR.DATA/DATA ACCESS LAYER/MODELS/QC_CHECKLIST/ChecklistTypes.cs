using System;
using System.Collections.Generic;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class ChecklistTypes : BaseEntity
    {
        public string ChecklistType  { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; }
        
        public ICollection<ChecklistQuestions> ChecklistQuestions { get; set; }
    }
}