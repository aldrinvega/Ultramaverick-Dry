using Microsoft.VisualBasic;

namespace ELIXIR.DATA.DTOs.SETUP_DTOs
{
    public class AnalysesDto
    {
        public int  Id { get; set; }
        public string AnalysisName { get; set; }
        public bool IsActive { get; set; }
        public string DateAdded { get; set; }
        public string ModifiedBy { get; set; }
        public string Reason { get; set; }
    }
}