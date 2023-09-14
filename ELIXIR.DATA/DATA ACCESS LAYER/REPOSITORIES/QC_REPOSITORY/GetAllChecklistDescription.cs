using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.HELPERS;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.SETUP_MODEL;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY
{
    public class GetAllChecklistsDescription
    {
        public class GetAllChecklistsDescriptionQuery : UserParams, IRequest<PagedList<GetAllChecklistsDescriptionQueryResult>>
        {
            public string Search { get; set; }
            public string ProductType { get; set; }
            public bool? Status { get; set; }
        }
        public class GetAllChecklistsDescriptionQueryResult
        {
                public int Id { get; set; }
                public string ChecklistDescription { get; set; }
                public bool IsActive { get; set; }
                public DateTime CreatedAt { get; set; }
                public DateTime UpdatedAt { get; set; }
                public string AddedBy { get; set; }
                public int ProductTypeId { get; set; }
                public string ProductType { get; set; }
        }
        
        public class Handler : IRequestHandler<GetAllChecklistsDescriptionQuery, PagedList<GetAllChecklistsDescriptionQueryResult>>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<PagedList<GetAllChecklistsDescriptionQueryResult>> Handle(GetAllChecklistsDescriptionQuery request,
                CancellationToken cancellationToken)
            {
                IQueryable<ChecklistDescriptions> checklistDescriptions = _context.ChecklistDescriptions
                    .Include(x => x.ProductType);

                if (!string.IsNullOrEmpty(request.Search))
                {
                    checklistDescriptions =
                        checklistDescriptions.Where(x => x.ChecklistDescription.Contains(request.Search));
                }
                if (!string.IsNullOrEmpty(request.ProductType))
                {
                    checklistDescriptions =
                        checklistDescriptions.Where(x => x.ProductType.ProductTypeName.Contains(request.ProductType));
                }

                if (request.Status != null)
                {
                    checklistDescriptions = checklistDescriptions.Where(x => x.IsActive == request.Status);
                }

                var result = checklistDescriptions
                    .Select(cd => new GetAllChecklistsDescriptionQueryResult
                    {
                        Id = cd.Id,
                        ChecklistDescription = cd.ChecklistDescription,
                        IsActive = cd.IsActive,
                        CreatedAt = cd.CreatedAt,
                        UpdatedAt = cd.UpdatedAt,
                        AddedBy = cd.AddedByUser != null ? cd.AddedByUser.FullName : "N/A",
                        ProductType = cd.ProductType.ProductTypeName,
                        ProductTypeId = cd.ProductTypeId
                    }).OrderBy(x => x.UpdatedAt);

                return await PagedList<GetAllChecklistsDescriptionQueryResult>.CreateAsync(result, request.PageNumber,
                    request.PageSize);
            }
        }
    }
}