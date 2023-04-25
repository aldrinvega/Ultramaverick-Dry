using System;
using Microsoft.VisualBasic;

namespace ELIXIR.DATA.DTOs.SETUP_DTOs
{
    public class DispositionDto
    {
        public int Id { get; set; }
        public string DispositionName { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateAdded { get; set; }
        public string ModifiedBy { get; set; }
        public string Reason { get; set; }
    }
}