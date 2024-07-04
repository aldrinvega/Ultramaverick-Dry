using System;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.API.Common;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ELIXIR.API.Features.Setup.User_Roles;

public class UpdateUserRole
{
    public class UpdateUserRoleCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class Handler : IRequestHandler<UpdateUserRoleCommand, Result>
    {
        private readonly StoreContext _context;
        public async Task<Result> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (role is null)
            {
                return UserRoleErrors.UserRoleNotFound();
            }

            role.RoleName = request.RoleName;
            role.ModifiedBy = request.ModifiedBy;
            role.DateModified = DateTime.Now;

            if (request.ModifiedBy == null)
            {
                role.ModifiedBy = "Admin";
            }

            return Result.Success();
        }
    }
}
