using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Operation
{
    public class AddNewChecklist
    {
        public class AddNewChecklistCommand : IRequest<Unit>
        {
            public int ReceivingId { get; set; }
            public int? ProductTypeId { get; set; }
            public IList<ChecklistOpenFieldAnswerCollection> OpenFieldAnswers { get; set; }
            public IList<ChecklistAnswer> ChecklistAnswers { get; set; }
            public IList<ChecklistProductDimensionCollection> ProductDimensions { get; set; }

            /* public ReviewVerificationLogs ReviewVerificationLog { get; set; }
             public ChecklistCompliances ChecklistCompliance { get; set; }
             public ChecklistOtherObservation ChecklistOtherObservations { get; set; }*/

            public class ChecklistAnswer
            {
                public int ChecklistQuestionId { get; set; }
                public bool Status { get; set; }
            }

            public class ChecklistOpenFieldAnswerCollection
            {
                public int ChecklistQuestionId { get; set; }
                public string Remarks { get; set; }
            }

            public class ChecklistProductDimensionCollection
            {
                public int ChecklistQuestionId { get; set; }
                public string Standard { get; set; }
                public string Actual { get; set; }
            }

            /*public class ReviewVerificationLogs
            {
                public int DispositionId { get; set; }
                public int QtyAccepted { get; set; }
                public int QtyRejected { get; set; }
                public string MonitoredBy { get; set; }
                public string ReviewedBy { get; set; }
                public string VerifiedBy { get; set; }
                public string NotedBy { get; set; }
            }

            public class ChecklistCompliances
            {
                public string Compliance { get; set; }
                public string ComplianceDescription { get; set; }
                public string RootCause { get; set; }
            }

            public class ChecklistOtherObservation
            {
                public string Observation { get; set; }
            }*/
        }

        public class Handler : IRequestHandler<AddNewChecklistCommand, Unit>
        {
            private readonly StoreContext _context;

            public Handler(StoreContext context)
            {
                _context = context;
            }

            public async Task<Unit> Handle(AddNewChecklistCommand request, CancellationToken cancellationToken)
            {
                var qcChecklist = new QCChecklist
                {
                    ReceivingId = request.ReceivingId,
                    ProductTypeId = request.ProductTypeId,
                };

                await _context.QcChecklists.AddAsync(qcChecklist, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                foreach (var checklistAnswer in request.ChecklistAnswers)
                {
                    var checklistAnswers = new ChecklistAnswers
                    {
                        QCChecklistId = qcChecklist.Id,
                        ChecklistQuestionsId = checklistAnswer.ChecklistQuestionId,
                        Status = checklistAnswer.Status
                    };

                    await _context.ChecklistAnswers.AddAsync(checklistAnswers, cancellationToken);
                }

                foreach (var openFieldAnswer in request.OpenFieldAnswers)
                {
                    var checklistOpenFieldAnswer = new ChecklistOpenFieldAnswer
                    {
                        QCChecklistId = qcChecklist.Id,
                        ChecklistQuestionId = openFieldAnswer.ChecklistQuestionId,
                        Remarks = openFieldAnswer.Remarks,
                    };

                    await _context.QChecklistOpenFieldAnswers.AddAsync(checklistOpenFieldAnswer, cancellationToken);
                }

                foreach (var productDimension in request.ProductDimensions)
                {
                    var checklistProductDimension = new ChecklistProductDimension
                    {
                        QCChecklistId = qcChecklist.Id,
                        ChecklistQuestionId = productDimension.ChecklistQuestionId,
                        Standard = productDimension.Standard,
                        Actual = productDimension.Actual,
                    };

                    await _context.QChecklistProductDimensions.AddAsync(checklistProductDimension, cancellationToken);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}