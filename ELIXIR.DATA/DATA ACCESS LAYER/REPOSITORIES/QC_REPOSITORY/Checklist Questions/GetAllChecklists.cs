using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Questions
{
    public class GetAllChecklists
    {
        public class GetAllChecklistsQuery : UserParams, IRequest<PagedList<GetAllChecklistsQueryResult>>
        {
            public int? ProductTypeId { get; set; }
            public int? ChecklistTypeId { get; set; }
            public bool? Status { get; set; }
        }
        public class GetAllChecklistsQueryResult
        {
             public int ChecklistTypeId { get; set; }
             public string ChecklistType { get; set; }
             public List<ChecklistQuestion> ChecklistQuestions { get; set; }

            public class ChecklistQuestion
            {
                public int? ProductTypeId { get; set; }
                public string ProductType { get; set; }
                public int? OrderId { get; set; }
                public int Id { get; set; }
                public string ChecklistsQuestions { get; set; }
                public AnswerType AnswerType { get; set; }
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
                IQueryable<ChecklistTypes> checklistDescriptions = _context.ChecklistTypes
                    .Include(ct => ct.ChecklistQuestions)
                    .ThenInclude(x => x.ProductType)
                    .OrderBy(x => x.OrderId);

                if(request.ChecklistTypeId != null)
                {
                    checklistDescriptions = checklistDescriptions.Where(x => x.Id == request.ChecklistTypeId);
                }

                if (request.Status != null)
                {
                    checklistDescriptions = checklistDescriptions
                        .Where(x => x.ChecklistQuestions.Any(q => q.IsActive == request.Status));
                }

                var result = checklistDescriptions.Select(x => new GetAllChecklistsQueryResult
                {
                    ChecklistTypeId = x.Id,
                    ChecklistType = x.ChecklistType,
                    ChecklistQuestions = x.ChecklistQuestions
                    .Where(q => q.ChecklistTypeId != null)
                    .Select(x =>
                        new GetAllChecklistsQueryResult.ChecklistQuestion
                        {
                            ProductTypeId = x.ProductTypeId,
                            ProductType = x.ProductType.ProductTypeName,
                            OrderId = x.OrderId,
                            Id = x.Id,
                            ChecklistsQuestions = x.ChecklistQuestion,
                            AnswerType = x.AnswerType,
                            IsActive = x.IsActive,
                            CreatedAt = x.CreatedAt,
                            UpdatedAt = x.UpdatedAt,
                            AddedBy = x.AddedByUser.FullName
                        }).Where(x => x.ProductTypeId == request.ProductTypeId)
                          .Where(x => x.IsActive == request.Status).ToList()
                });
                
                return await PagedList<GetAllChecklistsQueryResult>.CreateAsync(result, request.PageNumber,
                    request.PageSize);
            }
        }
    }
}