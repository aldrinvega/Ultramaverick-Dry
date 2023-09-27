using System;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
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
            public AnswerType AnswerType { get; set; }
            public int ChecklistTypeId { get; set; }
            public int? ProductTypeId { get; set; }
        }


        public class Handler : IRequestHandler<UpdateChecklistQuestionCommand, Unit>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context ?? throw new ArgumentNullException(nameof(context));
            }

            public async Task<Unit> Handle(UpdateChecklistQuestionCommand request, CancellationToken cancellationToken)
            {
                var existingChecklistQuestion = await _context.ChecklistQuestions.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (existingChecklistQuestion == null)
                {
                    throw new Exception("Checklist question not found");
                }

                if (existingChecklistQuestion.ChecklistQuestion == request.ChecklistQuestion &&
                    existingChecklistQuestion.ChecklistTypeId != request.ChecklistTypeId &&
                    existingChecklistQuestion.ProductTypeId != request.ProductTypeId &&
                    existingChecklistQuestion.AnswerType == request.AnswerType
                    )
                {
                    throw new Exception("Checklist question is already exist");
                }

                if (IsUpdated(existingChecklistQuestion, request))
                {
                    existingChecklistQuestion.ChecklistQuestion = request.ChecklistQuestion;
                    existingChecklistQuestion.ChecklistTypeId = request.ChecklistTypeId;
                    existingChecklistQuestion.ProductTypeId = request.ProductTypeId;
                    existingChecklistQuestion.AnswerType = request.AnswerType;
                    existingChecklistQuestion.UpdatedAt = DateTime.Now;

                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    throw new Exception("No changes");
                }

                return Unit.Value;
            }

            private static bool IsUpdated(ChecklistQuestions existingQuestion, UpdateChecklistQuestionCommand request)
            {
                return existingQuestion.ChecklistQuestion != request.ChecklistQuestion ||
                       existingQuestion.AnswerType != request.AnswerType ||
                       existingQuestion.ChecklistTypeId != request.ChecklistTypeId ||
                       existingQuestion.ProductTypeId != request.ProductTypeId;
            }
        }
    }
}
