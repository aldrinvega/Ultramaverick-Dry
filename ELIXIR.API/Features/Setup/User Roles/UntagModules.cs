using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.API.Common;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.API.Features.Setup.User_Roles;

public class UntagModules
{
    public class UntagModulesCommand : IRequest<Result>
    {
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
    }

    public class Handler : IRequestHandler<UntagModulesCommand, Result>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UntagModulesCommand request, CancellationToken cancellationToken)
        {
            var existingrolemodule = await _context.RoleModules
            .Where(x => x.RoleId == request.RoleId)
            .Where(x => x.ModuleId == request.ModuleId)
            .FirstOrDefaultAsync();

            var existingRoles = await _context.Roles
            .Where(x => x.Id == request.RoleId)
            .FirstOrDefaultAsync();

            if (existingRoles == null)
            {
                return UserRoleErrors.UserRoleNotFound();
            }

            if (existingrolemodule is null)
            {
                return UserRoleErrors.ModuleNotFound();
            }

            existingrolemodule.IsActive = false;
            _context.RoleModules.Update(existingrolemodule);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
