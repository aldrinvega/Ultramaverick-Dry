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

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Questions
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
            public List<ChecklistTypesCollection> ChecklistTypes { get; set; }

            public class ChecklistTypesCollection
            {
                public int Id { get; set; }
                public string ChecklistType { get; set; }
                public List<ChecklistQuestion> ChecklistQuestions { get; set; }
            }

            public class ChecklistQuestion
            {
                public int Id { get; set; }
                public string ChecklistsQuestions { get; set; }
                public bool IsOpenField { get; set; }
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
                    .Include(x => x.ChecklistTypes)
                    .ThenInclude(ct => ct.ChecklistQuestions);

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
                        ChecklistTypes = x.ChecklistTypes.Select(cd => new GetAllChecklistsQueryResult.ChecklistTypesCollection
                        {
                            Id = cd.Id,
                            ChecklistType = cd.ChecklistType,
                            ChecklistQuestions = cd.ChecklistQuestions.Select(x => new GetAllChecklistsQueryResult.ChecklistQuestion
                            {
                                Id = x.Id,
                                ChecklistsQuestions = x.ChecklistQuestion,
                                IsOpenField = x.IsOpenField,
                                IsActive = x.IsActive,
                                CreatedAt = x.CreatedAt,
                                UpdatedAt = x.UpdatedAt,
                                AddedBy = x.AddedByUser.FullName
                            }).ToList()
                        }).ToList()
                    });
                return await PagedList<GetAllChecklistsQueryResult>.CreateAsync(result, request.PageNumber,
                    request.PageSize);
            }
        }
    }
}