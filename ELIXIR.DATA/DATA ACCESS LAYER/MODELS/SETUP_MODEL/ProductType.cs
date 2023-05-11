using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class ProductType : BaseEntity
    {
        public string ProductTypeName { get; set; }
        public bool IsAcctive { get; set; }
        public string Date  { get; set; } = DateTime.Now.ToString("mm-dd-yyyy");
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
}
