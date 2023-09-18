using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Types
{
    public class UpdateChecklistType
    {
        public class UpdateChecklistTypeCommand : IRequest<Unit>
        {
            public string ChecklistType { get; set; }
            public int ProductTypeId { get; set; }
        }
        
        public class Handler : IRequestHandler<UpdateChecklistTypeCommand, Unit>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(UpdateChecklistTypeCommand request, CancellationToken cancellationToken)
            {
                var existingChecklistType =
                    await _context.ChecklistTypes.FirstOrDefaultAsync(x => x.ChecklistType == request.ChecklistType,
                        cancellationToken);
                

                if (existingChecklistType != null)
                {
                    throw new Exception("Checklist type is already exist");
                }

                existingChecklistType.ChecklistType = request.ChecklistType;

                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}