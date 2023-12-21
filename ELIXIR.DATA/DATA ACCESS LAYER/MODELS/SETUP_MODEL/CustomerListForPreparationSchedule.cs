using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.ORDERING_MODEL;
using ELIXIR.DATA.DTOs.ORDERING_DTOs;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class CustomerListForPreparationSchedule
    {
        public int Id
        {
            get;
            set;
        }
        public string CustomerCode
        {
            get; set;
        }
        public string CustomerName
        {
            get; set;
        }
        
        public string CompanyName
        {
            get; set;
        }
         
        public string LocationName
        {
            get; set;
        }
        public string DepartmentName
        {
            get; set;
        }
        public string FarmName
        {
            get;
            set;
        }

        public List<Ordering> Orders { get; set; } = new();
    }
}
