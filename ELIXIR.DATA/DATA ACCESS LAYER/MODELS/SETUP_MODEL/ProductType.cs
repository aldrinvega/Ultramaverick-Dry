using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using Swashbuckle.AspNetCore.Annotations;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class ProductType : BaseEntity
    {
            public string ProductTypeName { get; set; }
            public bool IsActive { get; set; } = true;
            public DateTime DateAdded { get; set; } = DateTime.Now;
            public string AddedBy { get; set; }
            public string ModifiedBy { get; set; }

            public virtual ICollection<ChecklistTypes> ChecklistTypes { get; set; }
    }
}