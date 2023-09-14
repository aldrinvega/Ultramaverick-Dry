using System;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY
{
    public class UpdateChecklistDescription
    {
        public class UpdateChecklistDescriptionCommand : IRequest<Unit>
        {
            public int Id { get; set; }
            public string ChecklistDescription { get; set; }
            public int ProductTypeId { get; set; }
        }
        public class Handler : IRequestHandler<UpdateChecklistDescriptionCommand, Unit>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(UpdateChecklistDescriptionCommand request, CancellationToken cancellationToken)
            {
                var existingChecklistDescription =
                    await _context.ChecklistDescriptions.FirstOrDefaultAsync(x => x.Id == request.Id,
                        cancellationToken);
                var isChecklistAlreadyExist =
                    await _context.ChecklistDescriptions.AnyAsync(
                        x => x.ChecklistDescription == request.ChecklistDescription, cancellationToken);

                if (isChecklistAlreadyExist)
                {
                    throw new Exception("Checklist description is already exist");
                }
                
                if (existingChecklistDescription == null)
                {
                    throw new Exception("Checklist description is not found");
                }
                
                existingChecklistDescription.ChecklistDescription = request.ChecklistDescription;
                existingChecklistDescription.ProductTypeId = request.ProductTypeId;
                existingChecklistDescription.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}