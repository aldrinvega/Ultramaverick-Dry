using System;
using System.Collections.Generic;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class ChecklistTypes : BaseEntity
    {
        public string ChecklistType { get; set; }
        public int? OrderId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; }
        public int AddedBy { get; set; }
        public int? ModifiedBy { get; set; }

        public User AddedByUser { get; set; }
        public User ModifiedByUser { get; set; }
        public ICollection<ChecklistQuestions> ChecklistQuestions { get; set; }
    }
}