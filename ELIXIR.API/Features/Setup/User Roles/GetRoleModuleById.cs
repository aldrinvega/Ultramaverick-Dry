using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.API.Common;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.API.Features.Setup.User_Roles;

public class GetRoleModuleById
{
    public class GetRoleModuleByIdRequest : IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class GetRoleModulesByIdResult
    {
        public string RoleName { get; set; }
        public string MainMenu { get; set; }
        public int MainMenuId { get; set; }
        public string MenuPath { get; set; }
        public string SubMenuName { get; set; }
        public string ModuleName { get; set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public string ModuleStatus { get; set; }
    }

    public class Handler : IRequestHandler<GetRoleModuleByIdRequest, Result>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetRoleModuleByIdRequest request, CancellationToken cancellationToken)
        {
            var roleModules = await _context.RoleModules
                .Include(rm => rm.Module)
                .Include(rm => rm.Role)
                .Where(rm => rm.RoleId == request.Id && rm.IsActive)
                .Select(rm => new GetRoleModulesByIdResult
                {
                    RoleName = rm.Role.RoleName,
                    MainMenu = rm.Module.MainMenu.ModuleName,
                    MainMenuId = rm.Module.MainMenuId,
                    MenuPath = rm.Module.MainMenu.MenuPath,
                    SubMenuName = rm.Module.SubMenuName,
                    ModuleName = rm.Module.ModuleName,
                    Id = rm.Module.Id,
                    IsActive = rm.Role.IsActive,
                    RoleId = rm.RoleId,
                    ModuleStatus = rm.Module.ModuleStatus
                })
                .ToListAsync(cancellationToken: cancellationToken);

            return Result.Success(roleModules);
        }
    }
}
