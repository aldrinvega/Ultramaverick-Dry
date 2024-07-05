using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using ELIXIR.API.Common;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.API.Features.Setup.User_Roles;

public class GetUntagModulesByRoleId
{
    public class GetUntagModulesByRoleIdRequest : IRequest<Result>
    {
        public int Id { get; set; }
        public int MenuId { get; set; }
    }
    public record class GetUntagModulesByRoleIdResult
    {
        public string Remakrs { get; set; }
        public string MainMenu { get; set; }
        public string SubMenu { get; set; }
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public bool IsActive { get; set; }
    }

    public class Handler : IRequestHandler<GetUntagModulesByRoleIdRequest, Result>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetUntagModulesByRoleIdRequest request, CancellationToken cancellationToken)
        {
            var availableModules = await _context.Modules
            .Where(x => x.MainMenuId == request.MenuId)
            .Where(x => !_context.RoleModules
            .Where(x => x.RoleId == request.Id)
            .Where(x => x.IsActive == true)
            .Select(x => x.ModuleId)
            .Contains(x.Id))
            .Select(roleModules => new GetUntagModulesByRoleIdResult
            {
                Remakrs = "Untag",
                MainMenu = roleModules.MainMenu.ModuleName,
                SubMenu = roleModules.SubMenuName,
                RoleId = request.Id,
                ModuleId = roleModules.Id,
                IsActive = roleModules.IsActive
            }).ToListAsync();
            
           return Result.Success(availableModules);
        }
    }
}
