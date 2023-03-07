using System.Collections.Generic;

namespace ELIXIR.DATA.DTOs.ORDERING_DTOs
{
    public class ManualAllocation
    {
        public int OrderNoPKey { get; set; }
        public string ItemCode { get; set; }
        public int QuantityOrdered  { get; set; }
    }
}