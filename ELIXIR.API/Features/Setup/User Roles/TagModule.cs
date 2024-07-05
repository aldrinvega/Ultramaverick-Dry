using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.API.Common;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.API.Features.Setup.User_Roles;

public class TagModule
{
    public class TagModuleCommand : IRequest<Result>
    {
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
    }
    public class Handler : IRequestHandler<TagModuleCommand, Result>
    {
        private readonly StoreContext _context;
        public Handler(StoreContext context)
        {
            _context = context;
        }
        public async Task<Result> Handle(TagModuleCommand request, CancellationToken cancellationToken)
        {
            var existingrolemodule = await _context.RoleModules
            .Where(x => x.RoleId == request.RoleId && x.ModuleId == request.ModuleId && !x.IsActive)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (existingrolemodule is not null)
            {
                existingrolemodule.IsActive = true;
                await _context.SaveChangesAsync(cancellationToken);
                return Result.Success();
            }

            return UserRoleErrors.ModuleNotFound();
        }
    }
}
