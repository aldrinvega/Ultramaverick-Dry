using System.Threading;
using System.Threading.Tasks;
using ELIXIR.API.Common;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.API.Features.Setup.User_Roles;

public class UpdateUserRoleStatus
{
    public class UpdateUserRoleStatusCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }

    public class Handler : IRequestHandler<UpdateUserRoleStatusCommand, Result>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateUserRoleStatusCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (role is null)
            {
                return UserRoleErrors.UserRoleNotFound();
            }

            role.Reason = role.IsActive ? "Change Data" : "Reopened Role";
            role.IsActive = !role.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
