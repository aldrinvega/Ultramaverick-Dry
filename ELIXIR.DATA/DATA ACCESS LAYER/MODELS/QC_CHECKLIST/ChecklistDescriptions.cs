using System;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST
{
    public class ChecklistDescriptions : BaseEntity
    {
        public string ChecklistDescription { get; set; }
        public int ProductTypeId { get; set; }
        public bool IsActive { get; set; } = true;
        public int AddedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        public User AddedByUser { get; set; }
        public ProductType ProductType { get; set; }
    }
}