using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Questions
{
    public class UpdateChecklistQuestion
    {
        public class UpdateChecklistQuestionCommand : IRequest<Unit>
        {
            public int Id { get; set; }
            public string ChecklistQuestion { get; set; }
            public AnswerType AnswerType { get; set; }
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
                    await _context.ChecklistQuestions.Where(x => x.ChecklistQuestion == request.ChecklistQuestion).FirstOrDefaultAsync(cancellationToken);
                    
                if (isChecklistAlreadyExist.ChecklistQuestion == request.ChecklistQuestion &&
                    isChecklistAlreadyExist.AnswerType == request.AnswerType &&
                    isChecklistAlreadyExist.ChecklistTypeId == request.ChecklistTypeId
                    )
                {
                    return Unit.Value;
                }

                if(isChecklistAlreadyExist.ChecklistQuestion == request.ChecklistQuestion && isChecklistAlreadyExist.ChecklistTypeId != request.ChecklistTypeId)
                
                if (existingChecklistQuestion == null)
                {
                    throw new Exception("Checklist question is already exist");
                }

                existingChecklistQuestion.ChecklistQuestion = request.ChecklistQuestion;
                existingChecklistQuestion.ChecklistTypeId = request.ChecklistTypeId;
                existingChecklistQuestion.AnswerType = request.AnswerType;
                existingChecklistQuestion.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}