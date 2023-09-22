using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ELIXIR.DATA.DATA_ACCESS_LAYER.MODELS.QC_CHECKLIST;
using ELIXIR.DATA.DATA_ACCESS_LAYER.STORE_CONTEXT;
using MediatR;

namespace ELIXIR.DATA.DATA_ACCESS_LAYER.REPOSITORIES.QC_REPOSITORY.Checklist_Operation
{
    public class AddNewChecklist
    {
        public class AddNewChecklistCommand : IRequest<Unit>
        {
            public int ReceivingId { get; set; }
            public IList<ChecklistOpenFieldAnswer> OpenFieldAnswers { get; set; }
            public IList<ChecklistProductDimension> ProductDimensions { get; set; }
            public ReviewVerificationLogs ReviewVerificationLog { get; set; }
            public ChecklistCompliances ChecklistCompliance { get; set; }

            public class ChecklistOpenFieldAnswer
            {
                public int ChecklistQuestionId { get; set; }
                public string Remarks { get; set; }
            }

            public class ChecklistProductDimension
            {
                public int ChecklistQuestionId { get; set; }
                public string Standard { get; set; }
                public string Actual { get; set; }
            }

            public class ReviewVerificationLogs
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
                };

                await _context.QcChecklists.AddAsync(qcChecklist, cancellationToken);

                foreach (var openFieldAnswer in request.OpenFieldAnswers)
                {
                    var checklistOpenFieldAnswer = new ChecklistOpenFieldAnswer
                    {
                        QcChecklistId = qcChecklist.Id,
                        ChecklistQuestionId = openFieldAnswer.ChecklistQuestionId,
                        Remarks = openFieldAnswer.Remarks,
                    };
                    
                    await _context.QChecklistOpenFieldAnswers.AddAsync(checklistOpenFieldAnswer, cancellationToken);
                }

                foreach (var productDimension in request.ProductDimensions)
                {
                    var checklistProductDimension = new ChecklistProductDimension
                    {
                        QcChecklistId = qcChecklist.Id,
                        ChecklistQuestionId = productDimension.ChecklistQuestionId,
                        Standard = productDimension.Standard,
                        Actual = productDimension.Actual,
                    };

                    await _context.QChecklistProductDimensions.AddAsync(checklistProductDimension, cancellationToken);

                }

                var reviewVerificationLog = new ChecklistReviewVerificationLog
                {
                    QcChecklistId = qcChecklist.Id,
                    DispositionId = request.ReviewVerificationLog.DispositionId,
                    QtyAccepted = request.ReviewVerificationLog.QtyAccepted,
                    QtyRejected = request.ReviewVerificationLog.QtyRejected,
                    MonitoredBy = request.ReviewVerificationLog.MonitoredBy,
                    ReviewedBy = request.ReviewVerificationLog.ReviewedBy,
                    VerifiedBy = request.ReviewVerificationLog.VerifiedBy,
                    NotedBy = request.ReviewVerificationLog.NotedBy,
                };

                await _context.ChecklistReviewVerificationLogs.AddAsync(reviewVerificationLog, cancellationToken);


                var checklistCompliance = new ChecklistCompliance
                {
                    QcChecklistId = qcChecklist.Id,
                    Compliance = request.ChecklistCompliance.Compliance,
                    Description = request.ChecklistCompliance.ComplianceDescription,
                    RootCause = request.ChecklistCompliance.RootCause,
                };

                await _context.ChecklistCompliances.AddAsync(checklistCompliance, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
        }
    }
}