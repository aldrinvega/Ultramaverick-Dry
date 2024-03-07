using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Types
{
    public class GetAllChecklistType
    {
        public class GetAllChecklistTypeQuery : UserParams, IRequest<PagedList<GetAllChecklistTypeResult>>
        {
            public string Search { get; set; }
            public bool? Status { get; set; }
        }

        public class GetAllChecklistTypeResult
        {
            public int Id { get; set; }
            public string ChecklistType { get; set; }
            public bool IsActive { get; set; }
            public string AddedBy { get; set; }
            public string ModifiedBy { get; set; }

        }
        
        public class Handler : IRequestHandler<GetAllChecklistTypeQuery, PagedList<GetAllChecklistTypeResult>>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetAllChecklistTypeResult>> Handle(GetAllChecklistTypeQuery request, CancellationToken cancellationToken)
            {
                IQueryable<ChecklistTypes> checklistTypes = _context.ChecklistTypes.OrderBy(x => x.OrderId);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    checklistTypes = checklistTypes.Where(x => x.ChecklistType == request.Search);
                }
                
                if (request.Status != null)
                {
                    checklistTypes = checklistTypes.Where(x => x.IsActive == request.Status);
                }

                var result = checklistTypes.Select(x => new GetAllChecklistTypeResult
                {
                    Id = x.Id,
                    ChecklistType = x.ChecklistType,
                    IsActive = x.IsActive,
                    AddedBy = x.AddedByUser.FullName,
                    ModifiedBy = x.ModifiedByUser.FullName
                });

                return await PagedList<GetAllChecklistTypeResult>.CreateAsync(result, request.PageNumber,
                    request.PageSize);
            }
        }
    }
}