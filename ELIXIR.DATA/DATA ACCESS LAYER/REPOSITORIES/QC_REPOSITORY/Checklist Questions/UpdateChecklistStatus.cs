using System;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Questions
{
    public class UpdateChecklistStatus
    {
        public class UpdateChecklistStatusCommand : IRequest<Unit>
        {
            public int ChecklistId { get; set; }
        }

        public class Handler : IRequestHandler<UpdateChecklistStatusCommand, Unit>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(UpdateChecklistStatusCommand request, CancellationToken cancellationToken)
            {
                var existingChecklistDescription =
                    await _context.ChecklistQuestions.FirstOrDefaultAsync(x => x.Id == request.ChecklistId,
                        cancellationToken);

                if (existingChecklistDescription == null)
                {
                    throw new Exception("Checklist Description not found");
                }

                existingChecklistDescription.IsActive = !existingChecklistDescription.IsActive;
                existingChecklistDescription.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;

            }
        }
    }
}