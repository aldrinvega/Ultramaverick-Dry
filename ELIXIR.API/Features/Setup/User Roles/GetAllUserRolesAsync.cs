using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.API.Common;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static ELIXIR.API.Features.Setup.User_Roles.GetAllUserRolesPaginationAsync;

namespace ELIXIR.API.Features.Setup.User_Roles;

public class GetAllUserRolesAsync
{
    public class GetAllUserRolesAsyncQuery : IRequest<Result> {}

    public class GetAllUserRolesAsyncResponse
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public string DateAdded { get; set; }
        public string DateModified { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string Reason { get; set; }
    }

    public class Handler : IRequestHandler<GetAllUserRolesAsyncQuery, Result>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetAllUserRolesAsyncQuery request, CancellationToken cancellationToken)
        {
          var userRoles = await _context.Roles
                .Where(role => role.IsActive)
                .OrderByDescending(role => role.DateAdded)
                .Select(role => new GetAllUserRolesAsyncResponse
                {
                    Id = role.Id,
                    RoleName = role.RoleName,
                    IsActive = role.IsActive,
                    DateAdded = role.DateAdded.ToString("MM/dd/yyyy"),
                    AddedBy = role.AddedBy,
                    ModifiedBy = role.ModifiedBy,
                    DateModified = role.DateModified.ToString("MM/dd/yyyy"),
                    Reason = role.Reason
                }).ToListAsync(cancellationToken);

            return Result.Success(userRoles);
        }
    }
}
