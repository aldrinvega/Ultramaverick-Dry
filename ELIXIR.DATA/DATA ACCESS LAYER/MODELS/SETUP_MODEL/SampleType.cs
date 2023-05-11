using System;
using Microsoft.VisualBasic;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class SampleType : BaseEntity
    {
        public string SampleTypeName { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public string Reason { get; set; }
        public string ModifiedBy { get; set; }
    }
}