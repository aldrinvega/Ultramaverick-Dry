using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.EXTENSIONS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ELIXIR.API.Features.Setup.User_Roles;

[Route("api/Roles"), ApiController]

public class GetAllUserRolesAsync : ControllerBase
{
    private readonly IMediator _mediator;

    public GetAllUserRolesAsync(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("page")]
    public async Task<IActionResult> GetUserRoles([FromQuery] GetAllUserRolesAsyncQuery request)
    {
        var roles = await _mediator.Send(request);
        Response.AddPaginationHeader(
            roles.CurrentPage, 
            roles.PageSize, 
            roles.TotalCount, 
            roles.TotalPages, 
            roles.HasNextPage, 
            roles.HasPreviousPage);

        var rolesResult = new
        {
            roles,
            roles.CurrentPage,
            roles.PageSize,
            roles.TotalCount,
            roles.TotalPages,
            roles.HasNextPage,
            roles.HasPreviousPage
        };
        return Ok(rolesResult);
    }
    public class GetAllUserRolesAsyncQuery : UserParams, IRequest<PagedList<GetAllUserRolesAsyncResult>>
    {
        public string Search { get; set; }
        public bool Status { get; set; }
    }

    public class GetAllUserRolesAsyncResult
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

    public class Handler : IRequestHandler<GetAllUserRolesAsyncQuery, PagedList<GetAllUserRolesAsyncResult>>
    {
        private readonly StoreContext _context;

        public Handler(StoreContext context)
        {
            _context = context;
        }

        public async Task<PagedList<GetAllUserRolesAsyncResult>> Handle(GetAllUserRolesAsyncQuery request, CancellationToken cancellationToken)
        {
            var roles = _context.Roles.OrderByDescending(x => x.DateAdded)
                                       .Select(role => new GetAllUserRolesAsyncResult
                                       {
                                           Id = role.Id,
                                           RoleName = role.RoleName,
                                           IsActive = role.IsActive,
                                           DateAdded = (role.DateAdded).ToString("MM/dd/yyyy"),
                                           AddedBy = role.AddedBy,
                                           ModifiedBy = role.ModifiedBy,
                                           DateModified = (role.DateModified).ToString("MM/dd/yyyy"),
                                           Reason = role.Reason
                                       }).Where(x => x.IsActive == request.Status);
           
            if (!string.IsNullOrEmpty(request.Search))
            {
                roles = roles.Where(x => x.RoleName.Contains(request.Search));
            }

            return await PagedList<GetAllUserRolesAsyncResult>.CreateAsync(roles, request.PageNumber, request.PageSize);
        }
    }
}
