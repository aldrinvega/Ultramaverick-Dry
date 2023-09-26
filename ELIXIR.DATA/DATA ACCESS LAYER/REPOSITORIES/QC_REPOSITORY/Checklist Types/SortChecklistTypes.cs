using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Types
{
    public class SortChecklistTypes
    {
        public class SortChecklistTypesCommand : IRequest<Unit>
        {
            public IEnumerable<ChecklistType> ChecklistTypes { get; set; }

            public class ChecklistType
            {
                public int ChecklistTypeId { get; set; }
                public int OrderId { get; set; }
            }
            
        }
        public class Handler : IRequestHandler<SortChecklistTypesCommand, Unit>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(SortChecklistTypesCommand request, CancellationToken cancellationToken)
            {

                foreach (var checklistAnswer in request.ChecklistTypes)
                {
                    var checklistTypes = await _context.ChecklistTypes.FirstOrDefaultAsync(x => x.Id == checklistAnswer.ChecklistTypeId);

                    checklistTypes.OrderId = checklistAnswer.OrderId;
                    await _context.ChecklistTypes.AddAsync(checklistTypes, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}
