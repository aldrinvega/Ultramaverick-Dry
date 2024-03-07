using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.USER_MODEL;

namespace ELIXIR.DATA.SERVICES
{
    public class UserWithSpecification : BaseSpecification<User>
    {
        public UserWithSpecification()
        {
            AddInclude(x => x.UserRole);
            AddInclude(x => x.Department);
        }
    }

}
