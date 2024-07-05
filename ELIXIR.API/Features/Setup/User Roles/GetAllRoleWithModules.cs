using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.API.Common;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.API.Features.Setup.User_Roles;

public class GetAllRoleWithModules
{
    public class GetAllRolesWithModulesQuery : IRequest<Result>{}
    public class GetAllrolesWithModulesResult
    {
        public string RoleName { get; set; }
        public string MainMenu { get; set; }
        public string SubMenu { get; set; }
        public string ModuleName { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }

    }

    public class Handler : IRequestHandler<GetAllRolesWithModulesQuery, Result>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetAllRolesWithModulesQuery request, CancellationToken cancellationToken)
        {
            var relesWithModules = await _context.RoleModules
                .Include(rm => rm.Role)
                .Include(rm => rm.Module)
                .Select(rm => new GetAllrolesWithModulesResult
                {
                    RoleName = rm.Role.RoleName,
                    MainMenu = rm.Module.MainMenu.ModuleName,
                    SubMenu = rm.Module.SubMenuName,
                    ModuleName = rm.Module.ModuleName,
                    Id = rm.Module.Id,
                    IsActive = rm.Role.IsActive
                })
                .ToListAsync(cancellationToken);

            return Result.Success(relesWithModules);
        }

    }
}
