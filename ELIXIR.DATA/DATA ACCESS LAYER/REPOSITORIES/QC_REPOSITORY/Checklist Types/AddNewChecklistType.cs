using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Types
{
    public class AddNewChecklistType
    {
        public class AddNewChecklistTypeCommand : IRequest<Unit>
        {
            [Required]
            public string ChecklistType { get; set; }
            public int? OrderId { get; set; }
            public int AddedBy { get; set; }
        }
        public class Handler : IRequestHandler<AddNewChecklistTypeCommand, Unit>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(AddNewChecklistTypeCommand request, CancellationToken cancellationToken)
            {
                var validateChecklistType = await _context.ChecklistTypes.FirstOrDefaultAsync(
                    x => x.ChecklistType == request.ChecklistType,
                    cancellationToken);

                if (validateChecklistType != null)
                {
                    throw new Exception($"{request.ChecklistType} is already exist");
                }

                var checklistType = new ChecklistTypes
                {
                    ChecklistType = request.ChecklistType,
                    OrderId = request.OrderId,
                    AddedBy = request.AddedBy
                };

                await _context.ChecklistTypes.AddAsync(checklistType, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                
                return Unit.Value;
            }
        }
    }
}