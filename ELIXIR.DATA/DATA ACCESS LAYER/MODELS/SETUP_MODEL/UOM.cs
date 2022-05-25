using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL
{
    public class UOM : BaseEntity
    {
     //   [Required(ErrorMessage = "UOM Code is Required!")]
        public string UOM_Code { get; set; }

     //   [Required(ErrorMessage = "UOM Description is Required!")]
        public string UOM_Description { get; set; }

        public DateTime DateAdded { get; set; }
        public string AddedBy { get; set; }
        public bool IsActive { get; set; }
        public string Reason { get; set; }

    }
}
