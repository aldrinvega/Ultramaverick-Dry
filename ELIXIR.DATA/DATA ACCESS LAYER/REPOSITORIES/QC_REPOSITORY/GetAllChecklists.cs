using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY
{
    public class GetAllChecklists
    {
        public class GetAllChecklistsQuery : UserParams, IRequest<PagedList<GetAllChecklistsQueryResult>>
        {
            public string ProductType { get; set; }
            public bool? Status { get; set; }
        }
        public class GetAllChecklistsQueryResult
        {
            public int ProductTypeId { get; set; }
            public string ProductType { get; set; }
            public List<ChecklistDescriptions> ChecklistDescription { get; set; }
            public class ChecklistDescriptions
            {
                public int Id { get; set; }
                public string ChecklistDescription { get; set; }
                public bool IsActive { get; set; }
                public DateTime CreatedAt { get; set; }
                public DateTime UpdatedAt { get; set; }
                public string AddedBy { get; set; }
            }
        }
        
        public class Handler : IRequestHandler<GetAllChecklistsQuery, PagedList<GetAllChecklistsQueryResult>>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetAllChecklistsQueryResult>> Handle(GetAllChecklistsQuery request,
                CancellationToken cancellationToken)
            {
                IQueryable<ProductType> checklistDescriptions = _context.ProductTypes
                    .Include(x => x.ChecklistDescription);

                if (!string.IsNullOrEmpty(request.ProductType))
                {
                    checklistDescriptions =
                        checklistDescriptions.Where(x => x.ProductTypeName.Contains(request.ProductType));
                }

                if (request.Status != null)
                {
                    checklistDescriptions = checklistDescriptions.Where(x => x.IsActive == request.Status);
                }
                
                var result = checklistDescriptions.Select(x => new GetAllChecklistsQueryResult
                    {
                        ProductTypeId = x.Id,
                        ProductType = x.ProductTypeName,
                        ChecklistDescription = x.ChecklistDescription.Select(cd => new GetAllChecklistsQueryResult.ChecklistDescriptions
                        {
                            Id = cd.Id,
                            ChecklistDescription = cd.ChecklistDescription,
                            IsActive = cd.IsActive,
                            CreatedAt = cd.CreatedAt,
                            UpdatedAt = cd.UpdatedAt,
                            AddedBy = cd.AddedByUser != null ? cd.AddedByUser.FullName : "N/A" 
                        }).ToList()
                    });

                return await PagedList<GetAllChecklistsQueryResult>.CreateAsync(result, request.PageNumber,
                    request.PageSize);
            }
        }
    }
}