using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.USER_REPOSITORY.Exceptions
{
    public class RoleCountShouldBeMorethan1Exception : Exception
    {
        public RoleCountShouldBeMorethan1Exception() : base("At least one module needs to be assigned to this role.") { }
    }
}
