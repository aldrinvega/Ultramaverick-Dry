using System;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY
{
    [Route("api/ChecklistDescription")]
    [ApiController]
    
    public class AddNewChecklistDescription : ControllerBase
    {
        private readonly IMediator _mediator;

        public AddNewChecklistDescription(IMediator mediator)
        {
            _mediator = mediator;
        }

        public class AddNewChecklistDescriptionCommand : IRequest<Unit>
        {
            public string ChecklistDescription { get; set; }
            public int ProductTypeId { get; set; }
            public int AddedBy { get; set; }
        }

        public class Handler : IRequestHandler<AddNewChecklistDescriptionCommand, Unit>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(AddNewChecklistDescriptionCommand request, CancellationToken cancellationToken)
            {
                var existingChecklistDesc =
                    await _context.ChecklistDescriptions.FirstOrDefaultAsync(x =>
                        x.ChecklistDescription == request.ChecklistDescription, cancellationToken);

                if (existingChecklistDesc != null)
                {
                    throw new Exception($"{request.ChecklistDescription} is already exist.");
                }

                var checklistDesc = new ChecklistDescriptions
                {
                    ChecklistDescription = request.ChecklistDescription,
                    ProductTypeId = request.ProductTypeId,
                    AddedBy = request.AddedBy,
                };

                await _context.ChecklistDescriptions.AddAsync(checklistDesc, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}