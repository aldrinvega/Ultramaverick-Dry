using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class ChecklistTypes : BaseEntity
    {
        public string ChecklistType  { get; set; }
        public int? ProductTypeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; }

        public ProductType ProductType { get; set; }
        public ICollection<ChecklistQuestions> ChecklistQuestions { get; set; }
    }
}