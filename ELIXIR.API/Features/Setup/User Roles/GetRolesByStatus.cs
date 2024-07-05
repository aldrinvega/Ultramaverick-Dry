using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.API.Common;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.API.Features.Setup.User_Roles;

public class GetRolesByStatus
{
    public class GetRolesByStatusQuery : IRequest<Result>
    {
        public bool Status { get; set; }
    }

    public class GetRolesByStatusResult
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public string DateAdded { get; set; }
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string DateModified { get; set; }
        public string Reason { get; set; }
    }

    public class Handler : IRequestHandler<GetRolesByStatusQuery, Result>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(GetRolesByStatusQuery request, CancellationToken cancellationToken)
        {
           var roles = await _context.Roles
                                        .Select(role => new GetRolesByStatusResult
                                        {
                                            Id = role.Id,
                                            RoleName = role.RoleName,
                                            IsActive = role.IsActive,
                                            DateAdded = (role.DateAdded).ToString("MM/dd/yyyy"),
                                            AddedBy = role.AddedBy,
                                            ModifiedBy = role.ModifiedBy,
                                            DateModified = (role.DateModified).ToString("MM/dd/yyyy"),
                                            Reason = role.Reason
                                        })
                                        .Where(x => x.IsActive == request.Status)
                                        .ToListAsync();

            return Result.Success(roles);
        }
    }
}
