using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.ORDERING_DTOs
{
    public class AllocationFinalResult
    {
        public List<AllocationDTO> Allocations
        {
        get; set; }
        public int SOH
        {
            get;
            set;
        }
    }
}
