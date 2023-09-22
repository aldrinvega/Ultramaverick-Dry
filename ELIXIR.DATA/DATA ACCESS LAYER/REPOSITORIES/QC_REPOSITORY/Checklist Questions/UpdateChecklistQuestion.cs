using System;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Questions
{
    public class UpdateChecklistQuestion
    {
        public class UpdateChecklistQuestionCommand : IRequest<Unit>
        {
            public int Id { get; set; }
            public string ChecklistQuestion { get; set; }
            public int ChecklistTypeId { get; set; }
        }
        public class Handler : IRequestHandler<UpdateChecklistQuestionCommand, Unit>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(UpdateChecklistQuestionCommand request, CancellationToken cancellationToken)
            {
                var existingChecklistQuestion =
                    await _context.ChecklistQuestions.FirstOrDefaultAsync(x => x.Id == request.Id, 
                        cancellationToken);
                var isChecklistAlreadyExist =
                    await _context.ChecklistQuestions.AnyAsync(
                        x => x.ChecklistQuestion == request.ChecklistQuestion && x.ChecklistTypeId == request.ChecklistTypeId, cancellationToken);

                if (isChecklistAlreadyExist)
                {
                    throw new Exception("Checklist question is already exist");
                }
                
                if (existingChecklistQuestion == null)
                {
                    throw new Exception("Checklist question is not found");
                }

                existingChecklistQuestion.ChecklistQuestion = request.ChecklistQuestion;
                existingChecklistQuestion.ChecklistTypeId = request.ChecklistTypeId;
                existingChecklistQuestion.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}