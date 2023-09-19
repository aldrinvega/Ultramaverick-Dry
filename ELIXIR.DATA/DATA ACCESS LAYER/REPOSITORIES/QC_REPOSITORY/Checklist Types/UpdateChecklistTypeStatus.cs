using System;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Types
{
    public class UpdateChecklistTypeStatus
    {
        public class UpdateChecklistTypeStatusCommand : IRequest<Unit>
        {
            public int ChecklistTypeId { get; set; }
        }

        public class Handler : IRequestHandler<UpdateChecklistTypeStatusCommand, Unit>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(UpdateChecklistTypeStatusCommand request, CancellationToken cancellationToken)
            {
                var existingChecklistType =
                    await _context.ChecklistTypes.FirstOrDefaultAsync(x => x.Id == request.ChecklistTypeId,
                        cancellationToken);

                if (existingChecklistType == null)
                {
                    throw new Exception("Checklist Type not found");
                }

                existingChecklistType.IsActive = !existingChecklistType.IsActive;

                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}