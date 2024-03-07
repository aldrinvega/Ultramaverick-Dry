using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.USER_REPOSITORY.Exceptions
{
    public class NoRolesFooundException : Exception
    {
        public NoRolesFooundException(int id) : base($"No roles found with {id} id") { }
    }
}
