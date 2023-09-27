using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Questions
{
    
    public class AddNewChecklistQuestions : ControllerBase
    {
        
        public class AddNewChecklistQuestionCommand : IRequest<Unit>
        {
            [Required]
            public string ChecklistQuestion{ get; set; }
            [Required]
            public int ChecklistTypeId { get; set; }
            public int? ProductTypeId { get; set; }
            [Required]
            public int AddedBy { get; set; }
            [Required]
            public AnswerType AnswerType { get; set; }
        }

        public class Handler : IRequestHandler<AddNewChecklistQuestionCommand, Unit>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(AddNewChecklistQuestionCommand request, CancellationToken cancellationToken)
            {
                var existingChecklistDesc =
                    await _context.ChecklistQuestions.FirstOrDefaultAsync(x =>
                        x.ChecklistQuestion == request.ChecklistQuestion && x.ChecklistTypeId == request.ChecklistTypeId, cancellationToken);


                if (existingChecklistDesc != null && existingChecklistDesc.ProductTypeId == request.ProductTypeId)
                {
                    throw new Exception($"{request.ChecklistQuestion} is already exist.");
                }


                var checklistDesc = new ChecklistQuestions
                {
                    ChecklistQuestion = request.ChecklistQuestion,
                    ChecklistTypeId = request.ChecklistTypeId,
                    ProductTypeId = request.ProductTypeId,
                    AnswerType = request.AnswerType,
                    AddedBy = request.AddedBy,
                };

                await _context.ChecklistQuestions.AddAsync(checklistDesc, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }
    }
}