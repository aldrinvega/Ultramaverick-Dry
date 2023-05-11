using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DTOs.SETUP_DTOs
{
    public class ProductTypeDto
    {
        public int Id { get; set; }
        public string ProductTypeName { get; set; }
        public bool IsActive { get; set; }
        public string DateAdded { get; set; } = DateTime.Now.ToString("MM-dd-yyyy");
    }
}
